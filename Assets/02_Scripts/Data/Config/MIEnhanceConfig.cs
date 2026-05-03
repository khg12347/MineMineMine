using System.Collections.Generic;

using MI.Data.Pickaxe;
using MI.Domain.Pickaxe;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UnityEngine;

namespace MI.Data.Config
{
    /// <summary>
    /// 곡괭이 종류별 강화 배율 커브를 관리하는 ScriptableObject.
    /// MIPickaxeStatsBuilder에서 강화 배율 조회에 사용한다.
    /// </summary>
    [CreateAssetMenu(fileName = "EnhanceConfig", menuName = "MI/Config/EnhanceConfig")]
    public class MIEnhanceConfig : SerializedScriptableObject
    {
        [Title("강화 배율 테이블")]
        [InfoBox("곡괭이 타입별 강화 배율 커브를 설정한다. 등록되지 않은 타입은 배율 1.0(무강화)으로 처리.")]
        [DictionaryDrawerSettings(KeyLabel = "곡괭이 타입", ValueLabel = "강화 커브 설정")]
        [OdinSerialize]
        private Dictionary<EPickaxeType, FEnhanceEntry> _entries = new();

        /// <summary>
        /// 해당 곡괭이 타입과 레벨의 데미지 배율을 반환한다.
        /// </summary>
        /// <param name="type">곡괭이 타입</param>
        /// <param name="level">강화 레벨 (1~100)</param>
        /// <returns>데미지 배율. 미등록 타입이면 1f.</returns>
        public float GetDamageMultiplier(EPickaxeType type, int level)
        {
            if (!_entries.TryGetValue(type, out FEnhanceEntry entry))
                return 1f;
            return entry.EvaluateDamage(level);
        }

        /// <summary>
        /// 해당 곡괭이 타입과 레벨의 크리티컬 배율을 반환한다.
        /// </summary>
        /// <param name="type">곡괭이 타입</param>
        /// <param name="level">강화 레벨 (1~100)</param>
        /// <returns>크리티컬 배율. 미등록 타입이면 1f.</returns>
        public float GetCriticalMultiplier(EPickaxeType type, int level)
        {
            if (!_entries.TryGetValue(type, out FEnhanceEntry entry))
                return 1f;
            return entry.EvaluateCritical(level);
        }

#if UNITY_EDITOR
        [Title("디버그")]
        [Button("밸런스 테이블 출력")]
        private void PrintBalanceTable()
        {
            int[] checkLevels = { 1, 25, 50, 61, 75, 100 };
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== 강화 밸런스 테이블 ===");
            sb.AppendLine($"{"타입",-20} {"Lv1",8} {"Lv25",8} {"Lv50",8} {"Lv61",8} {"Lv75",8} {"Lv100",8}");
            sb.AppendLine(new string('-', 68));

            foreach (EPickaxeType type in System.Enum.GetValues(typeof(EPickaxeType)))
            {
                if (type == EPickaxeType.None) continue;
                sb.Append($"{type,-20}");
                foreach (int level in checkLevels)
                    sb.Append($" {GetDamageMultiplier(type, level),8:F3}");
                sb.AppendLine();
            }

            Debug.Log(sb.ToString());
        }
#endif
    }
}
