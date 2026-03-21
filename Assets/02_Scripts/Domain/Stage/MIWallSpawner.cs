using MI.Data.Config;
using UnityEngine;

namespace MI.Domain.Stage
{
    /// <summary>
    /// 스테이지 좌우 물리 벽을 생성하고, 카메라 Y 위치에 맞춰 벽 위치를 업데이트합니다.
    ///
    /// 현재 구현:
    ///   - BoxCollider2D 를 코드로 생성 (프로토타입)
    ///   - 벽 프리팹 준비 이후 프리팹 기반 스폰으로 교체 가능
    ///
    /// [사용자 작업] 향후 벽 프리팹이 준비되면 아래 스펙으로 제작 후 교체:
    ///   - 스프라이트: 16×160 px (Pixels Per Unit 일치)
    ///   - BoxCollider2D 포함
    ///   - PhysicsMaterial2D 는 코드에서 생성하여 설정하거나 프리팹에 포함
    /// </summary>
    public class MIWallSpawner
    {
        private GameObject _leftWall;
        private GameObject _rightWall;

        /// <summary>
        /// 좌우 물리 벽을 생성합니다.
        /// </summary>
        /// <param name="pickaxeConfig">벽 바운스 계수를 가져올 곡괭이 설정</param>
        /// <param name="mainCamera">뷰포트 계산에 사용할 메인 카메라</param>
        /// <param name="tileSize">타일 1칸 크기 (벽 두께로 사용)</param>
        /// <param name="stageStartX">타일 영역 시작 X (미사용, 확장 대비)</param>
        /// <param name="stageWidth">가로 타일 수 (미사용, 확장 대비)</param>
        public MIWallSpawner(
            MIPickaxeConfig pickaxeConfig, Camera mainCamera,
            float tileSize, float stageStartX, int stageWidth)
        {
            var stats = pickaxeConfig.CreateStats();
            var wallMaterial = new PhysicsMaterial2D("WallBounce")
            {
                bounciness = stats.WallBounciness,
                friction   = 0f
            };

            float leftEdge  = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f)).x;
            float rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f)).x;

            // 뷰포트 엣지에서 tileSize 만큼 안쪽에 벽 배치
            _leftWall  = CreateWall("LeftWall",  leftEdge  + tileSize, tileSize, 200f, wallMaterial);
            _rightWall = CreateWall("RightWall", rightEdge - tileSize, tileSize, 200f, wallMaterial);
        }

        // ── 업데이트 ────────────────────────────────────────────────────

        /// <summary>카메라 Y 위치에 맞춰 벽을 이동시킵니다.</summary>
        public void UpdateWalls(float cameraY)
        {
            if (_leftWall != null)
                _leftWall.transform.position  = new Vector3(_leftWall.transform.position.x,  cameraY, 0f);
            if (_rightWall != null)
                _rightWall.transform.position = new Vector3(_rightWall.transform.position.x, cameraY, 0f);
        }

        // ── 헬퍼 ────────────────────────────────────────────────────────

        private static GameObject CreateWall(string wallName, float posX, float width, float height, PhysicsMaterial2D material)
        {
            var wall = new GameObject(wallName);
            wall.transform.position = new Vector3(posX, 0f, 0f);

            var col           = wall.AddComponent<BoxCollider2D>();
            col.size          = new Vector2(width, height);
            col.sharedMaterial = material;

            return wall;
        }
    }
}
