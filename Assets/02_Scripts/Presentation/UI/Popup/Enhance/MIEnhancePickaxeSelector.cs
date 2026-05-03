using System;
using System.Collections.Generic;
using MI.Data.UIRes;
using MI.Data.Pickaxe;
using MI.Domain.Pickaxe.Equipment;
using MI.Presentation.UI.Common;
using MI.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MI.Presentation.UI.Popup.Enhance
{
    /// <summary>
    /// 강화 팝업 좌측 Pickaxe Zone — 보유 곡괭이 중 하나를 선택해 표시한다.
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

            if (_btnLeft != null)
            {
                _btnLeft.onClick.RemoveAllListeners();
                _btnLeft.onClick.AddListener(OnLeftClicked);
            }

            if (_btnRight != null)
            {
                _btnRight.onClick.RemoveAllListeners();
                _btnRight.onClick.AddListener(OnRightClicked);
            }

            RefreshVisual();
            RefreshNavButtons();
        }

        public void RefreshOwned()
        {
            _ownedList.Clear();
            _ownedList.AddRange(_pickaxeInventory.OwnedPickaxes);
            
            RefreshVisual();
            RefreshNavButtons();
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

        #region Button Callbacks

        private void OnLeftClicked()
        {
            MILog.Log($"OnLeftClicked {_index}");
            if (_index <= 0) return;

            _index--;
            RefreshVisual();
            RefreshNavButtons();
            OnSelectionChanged?.Invoke(Current);
        }

        private void OnRightClicked()
        {
            MILog.Log($"OnRightClicked {_index}");
            if (_index >= _ownedList.Count - 1) return;

            _index++;
            RefreshVisual();
            RefreshNavButtons();
            OnSelectionChanged?.Invoke(Current);
        }

        #endregion Button Callbacks

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

        /// <summary>현재 인덱스에 따라 좌/우 버튼 인터랙터블을 갱신한다.</summary>
        private void RefreshNavButtons()
        {
            if (_btnLeft != null) _btnLeft.interactable = _index > 0;
            if (_btnRight != null) _btnRight.interactable = _index < _ownedList.Count - 1;
        }

        #endregion Helper
    }
}
