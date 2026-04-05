using MI.Data.UIRes;
using MI.Domain.Pickaxe;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MI.Presentation.UI.Popup.Craft
{
    /// <summary>
    /// 상단 장착 슬롯 1개 (Main / Sub1 / Sub2).
    /// </summary>
    public class MIEquipSlotViewer : MonoBehaviour
    {
        [SerializeField] private Image _icon;

        private MIPickaxeUIDataTable _iconTable;
        private EEquipSlot _slot;

        /// <summary>슬롯 초기화</summary>
        public void Setup(MIPickaxeUIDataTable iconTable, EEquipSlot slot, EPickaxeType equipped)
        {
            _iconTable = iconTable;
            _slot = slot;
            Refresh(equipped);
        }

        /// <summary>장착 곡괭이 변경 시 갱신</summary>
        public void Refresh(EPickaxeType equipped)
        {
            bool isEmpty = equipped == EPickaxeType.None;

            _icon.sprite = _iconTable.GetPickaxeIcon(equipped);

        }
    }
}
