using MI.Domain.Status;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace MI.Data.Config
{
    /// <summary>
    /// 레벨 진급에 필요한 EXP 테이블을 정의하는 ScriptableObject.
    /// 테이블에 정의된 마지막 레벨 이후에는 <see cref="_overflowMultiplier"/>를
    /// 누적 곱해 무한 레벨업을 지원한다.
    /// </summary>
    [CreateAssetMenu(fileName = "StatusConfig", menuName = "MI/Config/Status")]
    public class MIStatusConfig : SerializedScriptableObject
    {
        [Title("레벨 EXP 테이블")]
        [InfoBox("레벨 번호는 반드시 오름차순으로 정의하세요. 빈 항목이 있으면 해당 레벨을 건너뜁니다.")]
        [OdinSerialize]
        private FLevelEntry[] _levelTable = new FLevelEntry[]
        {
            new FLevelEntry { Level = 1, RequiredExp = 100 },
            new FLevelEntry { Level = 2, RequiredExp = 200 },
            new FLevelEntry { Level = 3, RequiredExp = 350 },
            new FLevelEntry { Level = 4, RequiredExp = 550 },
            new FLevelEntry { Level = 5, RequiredExp = 800 },
        };

        [Title("무한 레벨업 설정")]
        [InfoBox("정의된 마지막 레벨 초과 시, 마지막 RequiredExp에 이 배율을 단계마다 누적 적용합니다.")]
        [PropertyRange(1.0f, 5.0f)]
        [SerializeField] private float _overflowMultiplier = 1.5f;

        // ── 공개 API ─────────────────────────────────────────────────

        /// <summary>
        /// 특정 레벨에서 다음 레벨로 진급하는 데 필요한 EXP를 반환.
        /// 테이블 범위 내이면 테이블 값을, 초과이면 배율 계산 값을 반환.
        /// </summary>
        public int GetRequiredExp(int level)
        {
            if (_levelTable == null || _levelTable.Length == 0)
            {
                Debug.LogWarning("[MIStatusConfig] 레벨 테이블이 비어 있습니다.");
                return 100;
            }

            // 테이블 내에 해당 레벨이 있으면 직접 반환
            foreach (var entry in _levelTable)
            {
                if (entry.Level == level)
                    return entry.RequiredExp;
            }

            // 테이블 범위 초과: 마지막 항목 EXP × 배율 ^ 초과 단계
            var last         = _levelTable[_levelTable.Length - 1];
            int overflowStep = level - last.Level; // 마지막 레벨 기준 몇 단계 초과
            float result     = last.RequiredExp;
            for (int i = 0; i < overflowStep; i++)
                result *= _overflowMultiplier;

            return Mathf.RoundToInt(result);
        }

        /// <summary>테이블에 정의된 최대 레벨 번호.</summary>
        public int MaxDefinedLevel
        {
            get
            {
                if (_levelTable == null || _levelTable.Length == 0) return 0;
                return _levelTable[_levelTable.Length - 1].Level;
            }
        }
    }
}
