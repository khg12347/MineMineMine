using MI.Data.Config;
using UnityEngine;

namespace MI.Domain.Stage
{
    // 좌우 물리 벽 생성 및 카메라 Y 위치에 맞춰 위치 갱신
    public class MIWallSpawner
    {
        private GameObject _leftWall;
        private GameObject _rightWall;

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

            float leftEdge  = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f)).x - 0.5f;
            float rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f)).x + 0.5f;

            // 뷰포트 엣지에서 tileSize 만큼 안쪽에 벽 배치
            _leftWall  = CreateWall("LeftWall",  leftEdge  + tileSize, tileSize, 200f, wallMaterial);
            _rightWall = CreateWall("RightWall", rightEdge - tileSize, tileSize, 200f, wallMaterial);
        }

        #region Update

        public void UpdateWalls(float cameraY)
        {
            if (_leftWall != null)
                _leftWall.transform.position  = new Vector3(_leftWall.transform.position.x,  cameraY, 0f);
            if (_rightWall != null)
                _rightWall.transform.position = new Vector3(_rightWall.transform.position.x, cameraY, 0f);
        }

        #endregion Update

        #region Helper

        private static GameObject CreateWall(string wallName, float posX, float width, float height, PhysicsMaterial2D material)
        {
            var wall = new GameObject(wallName);
            wall.transform.position = new Vector3(posX, 0f, 0f);

            var col           = wall.AddComponent<BoxCollider2D>();
            col.size          = new Vector2(width, height);
            col.sharedMaterial = material;

            return wall;
        }

        #endregion Helper
    }
}
