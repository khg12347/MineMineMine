using MI.Data.Config;
using MI.Data.UIRes;
using MI.Domain.Pickaxe;
using MI.Domain.Pickaxe.Enhance;
using MI.Domain.Pickaxe.Equipment;
using MI.Domain.UserState.Inventory;
using MI.Domain.UserState.Wallet;
using MI.Presentation.UI.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MI.Presentation.UI.Popup.Enhance
{
    /// <summary>
    /// 강화 탭 — 현재 선택된 곡괭이의 강화 비용·재화·확률을 표시한다.
    /// Step 1 범위: 정적 표시. 보유량 비교/버튼 활성 처리는 Step 3, 강화 실행은 Step 4.
    /// </summary>
    public class MIEnhanceTab : MonoBehaviour
    {
        [Header("재료 슬롯 (요구량 기준 N개)")]
        [SerializeField] private GameObject[] _materialSlots;
        [SerializeField] private Image[] _materialIcons;
        [SerializeField] private TextMeshProUGUI[] _materialAmountTexts;

        [Header("재화 영역")]
        [SerializeField] private GameObject _currencyRoot;
        [SerializeField] private TextMeshProUGUI _currencyAmountText;

        [Header("성공률")]
        [SerializeField] private TextMeshProUGUI _probabilityText;

        [Header("강화 버튼")]
        [SerializeField] private MIButton _btnEnhance;

        [Header("MAX 표기 (선택)")]
        [SerializeField] private GameObject _maxLevelLabel;

        private IMIPickaxeEnhanceService _enhanceService;
        private IMIPickaxeInventory _pickaxeInventory;
        private MIUserInventory _userInventory;
        private MIUserWallet _userWallet;
        private MIEnhanceCostConfig _costConfig;
        private MIItemIconDataTable _itemIconTable;

        #region Public API

        public void Initialize(
            IMIPickaxeEnhanceService enhanceService,
            IMIPickaxeInventory pickaxeInventory,
            MIUserInventory userInventory,
            MIUserWallet userWallet,
            MIEnhanceCostConfig costConfig,
            MIItemIconDataTable itemIconTable)
        {
            _enhanceService = enhanceService;
            _pickaxeInventory = pickaxeInventory;
            _userInventory = userInventory;
            _userWallet = userWallet;
            _costConfig = costConfig;
            _itemIconTable = itemIconTable;
        }

        /// <summary>선택된 곡괭이 기준으로 비용/확률 표시를 갱신한다.</summary>
        public void Refresh(EPickaxeType type)
        {
            if (type == EPickaxeType.None)
            {
                SetVisible(false);
                return;
            }

            var entry = _enhanceService.GetCurrentLevelEntry(type);
            if (!entry.HasValue)
            {
                // MAX 도달 또는 데이터 없음 → 강화 영역 숨기고 MAX 라벨만.
                SetVisible(false);
                if (_maxLevelLabel != null) _maxLevelLabel.SetActive(true);
                return;
            }

            if (_maxLevelLabel != null) _maxLevelLabel.SetActive(false);
            SetVisible(true);

            var e = entry.Value;
            RefreshMaterials(e);
            RefreshCurrency(e);
            RefreshProbability(e);

            // Step 1: 버튼 활성/클릭 처리는 Step 3, Step 4 에서 추가.
            if (_btnEnhance != null) _btnEnhance.interactable = false;
        }

        #endregion Public API

        #region Helper

        private void SetVisible(bool visible)
        {
            for (int i = 0; i < _materialSlots.Length; i++)
            {
                if (_materialSlots[i] != null) _materialSlots[i].SetActive(visible);
            }

            if (_currencyRoot != null) _currencyRoot.SetActive(visible);
            if (_probabilityText != null) _probabilityText.gameObject.SetActive(visible);
            if (_btnEnhance != null) _btnEnhance.gameObject.SetActive(visible);
        }

        private void RefreshMaterials(FEnhanceLevelEntry entry)
        {
            int slotCount = _materialSlots != null ? _materialSlots.Length : 0;
            int matCount = entry.Materials != null ? entry.Materials.Length : 0;

            for (int i = 0; i < slotCount; i++)
            {
                if (i < matCount)
                {
                    if (_materialSlots[i] != null) _materialSlots[i].SetActive(true);

                    var mat = entry.Materials[i];

                    if (i < _materialIcons.Length && _materialIcons[i] != null)
                        _materialIcons[i].sprite = _itemIconTable.GetItemIcon(mat.ItemType);

                    if (i < _materialAmountTexts.Length && _materialAmountTexts[i] != null)
                        _materialAmountTexts[i].text = mat.Amount.ToString();
                }
                else
                {
                    if (_materialSlots[i] != null) _materialSlots[i].SetActive(false);
                }
            }
        }

        private void RefreshCurrency(FEnhanceLevelEntry entry)
        {
            bool hasCurrency = entry.Currencies != null && entry.Currencies.Length > 0;

            if (_currencyRoot != null) _currencyRoot.SetActive(hasCurrency);
            if (!hasCurrency) return;

            var cost = entry.Currencies[0];
            if (_currencyAmountText != null)
                _currencyAmountText.text = cost.Amount.ToString();
        }

        private void RefreshProbability(FEnhanceLevelEntry entry)
        {
            if (_probabilityText == null) return;
            int percent = Mathf.RoundToInt(entry.SuccessRate * 100f);
            _probabilityText.text = $"{percent}%";
        }

        #endregion Helper
    }
}
