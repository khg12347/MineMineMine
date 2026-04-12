using System;
using MI.Data.Config;

namespace MI.Domain.Pickaxe
{
    /// <summary>
    /// 제작된 곡괭이 1개의 런타임 상태.
    /// 강화 시스템은 Level / EnhanceCount 필드를 기반으로 동작한다.
    /// </summary>
    [Serializable]
    public struct FPickaxeInstance
    {
        // 곡괭이 종류
        public EPickaxeType PickaxeType;

        /// <summary>현재 레벨. 제작 시 1, 강화 시 증가.</summary>
        public int Level;

        /// <summary>누적 강화 횟수. 강화 시스템에서 상한선 계산에 사용.</summary>
        public int EnhanceCount;

        /// <summary>
        /// 기본 스탯. 강화 시스템은 이 값에 레벨 배율을 적용하여 최종 스탯 산출.
        /// </summary>
        public FPickaxeStats BaseStats;

        /// <summary>
        /// 강화 배율이 적용된 최종 스탯. ResolveStats()로 갱신한다.
        /// </summary>
        public FResolvedPickaxeStats ResolvedStats;

        /// <summary>
        /// 제작 완료 시 초기 인스턴스를 생성한다.
        /// </summary>
        /// <param name="type">곡괭이 종류</param>
        /// <param name="stats">기본 스탯</param>
        /// <param name="enhanceConfig">강화 배율 설정</param>
        public static FPickaxeInstance Create(EPickaxeType type, FPickaxeStats stats, MIEnhanceConfig enhanceConfig)
        {
            var instance = new FPickaxeInstance
            {
                PickaxeType = type,
                Level = 1,
                EnhanceCount = 0,
                BaseStats = stats,
            };
            instance.ResolveStats(enhanceConfig);
            return instance;
        }

        /// <summary>
        /// BaseStats + Level + EnhanceConfig를 기반으로 ResolvedStats를 재계산한다.
        /// 강화 후 반드시 호출해야 한다.
        /// </summary>
        /// <param name="enhanceConfig">강화 배율 설정</param>
        public void ResolveStats(MIEnhanceConfig enhanceConfig)
        {
            ResolvedStats = new MIPickaxeStatsBuilder()
                .SetBase(this)
                .ApplyEnhance(enhanceConfig)
                .Build();
        }

        /// <summary>
        /// BaseStats만으로 임시 인스턴스를 생성한다. (강화 미적용, Builder용)
        /// Inspector Config 기반 초기화에서 사용.
        /// </summary>
        internal static FPickaxeInstance Create(FPickaxeStats stats)
        {
            return new FPickaxeInstance
            {
                PickaxeType = EPickaxeType.None,
                Level = 1,
                EnhanceCount = 0,
                BaseStats = stats,
            };
        }
    }
}
