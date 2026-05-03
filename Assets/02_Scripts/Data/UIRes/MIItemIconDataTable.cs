using Sirenix.OdinInspector;
using System.Collections.Generic;
using MI.Data.User.Inventory;
using UnityEngine;

namespace MI.Data.UIRes
{
    [CreateAssetMenu(fileName = "ItemIconDataTable", menuName = "MI/Data/UIRes/ItemIconDataTable")]
    public class MIItemIconDataTable : SerializedScriptableObject
    {
        [DictionaryDrawerSettings(KeyLabel = "타입", ValueLabel = "아이콘")]
        [SerializeField] private Dictionary<EItemType, Sprite> _dataTable = new ();

        [SerializeField] private Sprite _blank; // 타입이 없을 때 반환하는 디버그용 빈 스프라이트
        public Sprite GetItemIcon(EItemType itemType)
        {
            if (_dataTable.TryGetValue(itemType, out Sprite icon))
            {
                return icon;
            }

            return _blank;
        }
    }
}
