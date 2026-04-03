using System.Collections.Generic;
using MI.Core.Pool;
using MI.Data.Config;
using MI.Domain.Stage;
using MI.Domain.Status;
using MI.Domain.Tile;
using MI.Presentation.World.Camera;
using MI.Presentation.World.Pickaxe;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MI.Presentation.World.Stage
{
    using Camera = UnityEngine.Camera;

    // 무한 하강 스테이지 총괄. 깊이/청크/타일/벽/카메라 각 모듈에 위임.
    public class MIStageOrchestrator : MonoBehaviour
    {
        #region Inspector

        [Title("스테이지 설정")]
        [Required]
        [SerializeField] private MIStageConfig _stageConfig;

        [LabelText("랜덤 시드 (0 = 매번 랜덤)")]
        [SerializeField] private int _randomSeed = 0;

        [Title("타일")]
        [Required]
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private Transform _tileParent;

        //[Title("광물")]
        //[Required]
        //[SerializeField]

        [Title("곡괭이")]
        [Required]
        [SerializeField] private MIPickaxeController _pickaxe;
        [Required]
        [SerializeField] private MIPickaxeConfig _pickaxeConfig;

        [Title("카메라")]
        [Required]
        [SerializeField] private Camera _mainCamera;
        [PropertyRange(1f, 10f)]
        [SerializeField] private float _cameraFollowSpeed = 3f;
        [Required]
        [SerializeField] private MICameraFollower _cameraFollower;

        [Title("배경/벽")]
        [SerializeField] private Tilemap _backgroundTilemap;
        [SerializeField] private Tilemap _wallTilemap;
        [SerializeField] private MIBackgroundConfig _backgroundConfig;
        [SerializeField] private MIWallConfig _wallConfig;

        [Title("청크 스폰 / 컬 설정")]
        [InfoBox("곡괭이 아래 미리 생성해 둘 행 수")]
        [PropertyRange(4, 64)]
        [SerializeField] private int _spawnAheadRows = 16;

        [InfoBox("곡괭이 위로 이 행 수를 초과하면 타일 제거 (메모리 절약)")]
        [PropertyRange(4, 32)]
        [SerializeField] private int _cullRowsAbove = 8;

        private FPoolConfig _tilePoolConfig = new FPoolConfig() { InitialSize = 256, GrowSize = 128 };

        #endregion Inspector

        #region Runtime Modules

        private MIDepthTracker _depthTracker;
        private MIChunkBuffer _chunkBuffer;
        private IMITileAlgorithm _algorithm;
        private MITileSpawner _tileSpawner;
        private MIWallSpawner _wallSpawner;
        private MIBackgroundPainter _backgroundPainter;
        private MIWallPainter _wallPainter;

        // 깊이 변경 감지용
        private int _lastReportedDepth = -1;

        #endregion Runtime Modules

        #region Unity Lifecycle

        private void Awake()
        {
        }

        private void Start()
        {
            // 타일 배치 시작 X 계산
            float stageStartX = CalculateStageStartX();

            // 타일 설정 조회 딕셔너리 빌드
            var tileConfigLookup = BuildTileConfigLookup();

            // 모듈 초기화
            _depthTracker = new MIDepthTracker(_stageConfig.RowHeight);
            _chunkBuffer = new MIChunkBuffer();
            _algorithm = new MIFloodFillAlgorithm(_randomSeed == 0 ? (int?)null : _randomSeed);
            _tileSpawner = new MITileSpawner(
                _tilePrefab, _tileParent,
                _stageConfig.RowHeight, stageStartX, _stageConfig.GridWidth,
                tileConfigLookup, _tilePoolConfig);
            _wallSpawner = new MIWallSpawner(
                _pickaxeConfig,
                _stageConfig.RowHeight, stageStartX, _stageConfig.GridWidth);

            // 카메라 추적 초기화
            _cameraFollower.Initialize(_mainCamera, _cameraFollowSpeed);

            // 배경/벽 Painter 초기화
            if (_backgroundTilemap != null && _backgroundConfig != null &&
                _wallTilemap != null && _wallConfig != null)
            {
                // Grid 위치를 stageStartX 기반으로 정렬
                // 타일맵 cell(leftColX=0)이 왼쪽 벽, cell(1)이 게임 타일 col=0과 일치하도록 설정
                float tileSize = _stageConfig.RowHeight;
                _backgroundTilemap.layoutGrid.transform.position =
                    new Vector3(stageStartX - 1.5f * tileSize, -0.5f * tileSize, 0f);

                int leftColX = 1;
                int rightColX = _stageConfig.GridWidth;
                int totalWidth = _stageConfig.GridWidth + 2;

                _backgroundPainter = new MIBackgroundPainter(
                    _backgroundTilemap, _backgroundConfig, totalWidth, leftColX);
                _wallPainter = new MIWallPainter(
                    _wallTilemap, _wallConfig, leftColX, rightColX);

                // 벽 타일맵: X는 타일 좌표 기반이므로 Y 오프셋만 적용
                _wallTilemap.transform.localPosition = new Vector3(0f, _wallConfig.PositionOffsetY, 0f);

                var bgOffset = _backgroundConfig.PositionOffset;
                _backgroundTilemap.transform.localPosition = new Vector3(bgOffset.x, bgOffset.y, 0f);
            }

            // 초기 청크 생성 + 타일 스폰
            FillChunkBuffer(0);
            _tileSpawner.SpawnRowsUpTo(_spawnAheadRows, _chunkBuffer);

            // 초기 배경/벽 페인팅 (타일 스폰과 동일한 범위)
            _backgroundPainter?.PaintRows(0, _spawnAheadRows - 1);
            _wallPainter?.PaintRows(0, _spawnAheadRows - 1);

            // 곡괭이 초기 스폰
            _pickaxe.SpawnAtOffScreen(_mainCamera);
        }

        private void Update()
        {
            float pickaxeY = _pickaxe.transform.position.y;

            // 1. 깊이 측정
            _depthTracker.Update(pickaxeY);
            int currentRow = _depthTracker.CurrentRow;

            // 2. 청크 버퍼 보충
            if (_chunkBuffer.NeedsMoreChunks(currentRow, _spawnAheadRows))
            {
                FillChunkBuffer(_chunkBuffer.GeneratedUpToRow);
            }

            // 3. 타일 스폰 / 제거
            int targetRow = currentRow + _spawnAheadRows;
            int cullRow = currentRow - _cullRowsAbove;
            _tileSpawner.SpawnRowsUpTo(targetRow, _chunkBuffer);
            _tileSpawner.CullAbove(cullRow);

            // 배경/벽 확장 및 컬링 (타일과 동일한 규칙)
            _backgroundPainter?.PaintRows(0, targetRow);
            _backgroundPainter?.CullAbove(cullRow);
            _wallPainter?.PaintRows(0, targetRow);
            _wallPainter?.CullAbove(cullRow);

            // 4. 벽 업데이트
            _wallSpawner.UpdateWalls(_mainCamera.transform.position.y);

            // 5. 카메라 추적
            _cameraFollower.Follow(pickaxeY);

            // 6. 깊이 상태 갱신 (변경 시에만)
            int maxRow = _depthTracker.MaxDepthRow;
            if (maxRow > _lastReportedDepth)
            {
                _lastReportedDepth = maxRow;
                MIStatusManager.Instance.UpdateDepth(maxRow);
            }
        }

        #endregion Unity Lifecycle

        #region Helper

        private void FillChunkBuffer(int startRow)
        {
            var chunk = _algorithm.Generate(
                startRow, _stageConfig.ChunkRows, _stageConfig.GridWidth, _stageConfig);
            _chunkBuffer.Enqueue(chunk);
        }

        private float CalculateStageStartX()
        {
            float leftEdge = _mainCamera.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f)).x;
            float rightEdge = _mainCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f)).x;
            float viewWidth = rightEdge - leftEdge;
            float stageWidth = _stageConfig.GridWidth * _stageConfig.RowHeight;
            return leftEdge + (viewWidth - stageWidth) * 0.5f + _stageConfig.RowHeight * 0.5f;
        }

        // 전 레벨 TileConfigs 취합 → ETileType 딕셔너리 빌드. 중복 시 첫 등록 우선.
        private Dictionary<ETileType, MITileConfig> BuildTileConfigLookup()
        {
            var lookup = new Dictionary<ETileType, MITileConfig>();
            if (_stageConfig?.Levels == null) return lookup;

            foreach (var level in _stageConfig.Levels)
            {
                if (level?.TileConfigs == null) continue;
                foreach (var cfg in level.TileConfigs)
                    if (cfg != null && !lookup.ContainsKey(cfg.TileType))
                        lookup[cfg.TileType] = cfg;
            }
            return lookup;
        }

        #endregion Helper
    }
}
