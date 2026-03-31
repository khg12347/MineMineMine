using MI.Domain.Status;
using MI.Utility;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace MI.Data.Config
{
    // 레벨 진급 EXP 테이블 ScriptableObject. 마지막 레벨 이후는 overflowMultiplier 배율로 무한 레벨업.
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

        #region Public API

        // 특정 레벨의 진급 필요 EXP 반환. 테이블 초과 시 배율 계산.
        public int GetRequiredExp(int level)
        {
            if (_levelTable == null || _levelTable.Length == 0)
            {
                MILog.LogWarning("[MIStatusConfig] 레벨 테이블이 비어 있습니다.");
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

        // 테이블에 정의된 최대 레벨
        public int MaxDefinedLevel
        {
            get
            {
                if (_levelTable == null || _levelTable.Length == 0) return 0;
                return _levelTable[_levelTable.Length - 1].Level;
            }
        }

        #endregion Public API
    }
}
