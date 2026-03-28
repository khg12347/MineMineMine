using UnityEngine;

namespace MI.Domain.Stage
{
    /// <summary>
    /// 곡괭이의 월드 Y 좌표를 행 인덱스로 변환하고,
    /// 현재 깊이 및 최대 도달 깊이를 추적하는 순수 C# 클래스.
    /// </summary>
    public class MIDepthTracker
    {
        private readonly float _tileSize;

        /// <summary>현재 곡괭이 위치의 행 인덱스</summary>
        public int CurrentRow { get; private set; }

        /// <summary>게임 시작 이후 도달한 최대 행 인덱스</summary>
        public int MaxDepthRow { get; private set; }

        /// <param name="tileSize">타일 1칸의 월드 단위 높이</param>
        public MIDepthTracker(float tileSize)
        {
            _tileSize = tileSize;
        }

        // ── 업데이트 ────────────────────────────────────────────────────

        /// <summary>
        /// 매 프레임 곡괭이의 월드 Y 좌표를 받아 깊이를 갱신합니다.
        /// </summary>
        public void Update(float pickaxeWorldY)
        {
            CurrentRow = WorldYToRow(pickaxeWorldY);
            if (CurrentRow > MaxDepthRow)
                MaxDepthRow = CurrentRow;
        }

        // ── 유틸 ────────────────────────────────────────────────────────

        /// <summary>월드 Y 좌표 → 행 인덱스 변환 (Y 가 음수 방향으로 내려갑니다)</summary>
        public int WorldYToRow(float worldY)
        {
            return Mathf.FloorToInt(-worldY / _tileSize);
        }
    }
}
