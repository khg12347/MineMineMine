using System;
using System.Collections.Generic;
using MI.Data.Config;
using MI.Domain.Tile;
using UnityEngine;


namespace MI.Domain.Stage
{
    using Random = System.Random;

    // Jittered Grid 시드 기반 Flood-Fill 타일 생성 알고리즘
    // Phase 1: BFS 타일 배치 → Phase 2: 보물 배치 → Phase 3: 광물 오버레이
    // MonoBehaviour 의존 없는 순수 C# (값 타입 Unity API는 허용)
    public class MIFloodFillAlgorithm : IMITileAlgorithm
    {
        private readonly Random _rng;

        // BFS 이웃 탐색 방향 (상·하·좌·우)
        private static readonly int[] DR = { -1, 1, 0, 0 };
        private static readonly int[] DC = { 0, 0, -1, 1 };

        // seed: null이면 무작위 시드 사용
        public MIFloodFillAlgorithm(int? seed = null)
        {
            _rng = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        #region IMITileAlgorithm

        public FChunkData Generate(int startRow, int chunkRows, int gridWidth, MIStageConfig config)
        {
            // 레벨 결정 (startRow 기준)
            var (primaryLevel, secondaryLevel, blendT) = MILevelResolver.Resolve(startRow, config);

            if (primaryLevel == null)
                return CreateEmptyChunk(startRow, chunkRows, gridWidth);

            // 타일 설정 조회 딕셔너리 빌드 (ETileType → MITileConfig)
            var configLookup = BuildConfigLookup(primaryLevel);

            // Phase 1: 타일 타입 그리드 생성
            var typeGrid = new ETileType[chunkRows, gridWidth];
            var weights = BuildBlendedWeights(primaryLevel, secondaryLevel, blendT);
            var seeds = MISeedPlacer.PlaceSeeds(gridWidth, chunkRows, primaryLevel.SeedDensity, weights, _rng);
            ExpandSeeds(typeGrid, seeds, chunkRows, gridWidth);
            FillRemaining(typeGrid, weights, chunkRows, gridWidth);

            // FTileData 배열 생성 (configLookup 으로 내구도 등 채우기)
            var cells = HydrateCells(typeGrid, chunkRows, gridWidth, configLookup);

            // Phase 2: 보물 상자 배치
            var treasures = PlaceTreasures(typeGrid, primaryLevel, chunkRows, gridWidth);

            // Phase 3: 광물 오버레이
            ApplyMinerals(cells, typeGrid, primaryLevel, chunkRows, gridWidth);

            return new FChunkData { Cells = cells, Treasures = treasures, StartRow = startRow };
        }

        #endregion IMITileAlgorithm

        #region Phase 1: Flood-Fill

        // 다중 출발점 BFS로 시드 타일 타입을 주변으로 확장
        private void ExpandSeeds(
            ETileType[,] grid,
            List<(int Col, int Row, ETileType Type)> seeds,
            int chunkRows, int gridWidth)
        {
            var assigned = new bool[chunkRows, gridWidth];
            var queue = new Queue<(int Row, int Col)>();

            // 시드 초기 배치
            foreach (var (sc, sr, type) in seeds)
            {
                if (sr < 0 || sr >= chunkRows || sc < 0 || sc >= gridWidth) continue;
                if (assigned[sr, sc]) continue;

                grid[sr, sc] = type;
                assigned[sr, sc] = true;
                queue.Enqueue((sr, sc));
            }

            // BFS 확장 — 먼저 배치된 시드가 더 넓은 영역을 차지하여 자연스러운 군집 형성
            while (queue.Count > 0)
            {
                var (r, c) = queue.Dequeue();
                var type = grid[r, c];

                for (int d = 0; d < 4; d++)
                {
                    int nr = r + DR[d];
                    int nc = c + DC[d];
                    if (nr < 0 || nr >= chunkRows || nc < 0 || nc >= gridWidth) continue;
                    if (assigned[nr, nc]) continue;

                    grid[nr, nc] = type;
                    assigned[nr, nc] = true;
                    queue.Enqueue((nr, nc));
                }
            }
        }

        // BFS 후 미배정 셀을 가중치 기반으로 채움
        private void FillRemaining(
            ETileType[,] grid, List<FTileWeight> weights,
            int chunkRows, int gridWidth)
        {
            for (int r = 0; r < chunkRows; r++)
                for (int c = 0; c < gridWidth; c++)
                {
                    if (grid[r, c] == ETileType.None)
                        grid[r, c] = MISeedPlacer.SelectWeighted(weights, _rng);
                }
        }

        #endregion Phase 1: Flood-Fill

        #region Phase 2: Treasure Placement

        private List<FTreasurePlacement> PlaceTreasures(
            ETileType[,] typeGrid, MILevelData level,
            int chunkRows, int gridWidth)
        {
            var treasures = new List<FTreasurePlacement>();
            if (level.TreasureWeights == null || level.TreasureWeights.Count == 0)
                return treasures;

            for (int r = 0; r < chunkRows && treasures.Count < level.MaxTreasuresPerChunk; r++)
                for (int c = 0; c < gridWidth && treasures.Count < level.MaxTreasuresPerChunk; c++)
                {
                    if ((float)_rng.NextDouble() > level.TreasureChance) continue;

                    var treasureType = SelectWeightedTreasure(level.TreasureWeights);
                    if (treasureType == ETreasureType.None) continue;

                    treasures.Add(new FTreasurePlacement
                    {
                        X = c,
                        Y = r,
                        Type = treasureType,
                        Width = 1,
                        Height = 1
                    });
                }

            return treasures;
        }

        private ETreasureType SelectWeightedTreasure(IReadOnlyList<FTreasureWeight> weights)
        {
            float total = 0f;
            foreach (var w in weights) total += w.Weight;
            if (total <= 0f) return ETreasureType.None;

            float roll = (float)_rng.NextDouble() * total;
            float cum = 0f;
            foreach (var w in weights)
            {
                cum += w.Weight;
                if (roll < cum) return w.TreasureType;
            }
            return ETreasureType.None;
        }

        #endregion Phase 2: Treasure Placement

        #region Phase 3: Mineral Overlay

        private void ApplyMinerals(
            FTileData[,] cells, ETileType[,] typeGrid, MILevelData level,
            int chunkRows, int gridWidth)
        {
            if (level.MineralWeights == null || level.MineralWeights.Count == 0) return;

            var affinities = level.MineralAffinities; // Dictionary<ETileType, List<FMineralAffinity>>

            for (int r = 0; r < chunkRows; r++)
            {
                for (int c = 0; c < gridWidth; c++)
                {
                    var tileType = typeGrid[r, c];
                    if (tileType == ETileType.None) continue;

                    // 타일 친화도 조회
                    if (affinities?.TryGetValue(tileType, out var tileAffinities) == true)
                    {
                        var (mineralType, density) = SelectMineral(level.MineralWeights, tileAffinities);
                        if (mineralType == EMineralType.None) continue;

                        // struct 는 값 복사이므로 수정 후 재할당
                        var cell = cells[r, c];
                        cell.MineralDrop = new FMineralDropEntry
                        {
                            MineralType = mineralType,
                            Density     = density,
                        };
                        cells[r, c] = cell;
                    }
                }
            }
        }

        // 섹션 가중치 × 친화도 복합 가중치로 광물 종류·밀도 결정. 미선택 시 None 반환.
        private (EMineralType Type, EMineralDensity Density) SelectMineral(
            IReadOnlyList<FMineralWeight> sectionWeights,
            List<FMineralAffinity> tileAffinities)
        {
            // 각 광물의 복합 가중치 = 섹션 가중치 × 친화도
            float combinedTotal = 0f;
            var combined = new List<(EMineralType Type, EMineralDensity Density, float Weight)>(sectionWeights.Count);

            foreach (var mw in sectionWeights)
            {
                if (mw.MineralType == EMineralType.None) continue;

                float affinity = 1f;
                if (tileAffinities != null)
                {
                    foreach (var af in tileAffinities)
                    {
                        if (af.MineralType == mw.MineralType) { affinity = af.Weight; break; }
                    }
                }
                float w = mw.Weight * affinity;
                combined.Add((mw.MineralType, mw.Density, w));
                combinedTotal += w;
            }

            if (combinedTotal <= 0f) return (EMineralType.None, EMineralDensity.None);

            // combinedTotal + 1 범위에서 roll → 초과분은 None (광물 없음)
            float roll = (float)_rng.NextDouble() * (combinedTotal + 1f);
            if (roll > combinedTotal) return (EMineralType.None, EMineralDensity.None);

            float cum = 0f;
            foreach (var (type, density, w) in combined)
            {
                cum += w;
                if (roll < cum) return (type, density);
            }
            return (EMineralType.None, EMineralDensity.None);
        }

        #endregion Phase 3: Mineral Overlay

        #region Helper

        // 타일 타입 그리드 → FTileData 배열 생성
        private FTileData[,] HydrateCells(
            ETileType[,] typeGrid, int chunkRows, int gridWidth,
            Dictionary<ETileType, MITileConfig> configLookup)
        {
            var cells = new FTileData[chunkRows, gridWidth];
            for (int r = 0; r < chunkRows; r++)
            {
                for (int c = 0; c < gridWidth; c++)
                {
                    var tileType = typeGrid[r, c];
                    cells[r, c] = configLookup.TryGetValue(tileType, out var cfg)
                        ? cfg.CreateTileData()
                        : new FTileData { TileType = tileType };
                }
            }
            return cells;
        }

        // 블렌딩 적용된 타일 가중치 반환
        private List<FTileWeight> BuildBlendedWeights(
            MILevelData primary, MILevelData secondary, float blendT)
        {
            if (secondary == null || blendT <= 0f)
            {
                return new List<FTileWeight>(primary.TileWeights);
            }

            var result = new List<FTileWeight>(primary.TileWeights.Count);
            foreach (var tw in primary.TileWeights)
            {
                float secW = 0f;
                if (secondary.TileWeights != null)
                {

                    foreach (var sw in secondary.TileWeights)
                    {
                        if (sw.TileType == tw.TileType)
                        {
                            secW = sw.Weight;
                            break;
                        }
                    }
                }

                result.Add(new FTileWeight
                {
                    TileType = tw.TileType,
                    Weight = Mathf.Lerp(tw.Weight, secW, blendT)
                });
            }
            return result;
        }

        // TileConfigs → ETileType 딕셔너리 빌드
        private static Dictionary<ETileType, MITileConfig> BuildConfigLookup(MILevelData level)
        {
            var lookup = new Dictionary<ETileType, MITileConfig>();
            if (level?.TileConfigs == null) return lookup;

            foreach (var cfg in level.TileConfigs)
            {
                if (cfg != null && !lookup.ContainsKey(cfg.TileType))
                {
                    lookup[cfg.TileType] = cfg;
                }
            }
            return lookup;
        }

        // 설정 없음 또는 레벨 null 시 빈 청크 반환
        private static FChunkData CreateEmptyChunk(int startRow, int chunkRows, int gridWidth)
        {
            return new FChunkData
            {
                Cells = new FTileData[chunkRows, gridWidth],
                Treasures = new List<FTreasurePlacement>(),
                StartRow = startRow
            };
        }

        #endregion Helper
    }
}
