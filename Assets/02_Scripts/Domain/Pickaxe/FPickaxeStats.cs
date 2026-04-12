namespace MI.Domain.Pickaxe
{
    public struct FPickaxeStats
    {
        //데미지 계열 (강화/잠재능력 시스템에 영향)
        public int HeadDamage;          // 머리 공격력 (높음)
        public int HandleDamage;        // 자루 공격력 (낮음)
        public float CriticalChance;       // 치명타 확률 (0.0 ~ 1.0)
        public float CriticalDamageMultiplier;   // 치명타 데미지 배율 (1.0 이상)

        //물리 계열 (물리 엔진에 영향)
        public float GravityScale;      // 중력 배율
        public float Bounciness;        // 기본 탄력 (0.0 ~ 1.0)
        public float Friction;          // 마찰력 (0.0 권장)
        public bool BounceOnBreak;      // 블록 파괴 시 바운스 여부
        public float BreakBounceForce;  // 파괴 시 바운스 힘
        public float SpawnOffsetY;      // 화면 상단 바깥 오프셋
        public float WallBounciness;    // 벽 반사 탄력 (0.0 ~ 1.0)
    }
}
