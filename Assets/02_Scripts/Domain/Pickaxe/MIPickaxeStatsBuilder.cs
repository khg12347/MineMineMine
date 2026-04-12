using MI.Data.Config;

using UnityEngine;

namespace MI.Domain.Pickaxe
{
    /// <summary>
    /// FPickaxeInstance의 BaseStats에 강화/잠재능력 배율을 단계적으로 적용하여
    /// FResolvedPickaxeStats를 산출하는 Builder.
    /// </summary>
    /// <example>
    /// var resolved = new MIPickaxeStatsBuilder()
    ///     .SetBase(instance)
    ///     .ApplyEnhance(enhanceConfig)
    ///     .Build();
    /// </example>
    public class MIPickaxeStatsBuilder
    {
        #region 중간 변수 (데미지 계열)

        // 데미지 계열 — 배율 적용 대상
        private int _headDamage;
        private int _handleDamage;
        private float _criticalChance;
        private float _criticalDamageMultiplier;

        #endregion 중간 변수 (데미지 계열)

        #region 원본 참조

        // 물리 계열은 BaseStats 원본값을 그대로 사용
        private FPickaxeStats _baseStats;
        private EPickaxeType _pickaxeType;
        private int _level;

        #endregion 원본 참조

        /// <summary>
        /// FPickaxeInstance에서 BaseStats의 데미지 계열 값을 중간 변수에 복사한다.
        /// </summary>
        /// <param name="instance">강화를 적용할 곡괭이 인스턴스</param>
        public MIPickaxeStatsBuilder SetBase(FPickaxeInstance instance)
        {
            _baseStats = instance.BaseStats;
            _pickaxeType = instance.PickaxeType;
            _level = instance.Level;

            // 데미지 계열 복사
            _headDamage = instance.BaseStats.HeadDamage;
            _handleDamage = instance.BaseStats.HandleDamage;
            _criticalChance = instance.BaseStats.CriticalChance;
            _criticalDamageMultiplier = instance.BaseStats.CriticalDamageMultiplier;

            return this;
        }

        /// <summary>
        /// 강화 배율 SO를 기반으로 데미지 계열 중간 변수에 배율을 곱한다.
        /// </summary>
        /// <param name="config">강화 배율 설정 SO</param>
        public MIPickaxeStatsBuilder ApplyEnhance(MIEnhanceConfig config)
        {
            if (config == null)
            {
                Debug.LogWarning("[MIPickaxeStatsBuilder] ApplyEnhance: config가 null입니다. 강화 배율 적용을 건너뜁니다.");
                return this;
            }

            float damageMultiplier = config.GetDamageMultiplier(_pickaxeType, _level);
            float criticalMultiplier = config.GetCriticalMultiplier(_pickaxeType, _level);

            _headDamage = Mathf.RoundToInt(_headDamage * damageMultiplier);
            _handleDamage = Mathf.RoundToInt(_handleDamage * damageMultiplier);
            _criticalChance *= criticalMultiplier;
            _criticalDamageMultiplier *= criticalMultiplier;

            return this;
        }

        // TODO: ApplyLatent(MILatentConfig config) — Phase 3 잠재능력 단계 확장 포인트

        /// <summary>
        /// 데미지 계열은 중간 변수, 물리 계열은 BaseStats 원본값으로 FResolvedPickaxeStats를 생성한다.
        /// </summary>
        public FResolvedPickaxeStats Build()
        {
            return new FResolvedPickaxeStats(
                headDamage: _headDamage,
                handleDamage: _handleDamage,
                criticalChance: _criticalChance,
                criticalDamageMultiplier: _criticalDamageMultiplier,
                gravityScale: _baseStats.GravityScale,
                bounciness: _baseStats.Bounciness,
                friction: _baseStats.Friction,
                bounceOnBreak: _baseStats.BounceOnBreak,
                breakBounceForce: _baseStats.BreakBounceForce,
                spawnOffsetY: _baseStats.SpawnOffsetY,
                wallBounciness: _baseStats.WallBounciness
            );
        }
    }
}
