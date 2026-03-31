using UnityEngine;

namespace MI.Domain.Stage
{
    // 곡괭이 월드Y → 행 인덱스 변환, 현재/최대 깊이 추적
    public class MIDepthTracker
    {
        private readonly float _tileSize;

        // 현재 행 인덱스
        public int CurrentRow { get; private set; }

        // 최대 도달 행 인덱스
        public int MaxDepthRow { get; private set; }

        public MIDepthTracker(float tileSize)
        {
            _tileSize = tileSize;
        }

        #region Update

        // 매 프레임 호출: 깊이 갱신
        public void Update(float pickaxeWorldY)
        {
            CurrentRow = WorldYToRow(pickaxeWorldY);
            if (CurrentRow > MaxDepthRow)
                MaxDepthRow = CurrentRow;
        }

        #endregion Update

        #region Utility

        // Y 음수 방향으로 내려가는 좌표계
        public int WorldYToRow(float worldY)
        {
            return Mathf.FloorToInt(-worldY / _tileSize);
        }

        #endregion Utility
    }
}
