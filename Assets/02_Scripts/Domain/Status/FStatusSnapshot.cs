namespace MI.Domain.Status
{
    /// <summary>
    /// 현재 EXP / 레벨 상태의 불변 스냅샷.
    /// UI 갱신 및 단위 테스트에서 상태를 순수하게 읽는 용도로 사용.
    /// </summary>
    public readonly struct FStatusSnapshot
    {
        public readonly int  CurrentLevel;
        public readonly int  CurrentExp;
        public readonly int  RequiredExp;
        public readonly long TotalExp;

        /// <summary>현재 레벨 내 EXP 진행률 [0, 1]. RequiredExp == 0 이면 1 반환.</summary>
        public float ExpRatio => RequiredExp > 0 ? (float)CurrentExp / RequiredExp : 1f;

        public FStatusSnapshot(int currentLevel, int currentExp, int requiredExp, long totalExp)
        {
            CurrentLevel = currentLevel;
            CurrentExp   = currentExp;
            RequiredExp  = requiredExp;
            TotalExp     = totalExp;
        }
    }
}
