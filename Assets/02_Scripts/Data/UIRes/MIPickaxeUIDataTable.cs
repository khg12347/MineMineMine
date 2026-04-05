using MI.Domain.Pickaxe;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace MI.Data.UIRes
{
    [CreateAssetMenu(fileName = "PickaxeIconDataTable", menuName = "MI/Data/UIRes/PickaxeIconDataTable")]
    public class MIPickaxeUIDataTable : SerializedScriptableObject
    {
        [DictionaryDrawerSettings(KeyLabel = "타입", ValueLabel = "아이콘")]
        [SerializeField] private Dictionary<EPickaxeType, PickaxeDataEntry> _dataTable = new();

        [SerializeField] private Sprite _blank; // 타입이 없을 때 반환하는 빈 스프라이트

        public Sprite GetPickaxeIcon(EPickaxeType itemType)
        {
            if (_dataTable.TryGetValue(itemType, out PickaxeDataEntry entry))
            {
                return entry.Icon;
            }

            return _blank;
        }

        public string GetPickaxeName(EPickaxeType itemType)
        {
            if (_dataTable.TryGetValue(itemType, out PickaxeDataEntry entry))
            {
                return entry.Name;
            }
            return "???";
        }

        private class PickaxeDataEntry
        {
            public string Name;
            public Sprite Icon;
        }
    }
}
