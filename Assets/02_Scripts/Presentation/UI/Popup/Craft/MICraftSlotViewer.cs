using System;
using MI.Domain.Pickaxe;
using MI.Presentation.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace MI.Presentation.UI.Popup.Craft
{
    /// <summary>
    /// 제작 슬롯 15칸 중 1칸.
    /// 곡괭이 아이콘 + 보유 상태 표시. 보유 시 강화 버튼 영역 활성화.
    /// </summary>
    public class MICraftSlotViewer : MonoBehaviour
    {
        [SerializeField] private Image _icon;

        // ??? 표시용 오버레이 (미해금 슬롯)
        [SerializeField] private GameObject _lockedOverlay;

        [SerializeField] private MIButton _button;

        // 강화 시스템 연동: 보유 시 강화 버튼 노출
        [SerializeField] private GameObject _enhanceButtonRoot;

        private EPickaxeType _type;
        private bool _isOwned;
        private Action<EPickaxeType> _onClick;
        private Action<EPickaxeType> _onEnhanceClicked;

        #region Setup

        /// <summary>일반 슬롯 초기화 (제작 가능한 10종)</summary>
        /// <param name="type">곡괭이 타입</param>
        /// <param name="isOwned">현재 보유 여부</param>
        /// <param name="onClick">슬롯 클릭 콜백</param>
        /// <param name="onEnhanceClicked">강화 버튼 클릭 콜백 (null이면 버튼 숨김)</param>
        public void Setup(
            EPickaxeType type,
            bool isOwned,
            Action<EPickaxeType> onClick,
            Action<EPickaxeType> onEnhanceClicked = null)
        {
            _type = type;
            _isOwned = isOwned;
            _onClick = onClick;
            _onEnhanceClicked = onEnhanceClicked;

            _lockedOverlay.SetActive(false);
            _button.interactable = true;

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => _onClick?.Invoke(_type));

            // TODO: 곡괭이 타입에 맞는 스프라이트 설정
            // _icon.sprite = ...;

            RefreshVisual();
        }

        /// <summary>미해금 슬롯 초기화 (??? 표시)</summary>
        public void SetupLocked()
        {
            _lockedOverlay.SetActive(true);
            _button.interactable = false;

            if (_enhanceButtonRoot != null)
                _enhanceButtonRoot.SetActive(false);
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
            if (_enhanceButtonRoot != null)
                _enhanceButtonRoot.SetActive(_isOwned && _onEnhanceClicked != null);

            // TODO: 보유/미보유 시각 차이 (밝기, 테두리 등)
        }

        #endregion Visual State
    }
}
