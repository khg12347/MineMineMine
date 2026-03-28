namespace MI.Domain.Pickaxe
{
    public struct FPickaxeStats
    {
        public int HeadDamage;          // 머리 공격력 (높음)
        public int HandleDamage;        // 자루 공격력 (낮음)
        public float GravityScale;      // 중력 배율
        public float Bounciness;        // 기본 탄력 (0.0 ~ 1.0)
        public float Friction;          // 마찰력 (0.0 권장)
        public bool BounceOnBreak;      // 블록 파괴 시 바운스 여부
        public float BreakBounceForce;  // 파괴 시 바운스 힘
        public float SpawnOffsetY;      // 화면 상단 바깥 오프셋
        public float WallBounciness;    // 벽 반사 탄력 (0.0 ~ 1.0)
    }
}
