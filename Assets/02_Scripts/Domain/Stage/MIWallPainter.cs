using MI.Data.Config;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MI.Domain.Stage
{
    /// <summary>
    /// 벽 Tilemap에 행 단위로 좌우 벽 타일을 배치/컬링합니다.
    /// 왼쪽 벽은 순환 배열, 오른쪽 벽은 왼쪽 타일을 X축 flip + 별도 오프셋으로 배치합니다.
    /// </summary>
    public class MIWallPainter
    {
        private readonly Tilemap      _tilemap;
        private readonly MIWallConfig _config;
        private readonly int          _leftColX;
        private readonly int          _rightColX;

        // 컬링 기준: 이 행 미만은 이미 제거됨
        private int _culledUpToRow = 0;

        /// <summary>현재까지 벽이 칠해진 최대 행 인덱스</summary>
        public int PaintedUpToRow { get; private set; } = -1;

        /// <summary>
        /// MIWallPainter 생성자.
        /// </summary>
        /// <param name="wallTilemap">벽용 Tilemap 참조</param>
        /// <param name="config">벽 설정 SO</param>
        /// <param name="leftColX">왼쪽 벽 열의 타일맵 X 좌표</param>
        /// <param name="rightColX">오른쪽 벽 열의 타일맵 X 좌표</param>
        public MIWallPainter(Tilemap wallTilemap, MIWallConfig config, int leftColX, int rightColX)
        {
            _tilemap   = wallTilemap;
            _config    = config;
            _leftColX  = leftColX;
            _rightColX = rightColX;
        }

        #region PaintRows

        /// <summary>
        /// fromRow 부터 toRow 까지 좌우 벽 타일을 배치합니다.
        /// 이미 칠해진 행은 건너뜁니다.
        /// </summary>
        public void PaintRows(int fromRow, int toRow)
        {
            int startRow = Mathf.Max(fromRow, PaintedUpToRow + 1);

            for (int row = startRow; row <= toRow; row++)
            {
                // 왼쪽 벽
                _tilemap.SetTile(new Vector3Int(_leftColX - 1, -row, 0), _config.WallTempTile);
                _tilemap.SetTile(new Vector3Int(_leftColX,     -row, 0), _config.GetLeftWallTile(row));

                // 오른쪽 벽
                _tilemap.SetTile(new Vector3Int(_rightColX + 1, -row, 0), _config.WallTempTile);
                _tilemap.SetTile(new Vector3Int(_rightColX,     -row, 0), _config.GetRightWallTile(row));
            }

            if (toRow > PaintedUpToRow)
                PaintedUpToRow = toRow;
        }

        #endregion PaintRows

        #region Culling

        /// <summary>
        /// cullBeforeRow 미만의 행 타일을 타일맵에서 제거합니다.
        /// MITileSpawner.CullAbove 와 동일한 규칙으로 호출하면 됩니다.
        /// </summary>
        public void CullAbove(int cullBeforeRow)
        {
            if (cullBeforeRow <= _culledUpToRow) return;

            for (int row = _culledUpToRow; row < cullBeforeRow; row++)
            {
                _tilemap.SetTile(new Vector3Int(_leftColX - 1,  -row, 0), null);
                _tilemap.SetTile(new Vector3Int(_leftColX,      -row, 0), null);
                _tilemap.SetTile(new Vector3Int(_rightColX,     -row, 0), null);
                _tilemap.SetTile(new Vector3Int(_rightColX + 1, -row, 0), null);
            }

            _culledUpToRow = cullBeforeRow;
        }

        #endregion Culling
    }
}
