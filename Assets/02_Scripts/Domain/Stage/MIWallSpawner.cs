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
            MIPickaxeConfig pickaxeConfig,
            float tileSize, float stageStartX, int stageWidth)
        {
            var stats = pickaxeConfig.CreateStats();
            var wallMaterial = new PhysicsMaterial2D("WallBounce")
            {
                bounciness = stats.WallBounciness,
                friction   = 0f
            };

            // 타일 그리드 바로 옆에 벽 배치
            // 왼쪽: 그리드 왼쪽 엣지(stageStartX - tileSize*0.5) 바깥 한 칸
            // 오른쪽: 그리드 오른쪽 엣지(stageStartX + stageWidth*tileSize - tileSize*0.5) 바깥 한 칸
            float leftX  = stageStartX - tileSize;
            float rightX = stageStartX + stageWidth * tileSize;

            _leftWall  = CreateWall("LeftWall",  leftX,  tileSize, 200f, wallMaterial);
            _rightWall = CreateWall("RightWall", rightX, tileSize, 200f, wallMaterial);
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
