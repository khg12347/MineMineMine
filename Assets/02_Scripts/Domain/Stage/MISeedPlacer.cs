using System;
using System.Collections.Generic;
using MI.Domain.Tile;
using UnityEngine;

namespace MI.Domain.Stage
{
    using Random = System.Random;
    /// <summary>
    /// Jittered Grid 방식으로 FloodFill 시드를 배치하는 정적 유틸 클래스.
    /// 그리드를 균일한 셀로 분할하고, 각 셀 내 임의 위치에 시드를 배치합니다.
    /// </summary>
    public static class MISeedPlacer
    {
        /// <summary>
        /// 시드 목록을 생성합니다.
        /// </summary>
        /// <param name="gridWidth">가로 타일 수</param>
        /// <param name="chunkRows">세로 행 수</param>
        /// <param name="seedDensity">시드 밀도 (0.01~1.0). 높을수록 시드가 촘촘합니다.</param>
        /// <param name="weights">타일 가중치 목록 (시드 타입 결정에 사용)</param>
        /// <param name="rng">재현성을 위한 System.Random 인스턴스</param>
        public static List<(int Col, int Row, ETileType Type)> PlaceSeeds(
            int gridWidth, int chunkRows, float seedDensity,
            IReadOnlyList<FTileWeight> weights, Random rng)
        {
            // seedDensity 로부터 셀 크기 계산
            float cellSize = Mathf.Sqrt(1f / Mathf.Max(seedDensity, 0.01f));
            int cellCols = Mathf.Max(1, Mathf.FloorToInt(gridWidth  / cellSize));
            int cellRows = Mathf.Max(1, Mathf.FloorToInt(chunkRows  / cellSize));

            float cw = (float)gridWidth  / cellCols;
            float ch = (float)chunkRows  / cellRows;

            var seeds = new List<(int, int, ETileType)>(cellCols * cellRows);

            for (int cr = 0; cr < cellRows; cr++)
            for (int cc = 0; cc < cellCols; cc++)
            {
                int col = Mathf.Clamp((int)(cc * cw + rng.NextDouble() * cw), 0, gridWidth  - 1);
                int row = Mathf.Clamp((int)(cr * ch + rng.NextDouble() * ch), 0, chunkRows  - 1);

                var type = SelectWeighted(weights, rng);
                seeds.Add((col, row, type));
            }

            return seeds;
        }

        /// <summary>가중치에 따라 ETileType 을 무작위 선택합니다.</summary>
        public static ETileType SelectWeighted(IReadOnlyList<FTileWeight> weights, Random rng)
        {
            if (weights == null || weights.Count == 0) return ETileType.None;

            float total = 0f;
            foreach (var w in weights) total += w.Weight;
            if (total <= 0f) return ETileType.None;

            float roll = (float)rng.NextDouble() * total;
            float cum  = 0f;
            foreach (var w in weights)
            {
                cum += w.Weight;
                if (roll < cum) return w.TileType;
            }

            return weights[weights.Count - 1].TileType;
        }
    }
}
