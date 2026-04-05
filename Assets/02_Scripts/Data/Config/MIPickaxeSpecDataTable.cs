using System.Collections.Generic;

using MI.Domain.Pickaxe;

using Sirenix.OdinInspector;

using UnityEngine;

namespace MI.Data.Config
{
    /// <summary>
    /// 곡괭이 타입별 베이스 스펙 데이터 테이블.
    /// 장착한 곡괭이 스폰 시 기본 스펙을 조회한다.
    /// </summary>
    [CreateAssetMenu(
        fileName = "PickaxeSpecDataTable",
        menuName = "MI/Config/PickaxeSpecDataTable")]
    public class MIPickaxeSpecDataTable : SerializedScriptableObject
    {
        [DictionaryDrawerSettings(KeyLabel = "곡괭이 타입", ValueLabel = "스펙 설정")]
        [SerializeField] private Dictionary<EPickaxeType, MIPickaxeConfig> _specTable = new();

        /// <summary>
        /// 해당 곡괭이 타입의 베이스 스펙 Config를 반환한다. 없으면 null.
        /// </summary>
        public MIPickaxeConfig GetConfig(EPickaxeType type)
        {
            return _specTable.TryGetValue(type, out MIPickaxeConfig config) ? config : null;
        }

        /// <summary>
        /// 해당 곡괭이 타입의 FPickaxeStats를 생성하여 반환한다. 없으면 null.
        /// </summary>
        public FPickaxeStats? GetStats(EPickaxeType type)
        {
            MIPickaxeConfig config = GetConfig(type);
            return config != null ? config.CreateStats() : null;
        }
    }
}
