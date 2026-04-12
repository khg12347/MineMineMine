namespace MI.Domain.Pickaxe
{
    /// <summary>
    /// 강화(Enhance) 배율까지 적용된 곡괭이 최종 스탯.
    /// MIPickaxeStatsBuilder.Build()를 통해서만 생성 가능.
    /// </summary>
    public readonly struct FResolvedPickaxeStats
    {
        #region 데미지 계열 (강화 영향 O)

        /// <summary>머리 공격력 (강화 배율 적용)</summary>
        public readonly int HeadDamage;

        /// <summary>자루 공격력 (강화 배율 적용)</summary>
        public readonly int HandleDamage;

        /// <summary>치명타 확률 0.0~1.0 (강화 배율 적용)</summary>
        public readonly float CriticalChance;

        /// <summary>치명타 데미지 배율 1.0 이상 (강화 배율 적용)</summary>
        public readonly float CriticalDamageMultiplier;

        #endregion 데미지 계열 (강화 영향 O)

        #region 물리 계열 (강화 영향 X)

        /// <summary>중력 배율</summary>
        public readonly float GravityScale;

        /// <summary>기본 탄력 (0.0 ~ 1.0)</summary>
        public readonly float Bounciness;

        /// <summary>마찰력 (0.0 권장)</summary>
        public readonly float Friction;

        /// <summary>블록 파괴 시 바운스 여부</summary>
        public readonly bool BounceOnBreak;

        /// <summary>파괴 시 바운스 힘</summary>
        public readonly float BreakBounceForce;

        /// <summary>화면 상단 바깥 오프셋</summary>
        public readonly float SpawnOffsetY;

        /// <summary>벽 반사 탄력 (0.0 ~ 1.0)</summary>
        public readonly float WallBounciness;

        #endregion 물리 계열 (강화 영향 X)

        /// <summary>
        /// Builder에서만 호출. 외부 직접 생성 불가.
        /// </summary>
        internal FResolvedPickaxeStats(
            int headDamage,
            int handleDamage,
            float criticalChance,
            float criticalDamageMultiplier,
            float gravityScale,
            float bounciness,
            float friction,
            bool bounceOnBreak,
            float breakBounceForce,
            float spawnOffsetY,
            float wallBounciness)
        {
            HeadDamage = headDamage;
            HandleDamage = handleDamage;
            CriticalChance = criticalChance;
            CriticalDamageMultiplier = criticalDamageMultiplier;
            GravityScale = gravityScale;
            Bounciness = bounciness;
            Friction = friction;
            BounceOnBreak = bounceOnBreak;
            BreakBounceForce = breakBounceForce;
            SpawnOffsetY = spawnOffsetY;
            WallBounciness = wallBounciness;
        }
    }
}
