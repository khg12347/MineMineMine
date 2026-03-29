using MI.Data.Config;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MI.Domain.Stage
{
    /// <summary>
    /// 배경 Tilemap에 행 단위로 배경 타일을 배치합니다.
    /// 메인 타일과 변형 타일을 가중치 기반으로 선택합니다.
    /// </summary>
    public class MIBackgroundPainter
    {
        private readonly Tilemap _tilemap;
        private readonly MIBackgroundConfig _config;
        private readonly int _totalWidth;
        private readonly int _leftColX;

        // 가중치 캐시
        private readonly float _totalWeight;

        // 컬링 기준: 이 행 미만은 이미 제거됨
        private int _culledUpToRow = 0;

        /// <summary>현재까지 배경이 칠해진 최대 행 인덱스</summary>
        public int PaintedUpToRow { get; private set; } = -1;

        /// <summary>
        /// MIBackgroundPainter 생성자.
        /// </summary>
        /// <param name="bgTilemap">배경용 Tilemap 참조</param>
        /// <param name="config">배경 설정 SO</param>
        /// <param name="totalWidth">벽 포함 전체 가로 칸 수 (GridWidth + 2)</param>
        /// <param name="leftColX">가장 왼쪽 열의 타일맵 X 좌표</param>
        public MIBackgroundPainter(Tilemap bgTilemap, MIBackgroundConfig config, int totalWidth, int leftColX)
        {
            _tilemap = bgTilemap;
            _config = config;
            _totalWidth = totalWidth;
            _leftColX = leftColX;

            // 전체 가중치 합 미리 계산
            _totalWeight = config.MainWeight;
            foreach (var variant in config.Variants)
                _totalWeight += variant.Weight;
        }

        /// <summary>
        /// fromRow 부터 toRow 까지 배경 타일을 배치합니다.
        /// 이미 칠해진 행은 건너뜁니다.
        /// </summary>
        public void PaintRows(int fromRow, int toRow)
        {
            int startRow = Mathf.Max(fromRow, PaintedUpToRow + 1);

            for (int row = startRow; row <= toRow; row++)
            {
                for (int col = 0; col < _totalWidth; col++)
                {
                    // 타일맵 좌표: X = leftColX + col, Y = -row (아래로 갈수록 음수)
                    var cellPos = new Vector3Int(_leftColX + col, -row, 0);
                    _tilemap.SetTile(cellPos, PickTile());
                }
            }

            if (toRow > PaintedUpToRow)
                PaintedUpToRow = toRow;
        }

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
                for (int col = 0; col < _totalWidth; col++)
                    _tilemap.SetTile(new Vector3Int(_leftColX + col, -row, 0), null);
            }

            _culledUpToRow = cullBeforeRow;
        }

        #endregion Culling

        /// <summary>가중치 기반으로 메인 또는 변형 타일 중 하나를 선택합니다.</summary>
        private TileBase PickTile()
        {
            float roll = Random.Range(0f, _totalWeight);

            // 메인 타일 범위
            if (roll < _config.MainWeight) return _config.MainTile;

            // 변형 타일 순서대로 범위 확인
            float cursor = _config.MainWeight;
            foreach (var variant in _config.Variants)
            {
                cursor += variant.Weight;
                if (roll < cursor) return variant.Tile;
            }

            // 부동소수점 오차 대비 폴백
            return _config.MainTile;
        }
    }
}
