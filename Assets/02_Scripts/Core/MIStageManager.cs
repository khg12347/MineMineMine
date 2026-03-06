using System.Collections.Generic;
using MI.Data.Config;
using MI.Presentation.Pickaxe;
using MI.Presentation.Tile;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MI.Core
{
    /// <summary>
    /// 무한 하강 스테이지를 관리합니다.
    /// 청크 기반으로 타일을 동적 생성/제거하며, 좌우 벽과 카메라 추적을 처리합니다.
    /// MIStageInitializer를 대체합니다.
    /// </summary>
    public class MIStageManager : MonoBehaviour
    {
        [Title("타일 설정")]
        [SerializeField] private MITileConfig[] _tileConfigs;
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private Transform _tileParent;

        [Title("스테이지 크기")]
        [PropertyRange(1, 20)]
        [SerializeField] private int _stageWidth = 8;
        [PropertyRange(0.1f, 2f)]
        [SerializeField] private float _tileSize = 0.5f;

        [Title("청크 설정")]
        [InfoBox("곡괭이 아래로 미리 생성해 둘 행 수")]
        [PropertyRange(4, 32)]
        [SerializeField] private int _spawnAheadRows = 16;

        [InfoBox("곡괭이 위로 이 행 수를 초과하면 타일 제거 (메모리 절약)")]
        [PropertyRange(4, 32)]
        [SerializeField] private int _cullRowsAbove = 8;

        [InfoBox("깊이 N행마다 다음 티어 타일로 전환")]
        [PropertyRange(5, 50)]
        [SerializeField] private int _tierDepthStep = 20;

        [Title("카메라")]
        [SerializeField] private Camera _mainCamera;
        [PropertyRange(1f, 10f)]
        [SerializeField] private float _cameraFollowSpeed = 3f;

        [Title("곡괭이")]
        [SerializeField] private MIPickaxeController _pickaxe;
        [SerializeField] private MIPickaxeConfig _pickaxeConfig;

        // 런타임 상태
        private int _generatedRows;
        private float _stageStartX;        // 타일 배치 시작 X (카메라 왼쪽 엣지 기준)
        private float _cameraTargetY;
        private GameObject _leftWall;
        private GameObject _rightWall;

        // 행 인덱스 → 해당 행의 타일 오브젝트 목록
        private readonly Dictionary<int, List<GameObject>> _tilesByRow = new();

        private void Start()
        {
            CalculateStageStartX();
            CreateWalls();

            // 초기 타일 생성
            GenerateRowsUpTo(_spawnAheadRows);

            _cameraTargetY = _mainCamera.transform.position.y;
            _pickaxe.SpawnAtOffScreen(_mainCamera);
        }

        private void Update()
        {
            UpdateCameraFollow();
            UpdateWallPositions();
            CheckChunkGeneration();
            CullDistantTiles();
        }

        // ── 초기화 ────────────────────────────────────────────────────

        private void CalculateStageStartX()
        {
            // 카메라 뷰포트 왼쪽 엣지 기준으로 타일 배치 시작 X 계산
            float leftEdge  = _mainCamera.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f)).x;
            float rightEdge = _mainCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f)).x;
            float viewWidth = rightEdge - leftEdge;

            // 타일 전체 너비가 뷰포트를 넘지 않으면 중앙 정렬, 넘으면 왼쪽 엣지에 맞춤
            float stageWidth = _stageWidth * _tileSize;
            _stageStartX = leftEdge + (viewWidth - stageWidth) * 0.5f + _tileSize * 0.5f;
        }

        private void CreateWalls()
        {
            var stats = _pickaxeConfig.CreateStats();
            var wallMaterial = new PhysicsMaterial2D("WallBounce")
            {
                bounciness = stats.WallBounciness,
                friction = 0f
            };

            float leftEdge  = _mainCamera.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f)).x;
            float rightEdge = _mainCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f)).x;
            const float wallThickness = 1f;
            const float wallHeight    = 200f; // 카메라 이동 범위를 커버할 높이

            _leftWall  = CreateWall("LeftWall",  leftEdge  - wallThickness * 0.5f, wallThickness, wallHeight, wallMaterial);
            _rightWall = CreateWall("RightWall", rightEdge + wallThickness * 0.5f, wallThickness, wallHeight, wallMaterial);
        }

        private GameObject CreateWall(string wallName, float posX, float width, float height, PhysicsMaterial2D material)
        {
            var wall = new GameObject(wallName);
            wall.transform.SetParent(transform);
            wall.transform.position = new Vector3(posX, 0f, 0f);

            var col = wall.AddComponent<BoxCollider2D>();
            col.size           = new Vector2(width, height);
            col.sharedMaterial = material;

            return wall;
        }

        // ── 청크 생성 / 제거 ──────────────────────────────────────────

        private void CheckChunkGeneration()
        {
            int pickaxeRow = WorldYToRow(_pickaxe.transform.position.y);
            int neededRows = pickaxeRow + _spawnAheadRows;
            if (neededRows > _generatedRows)
                GenerateRowsUpTo(neededRows);
        }

        private void GenerateRowsUpTo(int targetRow)
        {
            for (int row = _generatedRows; row < targetRow; row++)
            {
                var rowTiles = new List<GameObject>(_stageWidth);
                for (int x = 0; x < _stageWidth; x++)
                {
                    var config = SelectTileConfig(row);
                    var pos    = new Vector3(_stageStartX + x * _tileSize, -row * _tileSize, 0f);
                    var obj    = Instantiate(_tilePrefab, pos, Quaternion.identity, _tileParent);
                    obj.GetComponent<MITileView>().Initialize(config.CreateTileData());
                    rowTiles.Add(obj);
                }
                _tilesByRow[row] = rowTiles;
            }
            _generatedRows = Mathf.Max(_generatedRows, targetRow);
        }

        private void CullDistantTiles()
        {
            int pickaxeRow = WorldYToRow(_pickaxe.transform.position.y);
            int cullBefore = pickaxeRow - _cullRowsAbove;

            var toRemove = new List<int>();
            foreach (var kv in _tilesByRow)
            {
                if (kv.Key >= cullBefore) continue;
                foreach (var tile in kv.Value)
                    if (tile != null) Destroy(tile);
                toRemove.Add(kv.Key);
            }
            foreach (int row in toRemove)
                _tilesByRow.Remove(row);
        }

        // ── 카메라 / 벽 추적 ─────────────────────────────────────────

        private void UpdateCameraFollow()
        {
            float pickaxeY = _pickaxe.transform.position.y;
            // 곡괭이가 더 아래로 내려갈 때만 카메라 타깃을 업데이트 (위로 튀어도 카메라는 내려간 위치 유지)
            _cameraTargetY = Mathf.Min(_cameraTargetY, pickaxeY);

            var camPos = _mainCamera.transform.position;
            camPos.y = Mathf.Lerp(camPos.y, _cameraTargetY, Time.deltaTime * _cameraFollowSpeed);
            _mainCamera.transform.position = camPos;
        }

        private void UpdateWallPositions()
        {
            // 벽을 카메라 Y에 맞춰 이동 (항상 화면을 커버하도록)
            float camY = _mainCamera.transform.position.y;
            if (_leftWall  != null) _leftWall.transform.position  = new Vector3(_leftWall.transform.position.x,  camY, 0f);
            if (_rightWall != null) _rightWall.transform.position = new Vector3(_rightWall.transform.position.x, camY, 0f);
        }

        // ── 헬퍼 ─────────────────────────────────────────────────────

        /// <summary>월드 Y 좌표 → 행 인덱스 변환</summary>
        private int WorldYToRow(float worldY) => Mathf.FloorToInt(-worldY / _tileSize);

        /// <summary>깊이에 따라 점진적으로 강한 타일 Config 반환</summary>
        private MITileConfig SelectTileConfig(int depth)
        {
            int tier = depth / _tierDepthStep;
            int idx  = Mathf.Clamp(tier, 0, _tileConfigs.Length - 1);
            return _tileConfigs[idx];
        }
    }
}
