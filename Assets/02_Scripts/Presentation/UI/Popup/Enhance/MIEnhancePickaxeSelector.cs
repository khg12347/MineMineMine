using System;
using System.Collections.Generic;
using MI.Data.UIRes;
using MI.Domain.Pickaxe;
using MI.Domain.Pickaxe.Equipment;
using MI.Presentation.UI.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MI.Presentation.UI.Popup.Enhance
{
    /// <summary>
    /// 강화 팝업 좌측 Pickaxe Zone — 보유 곡괭이 중 하나를 선택해 표시한다.
    /// Step 1 범위: 첫 항목 표시. 좌/우 페이지 동작은 Step 2.
    /// </summary>
    public class MIEnhancePickaxeSelector : MonoBehaviour
    {
        [SerializeField] private Image _pickaxeIcon;
        [SerializeField] private TextMeshProUGUI _pickaxeName;
        [SerializeField] private TextMeshProUGUI _damageText;

        [SerializeField] private MIButton _btnLeft;
        [SerializeField] private MIButton _btnRight;

        private IMIPickaxeInventory _pickaxeInventory;
        private MIPickaxeUIDataTable _pickaxeIconTable;

        private readonly List<EPickaxeType> _ownedList = new();
        private int _index;

        /// <summary>현재 선택된 곡괭이. 보유 목록이 비면 None.</summary>
        public EPickaxeType Current =>
            _ownedList.Count == 0 ? EPickaxeType.None : _ownedList[_index];

        /// <summary>선택 변경 시 발행 (Step 2 에서 좌/우 버튼 핸들러가 호출).</summary>
        public event Action<EPickaxeType> OnSelectionChanged;

        #region Public API

        public void Initialize(IMIPickaxeInventory pickaxeInventory, MIPickaxeUIDataTable pickaxeIconTable)
        {
            _pickaxeInventory = pickaxeInventory;
            _pickaxeIconTable = pickaxeIconTable;

            RebuildOwnedList();

            // Step 1: 좌/우 버튼은 비활성. 동작은 Step 2 에서 구현.
            if (_btnLeft != null) _btnLeft.interactable = false;
            if (_btnRight != null) _btnRight.interactable = false;

            RefreshVisual();
        }

        /// <summary>현재 선택 곡괭이의 표시를 다시 그린다 (강화 후 공격력 변경 등에 사용).</summary>
        public void RefreshVisual()
        {
            var current = Current;
            if (current == EPickaxeType.None)
            {
                if (_pickaxeIcon != null) _pickaxeIcon.enabled = false;
                if (_pickaxeName != null) _pickaxeName.text = string.Empty;
                if (_damageText != null) _damageText.text = string.Empty;
                return;
            }

            if (_pickaxeIcon != null)
            {
                _pickaxeIcon.enabled = true;
                _pickaxeIcon.sprite = _pickaxeIconTable.GetPickaxeIcon(current);
            }

            if (_pickaxeName != null)
                _pickaxeName.text = _pickaxeIconTable.GetPickaxeName(current);

            if (_damageText != null)
            {
                var instance = _pickaxeInventory.GetInstance(current);
                int damage = instance.HasValue ? instance.Value.ResolvedStats.HeadDamage : 0;
                _damageText.text = damage.ToString();
            }
        }

        #endregion Public API

        #region Helper

        private void RebuildOwnedList()
        {
            _ownedList.Clear();
            foreach (var type in _pickaxeInventory.OwnedPickaxes)
                _ownedList.Add(type);

            if (_ownedList.Count == 0)
            {
                _index = 0;
                return;
            }

            _index = Mathf.Clamp(_index, 0, _ownedList.Count - 1);
        }

        #endregion Helper
    }
}
