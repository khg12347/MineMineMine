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

        // "MAIN", "SUB1", "SUB2" 레이블
        [SerializeField] private TextMeshProUGUI _slotLabel;

        // 빈 슬롯 상태 표시
        [SerializeField] private GameObject _emptyState;

        private EEquipSlot _slot;

        /// <summary>슬롯 초기화</summary>
        public void Setup(EEquipSlot slot, EPickaxeType equipped)
        {
            _slot = slot;
            _slotLabel.text = slot.ToString().ToUpper();
            Refresh(equipped);
        }

        /// <summary>장착 곡괭이 변경 시 갱신</summary>
        public void Refresh(EPickaxeType equipped)
        {
            bool isEmpty = equipped == EPickaxeType.None;
            _emptyState.SetActive(isEmpty);
            _icon.gameObject.SetActive(!isEmpty);

            if (!isEmpty)
            {
                // TODO: 곡괭이 타입에 맞는 스프라이트 설정
                // _icon.sprite = ...;
            }
        }
    }
}
