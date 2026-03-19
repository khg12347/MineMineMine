using System;
using System.Collections.Generic;
using MI.Core.Pool;
using MI.Data.Config;
using MI.Domain.Tile;
using MI.Presentation.World.Tile;
using UnityEngine;

namespace MI.Core.Stage
{
    /// <summary>
    /// MIChunkBuffer 에서 행 데이터를 소비하여 MITileModel 인스턴스를 스폰/제거합니다.
    /// 풀링(MIPoolManager)을 사용하며, 행 인덱스별로 타일 목록을 관리합니다.
    /// </summary>
    public class MITileSpawner
    {
        private readonly GameObject                      _tilePrefab;
        private readonly Transform                       _tileParent;
        private readonly float                           _tileSize;
        private readonly float                           _stageStartX;
        private readonly int                             _stageWidth;
        private readonly Dictionary<ETileType, MITileConfig> _configLookup;

        // 행 인덱스 → 해당 행의 타일 모델 목록
        private readonly Dictionary<int, List<MITileModel>> _tilesByRow = new();

        // 다음으로 스폰해야 할 절대 행 인덱스
        private int _spawnedRows;

        /// <summary>
        /// MITileSpawner 생성자.
        /// </summary>
        /// <param name="tilePrefab">타일 프리팹 (MITileModel 컴포넌트 포함)</param>
        /// <param name="tileParent">스폰된 타일의 부모 Transform</param>
        /// <param name="tileSize">타일 1칸의 월드 단위 크기</param>
        /// <param name="stageStartX">타일 배치 시작 X 월드 좌표</param>
        /// <param name="stageWidth">가로 타일 수</param>
        /// <param name="configLookup">ETileType → MITileConfig 조회 딕셔너리</param>
        public MITileSpawner(
            GameObject tilePrefab, Transform tileParent,
            float tileSize, float stageStartX, int stageWidth,
            Dictionary<ETileType, MITileConfig> configLookup)
        {
            _tilePrefab   = tilePrefab;
            _tileParent   = tileParent;
            _tileSize     = tileSize;
            _stageStartX  = stageStartX;
            _stageWidth   = stageWidth;
            _configLookup = configLookup;
        }

        // ── 스폰 ─────────────────────────────────────────────────────────

        /// <summary>
        /// buffer 에서 행 데이터를 소비하여 targetRow 행까지 타일을 스폰합니다.
        /// </summary>
        public void SpawnRowsUpTo(int targetRow, MIChunkBuffer buffer)
        {
            while (_spawnedRows < targetRow)
            {
                if (!buffer.TryDequeueRow(_spawnedRows, out var rowData, out var treasures))
                    break; // 버퍼에 데이터가 없으면 중단

                SpawnRow(_spawnedRows, rowData);
                _spawnedRows++;
            }
        }

        private void SpawnRow(int row, FTileData[] rowData)
        {
            var rowTiles = new List<MITileModel>(_stageWidth);

            for (int x = 0; x < _stageWidth && x < rowData.Length; x++)
            {
                var data     = rowData[x];
                if (data.TileType == ETileType.None) continue;

                if (!_configLookup.TryGetValue(data.TileType, out var config))
                    continue; // 설정이 없는 타일 타입은 스킵

                var pos   = new Vector3(_stageStartX + x * _tileSize, -row * _tileSize, 0f);
                var model = MIPoolManager.Instance.Get<MITileModel>(_tilePrefab, pos, Quaternion.identity, _tileParent);

                model.ResetTile(config, data);
                model.SetDestroyedCallback(OnTileDestroyed);
                rowTiles.Add(model);
            }

            _tilesByRow[row] = rowTiles;
        }

        // ── 제거 ─────────────────────────────────────────────────────────

        /// <summary>
        /// cullBeforeRow 보다 위에 있는 행의 타일을 풀에 반환합니다.
        /// </summary>
        public void CullAbove(int cullBeforeRow)
        {
            var toRemove = new List<int>();

            foreach (var kv in _tilesByRow)
            {
                if (kv.Key >= cullBeforeRow) continue;

                foreach (var tile in kv.Value)
                    if (tile != null && tile.gameObject.activeSelf)
                        MIPoolManager.Instance.Return(tile);

                toRemove.Add(kv.Key);
            }

            foreach (int row in toRemove)
                _tilesByRow.Remove(row);
        }

        // ── 콜백 ─────────────────────────────────────────────────────────

        /// <summary>타일이 파괴(Break)될 때 MITileModel 에서 호출됩니다.</summary>
        private void OnTileDestroyed(MITileModel tile)
        {
            foreach (var kv in _tilesByRow)
            {
                if (kv.Value.Remove(tile))
                    break;
            }
        }
    }
}
