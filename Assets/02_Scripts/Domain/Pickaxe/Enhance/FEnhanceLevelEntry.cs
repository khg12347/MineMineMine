using System;

using MI.Domain.Pickaxe.Craft;

using Sirenix.OdinInspector;

using UnityEngine;

namespace MI.Domain.Pickaxe.Enhance
{
    /// <summary>
    /// 특정 강화 레벨의 비용 + 확률 정의.
    /// MIEnhanceCostConfig에서 배열로 관리한다.
    /// </summary>
    [Serializable]
    public struct FEnhanceLevelEntry
    {
        [Title("대상 레벨")]
        [InfoBox("현재 레벨(강화 전). 예: Lv1 → Lv2 강화 시 FromLevel = 1")]
        // 이 엔트리가 적용되는 강화 전 레벨
        public int FromLevel;

        [Title("성공 확률")]
        [PropertyRange(0f, 1f)]
        // 강화 성공 확률 (0.0 ~ 1.0). 예: 0.9 = 90%
        public float SuccessRate;

        [Title("대성공 확률")]
        [InfoBox("성공 판정 이후 추가로 굴리는 대성공 확률. 0이면 대성공 없음.")]
        [PropertyRange(0f, 1f)]
        // 대성공 확률 (0.0 ~ 1.0). 성공 시에만 판정. 대성공 시 재화/재료 소모 없이 자동 재도전.
        public float PerfectSuccessRate;

        [Title("재료 비용")]
        // 필요 강화석 목록 (EItemType 300번대)
        public FMaterialCost[] Materials;

        [Title("재화 비용")]
        // 필요 재화 목록. 비어있으면 재화 불필요
        public FCurrencyCost[] Currencies;
    }
}
