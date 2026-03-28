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

namespace MI.Presentation.World.Stage
{
    using Camera = UnityEngine.Camera;
    /// <summary>
    /// 무한 하강 스테이지를 총괄하는 오케스트라 MonoBehaviour.
    /// 각 책임은 전담 모듈에 위임합니다:
    ///   - MIDepthTracker       : 깊이 측정
    ///   - MIChunkBuffer        : 청크 데이터 큐 관리
    ///   - IMITileAlgorithm     : 타일 생성 알고리즘 (MIFloodFillAlgorithm)
    ///   - MITileSpawner        : 타일 인스턴스 생성/제거
    ///   - MIWallSpawner        : 좌우 벽 생성/위치 업데이트
    ///   - IMICameraFollower    : 카메라 추적 (MICameraFollower)
    /// </summary>
    public class MIStageOrchestrator : MonoBehaviour
    {
        // ── Inspector 설정 ────────────────────────────────────────────────

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

        [Title("청크 스폰 / 컬 설정")]
        [InfoBox("곡괭이 아래 미리 생성해 둘 행 수")]
        [PropertyRange(4, 64)]
        [SerializeField] private int _spawnAheadRows = 16;

        [InfoBox("곡괭이 위로 이 행 수를 초과하면 타일 제거 (메모리 절약)")]
        [PropertyRange(4, 32)]
        [SerializeField] private int _cullRowsAbove = 8;

        private FPoolConfig _tilePoolConfig = new FPoolConfig() {InitialSize = 256, GrowSize = 128};

        // ── 런타임 모듈 ────────────────────────────────────────────────────

        private MIDepthTracker  _depthTracker;
        private MIChunkBuffer   _chunkBuffer;
        private IMITileAlgorithm _algorithm;
        private MITileSpawner   _tileSpawner;
        private MIWallSpawner   _wallSpawner;

        // 깊이 변경 감지용
        private int _lastReportedDepth = -1;

        // ── Unity 생명주기 ─────────────────────────────────────────────────
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
            _chunkBuffer  = new MIChunkBuffer();
            _algorithm    = new MIFloodFillAlgorithm(_randomSeed == 0 ? (int?)null : _randomSeed);
            _tileSpawner  = new MITileSpawner(
                _tilePrefab, _tileParent,
                _stageConfig.RowHeight, stageStartX, _stageConfig.GridWidth,
                tileConfigLookup, _tilePoolConfig);
            _wallSpawner  = new MIWallSpawner(
                _pickaxeConfig, _mainCamera,
                _stageConfig.RowHeight, stageStartX, _stageConfig.GridWidth);

            // 카메라 추적 초기화
            _cameraFollower.Initialize(_mainCamera, _cameraFollowSpeed);

            // 초기 청크 생성 + 타일 스폰
            FillChunkBuffer(0);
            _tileSpawner.SpawnRowsUpTo(_spawnAheadRows, _chunkBuffer);

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
                FillChunkBuffer(_chunkBuffer.GeneratedUpToRow);

            // 3. 타일 스폰 / 제거
            _tileSpawner.SpawnRowsUpTo(currentRow + _spawnAheadRows, _chunkBuffer);
            _tileSpawner.CullAbove(currentRow - _cullRowsAbove);

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

        // ── 헬퍼 ──────────────────────────────────────────────────────────

        private void FillChunkBuffer(int startRow)
        {
            var chunk = _algorithm.Generate(
                startRow, _stageConfig.ChunkRows, _stageConfig.GridWidth, _stageConfig);
            _chunkBuffer.Enqueue(chunk);
        }

        private float CalculateStageStartX()
        {
            float leftEdge  = _mainCamera.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f)).x;
            float rightEdge = _mainCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f)).x;
            float viewWidth = rightEdge - leftEdge;
            float stageWidth = _stageConfig.GridWidth * _stageConfig.RowHeight;
            return leftEdge + (viewWidth - stageWidth) * 0.5f + _stageConfig.RowHeight * 0.5f;
        }

        /// <summary>
        /// 모든 레벨의 TileConfigs 를 취합하여 ETileType → MITileConfig 딕셔너리를 빌드합니다.
        /// 같은 TileType 이 여러 레벨에 있으면 첫 번째 발견 기준으로 등록합니다.
        /// </summary>
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
    }
}
