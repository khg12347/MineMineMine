using MI.Data.UIRes;
using MI.Domain.Pickaxe;
using MI.Domain.Pickaxe.Equipment;
using MI.Presentation.UI.Common;
using System;
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
        [SerializeField] private MIButton _button;
        [SerializeField] private GameObject _goSelectFrame;

        private MIPickaxeUIDataTable _iconTable;
        private EEquipSlot _slot;

        /// <summary>슬롯 초기화</summary>
        public void Setup(MIPickaxeUIDataTable iconTable, EEquipSlot slot, EPickaxeType equipped, Action<EEquipSlot> onClickAction = null)
        {
            _iconTable = iconTable;
            _slot = slot;

            SetSelected(false);
            _button.onClick.AddListener(() => onClickAction?.Invoke(_slot));
            _button.onClick.AddListener(() => SetSelected(false));

            Refresh(equipped);
        }

        /// <summary>장착 곡괭이 변경 시 갱신</summary>
        public void Refresh(EPickaxeType equipped)
        {
            bool isEmpty = equipped == EPickaxeType.None;

            _icon.sprite = _iconTable.GetPickaxeIcon(equipped);

        }

        /// <summary>장착 버튼 클릭 -> Select Frame 및 Button 활성화</summary>
        public void SetSelected(bool selected)
        {
            _goSelectFrame.SetActive(selected);
            _button.interactable = selected;
        }

    }
}
