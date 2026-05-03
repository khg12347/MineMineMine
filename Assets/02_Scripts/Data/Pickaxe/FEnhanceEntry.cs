using System;

using Sirenix.OdinInspector;

using UnityEngine;

namespace MI.Data.Pickaxe
{
    /// <summary>
    /// 곡괭이 1종의 강화 배율 설정 단위.
    /// AnimationCurve로 레벨(1~100)에 따른 배율을 정의한다.
    /// </summary>
    [Serializable]
    public struct FEnhanceEntry
    {
        [Title("데미지 배율 커브")]
        [InfoBox("X축: 강화 레벨(0=Lv1, 1=Lv100) / Y축: 데미지 배율 (예: 1.5 = 150%)")]
        [SerializeField] private AnimationCurve _damageCurve;

        [Title("크리티컬 배율 커브")]
        [InfoBox("X축: 강화 레벨(0=Lv1, 1=Lv100) / Y축: 크리티컬 관련 배율 (예: 1.2 = 120%)")]
        [SerializeField] private AnimationCurve _criticalCurve;

        /// <summary>
        /// 레벨(1~100)에 해당하는 데미지 배율을 반환한다.
        /// </summary>
        /// <param name="level">강화 레벨 (1~100)</param>
        /// <returns>데미지 배율 (커브 평가값)</returns>
        public float EvaluateDamage(int level)
        {
            float t = (level - 1) / 99f;
            return _damageCurve?.Evaluate(t) ?? 1f;
        }

        /// <summary>
        /// 레벨(1~100)에 해당하는 크리티컬 배율을 반환한다.
        /// </summary>
        /// <param name="level">강화 레벨 (1~100)</param>
        /// <returns>크리티컬 배율 (커브 평가값)</returns>
        public float EvaluateCritical(int level)
        {
            float t = (level - 1) / 99f;
            return _criticalCurve?.Evaluate(t) ?? 1f;
        }
    }
}
