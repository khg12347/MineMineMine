using UnityEngine;
namespace MI.Core
{
    
    // ── Status 리스너 계약 ───────────────────────────────────────────────

    /// <summary>
    /// EXP / 레벨 변화를 수신하는 리스너 계약.
    /// UI나 사운드 등 외부 시스템이 이 인터페이스를 구현하고
    /// <see cref="MI.Core.MIStatusManager"/>에 등록하면 변경 알림을 받는다.
    ///
    /// 등록 방법:
    ///   MIStatusManager.Instance.RegisterListener(this);
    /// 해제 방법:
    ///   MIStatusManager.Instance.UnregisterListener(this);
    /// </summary>
    public interface IMIStatusListener
    {
        /// <summary>
        /// EXP 수치가 변경될 때 호출 (레벨업 직후 포함).
        /// </summary>
        /// <param name="currentExp">현재 레벨 내 누적 EXP</param>
        /// <param name="requiredExp">현재 레벨에서 다음 레벨까지 필요한 EXP</param>
        /// <param name="ratio">진행률 [0, 1]</param>
        void OnExpChanged(int currentExp, int requiredExp, float ratio);

        /// <summary>
        /// 레벨업이 발생할 때마다 호출. 한 번에 여러 레벨 오를 경우 레벨마다 개별 호출.
        /// </summary>
        /// <param name="newLevel">오른 후의 새 레벨</param>
        void OnLevelUp(int newLevel);

        /// <summary>
        /// 깊이(depth)가 변경될 때 호출.
        /// </summary>
        /// <param name="newDepth">새로운 깊이 값</param>
        void OnDepthUpdated(int newDepth);
    }
}