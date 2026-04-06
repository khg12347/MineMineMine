using System;
using MI.Data.UIRes;
using MI.Domain.Pickaxe;
using MI.Presentation.UI.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MI.Presentation.UI.Popup.Craft
{
    /// <summary>
    /// 제작 슬롯 15칸 중 1칸.
    /// 곡괭이 아이콘 + 보유 상태 표시. 보유 시 강화 버튼 영역 활성화.
    /// </summary>
    public class MICraftSlotViewer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image _icon;

        [SerializeField] private MIButton _button;

        private MIPickaxeUIDataTable _iconDataTable;

        private EPickaxeType _type;
        private bool _isOwned;
        private Action<EPickaxeType> _onClick;

        private GameObject _ghostIcon; // 드래그 시 따라다니는 아이콘(캐싱용)
        
        public EPickaxeType Type => _type;

        #region Setup

        /// <summary>일반 슬롯 초기화 (제작 가능한 10종)</summary>
        /// <param name="type">곡괭이 타입</param>
        /// <param name="isOwned">현재 보유 여부</param>
        /// <param name="onClick">슬롯 클릭 콜백</param>
        public void Setup(
            MIPickaxeUIDataTable iconDataTable,
            EPickaxeType type,
            bool isOwned,
            Action<EPickaxeType> onClick)
        {
            _iconDataTable = iconDataTable;
            _type = type;
            _isOwned = isOwned;
            _onClick = onClick;

            _icon.gameObject.SetActive(_isOwned);
            _button.interactable = true;

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => _onClick?.Invoke(_type));

            _icon.sprite = _iconDataTable.GetPickaxeIcon(_type);

            RefreshVisual();
        }

        /// <summary>미해금 슬롯 초기화 (??? 표시)</summary>
        public void SetupLocked()
        {
            _icon.gameObject.SetActive(false);
            _type = EPickaxeType.None;
        }

        #endregion Setup

        #region Refresh visual state

        /// <summary>보유 상태 변경 시 갱신 (제작 완료 이벤트에서 호출)</summary>
        public void SetOwned(bool isOwned)
        {
            _isOwned = isOwned;
            RefreshVisual();
        }

        private void RefreshVisual()
        {
            // TODO: 보유/미보유 시각 차이 (밝기, 테두리 등)
            _icon.gameObject.SetActive(_isOwned);
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

        #endregion Visual State
    }
}