using MI.Data.Pickaxe.Craft;
using MI.Data.Pickaxe.Enhance;
using MI.Data.User.Inventory;
using MI.Data.User.Wallet;

using Sirenix.OdinInspector;

using UnityEngine;

namespace MI.Data.Config
{
    /// <summary>
    /// 강화 레벨별 비용/확률 데이터 테이블.
    /// 기획 확정 전에는 구간별 엔트리만 정의해도 동작하며,
    /// 확정 후 레벨별 세밀한 조정이 가능하다.
    /// </summary>
    [CreateAssetMenu(
        fileName = "EnhanceCostConfig",
        menuName = "MI/Config/EnhanceCostConfig")]
    public class MIEnhanceCostConfig : SerializedScriptableObject
    {
        [Title("최대 강화 레벨")]
        [InfoBox("강화 가능한 최대 레벨.")]
        [PropertyRange(1, 200)]
        [SerializeField] private int _maxLevel = 100;

        [Title("레벨별 강화 비용 테이블")]
        [InfoBox(
            "FromLevel은 강화 전 현재 레벨을 의미한다.\n" +
            "정확히 일치하는 엔트리가 없으면 FromLevel 이하 중 가장 큰 엔트리를 사용한다.\n" +
            "예: 1, 10, 50만 정의 시 Lv25는 FromLevel=10 엔트리 적용.")]
        [TableList]
        [SerializeField] private FEnhanceLevelEntry[] _entries;

        /// <summary>최대 강화 레벨</summary>
        public int MaxLevel => _maxLevel;

        #region Public API

        /// <summary>
        /// 현재 레벨(fromLevel)에 해당하는 강화 비용 엔트리를 반환한다.
        /// fromLevel 이하 중 가장 큰 FromLevel 엔트리를 반환하여 구간 정의만으로도 동작한다.
        /// 일치하는 엔트리가 없으면 null 반환.
        /// </summary>
        /// <param name="fromLevel">현재 레벨 (강화 전)</param>
        /// <returns>해당 레벨의 강화 비용 엔트리. 없으면 null.</returns>
        public FEnhanceLevelEntry? GetEntry(int fromLevel)
        {
            if (_entries == null || _entries.Length == 0) return null;

            FEnhanceLevelEntry? best = null;
            for (int i = 0; i < _entries.Length; i++)
            {
                if (_entries[i].FromLevel > fromLevel) continue;

                if (!best.HasValue || _entries[i].FromLevel > best.Value.FromLevel)
                    best = _entries[i];
            }
            return best;
        }

        #endregion Public API

#if UNITY_EDITOR

        #region Editor

        [Title("에디터 도구")]
        [InfoBox("선형 기본값 자동 생성: Lv1=100%, Lv100=1% 선형 확률, 강화석 = 레벨 개수, 골드 = 레벨 × 100")]
        [Button("선형 기본값 자동 생성")]
        private void GenerateLinearDefaults()
        {
            _entries = new FEnhanceLevelEntry[_maxLevel];
            for (int i = 0; i < _maxLevel; i++)
            {
                int level = i + 1;
                float successRate = Mathf.Clamp01(1f - (float)(level - 1) / (_maxLevel - 1) * 0.99f);

                _entries[i] = new FEnhanceLevelEntry
                {
                    FromLevel   = level,
                    SuccessRate = successRate,
                    Materials = new[]
                    {
                        new FMaterialCost
                        {
                            ItemType = EItemType.EnhanceStone_Low,
                            Amount   = level,
                        },
                    },
                    Currencies = new[]
                    {
                        new FCurrencyCost
                        {
                            CurrencyType = ECurrencyType.Gold,
                            Amount       = level * 100L,
                        },
                    },
                };
            }
        }

        #endregion Editor

#endif
    }
}
