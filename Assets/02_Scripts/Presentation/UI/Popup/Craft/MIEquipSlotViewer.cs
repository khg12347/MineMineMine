using MI.Data.UIRes;
using MI.Domain.Pickaxe;
using MI.Domain.Pickaxe.Equipment;
using MI.Presentation.UI.Common;
using MI.Utility;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MI.Presentation.UI.Popup.Craft
{
    /// <summary>
    /// 상단 장착 슬롯 1개 (Main / Sub1 / Sub2).
    /// </summary>
    public class MIEquipSlotViewer : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image _icon;
        [SerializeField] private MIButton _button;
        [SerializeField] private GameObject _goSelectFrame;

        private MIPickaxeUIDataTable _iconTable;
        private EEquipSlot _slot;
        private EPickaxeType _equippedType;

        private GameObject _ghostIcon; // 드래그 중인 고스트 아이콘

        public EPickaxeType EquippedType => _equippedType;
        public EEquipSlot Slot => _slot;

        /// <summary>슬롯 초기화</summary>
        public void Setup(MIPickaxeUIDataTable iconTable, EEquipSlot slot, EPickaxeType equipped, Action<EEquipSlot> onClickAction = null)
        {
            _iconTable = iconTable;
            _slot = slot;
            _equippedType = equipped;

            SetSelected(false);
            _button.onClick.AddListener(() => onClickAction?.Invoke(_slot));
            _button.onClick.AddListener(() => SetSelected(false));

            Refresh(equipped);
        }

        /// <summary>장착 곡괭이 변경 시 갱신</summary>
        public void Refresh(EPickaxeType equipped)
        {
            bool isEmpty = equipped == EPickaxeType.None;

            _equippedType = equipped;
            _icon.sprite = _iconTable.GetPickaxeIcon(equipped);

        }

        /// <summary>장착 버튼 클릭 -> Select Frame 및 Button 활성화</summary>
        public void SetSelected(bool selected)
        {
            _goSelectFrame.SetActive(selected);
            _button.interactable = selected;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // 고스트 아이콘 예외처리
            if (_ghostIcon != null) Destroy(_ghostIcon);

            // 고스트 아이콘 생성
            _ghostIcon = Instantiate(_icon.gameObject, transform.parent.parent);
            var ghostImage = _ghostIcon.GetComponent<Image>();
            ghostImage.color = new Color(1f, 1f, 1f, 0.6f); // 반투명
            ghostImage.raycastTarget = false;
            _ghostIcon.transform.SetAsLastSibling(); // 최상단으로
            _ghostIcon.transform.position = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_ghostIcon != null)
            {
                _ghostIcon.transform.position = eventData.position;
            }
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            if (_ghostIcon != null)
            {
                Destroy(_ghostIcon);
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            MILog.Log("OnDrop: " + eventData.pointerDrag.name);
            var craftSlot = eventData.pointerDrag.GetComponent<MICraftSlotViewer>();
            var popupCraft = GetComponentInParent<MIPopupCraft>();

            if (eventData.pointerDrag.TryGetComponent(out MICraftSlotViewer craftSlotViewer))
            {
                if (popupCraft != null)
                {
                    popupCraft.OnEquipSlotSelected(_slot, craftSlotViewer.Type);
                }
            }
            else if (eventData.pointerDrag.TryGetComponent(out MIEquipSlotViewer equipSlotViewer))
            {
                MILog.Log("EquipSlotViewer OnDrop: " + equipSlotViewer.EquippedType);
                if (popupCraft != null)
                {
                    // 서로 장착된 곡괭이 교체
                    var capturedType = equipSlotViewer.EquippedType;
                    popupCraft.OnEquipSlotSelected(equipSlotViewer.Slot, _equippedType);
                    popupCraft.OnEquipSlotSelected(_slot, capturedType);
                }

            }
            else
            {
                MILog.LogWarning("Dropped object is not a valid slot viewer.");
            }
        }
    }
}
