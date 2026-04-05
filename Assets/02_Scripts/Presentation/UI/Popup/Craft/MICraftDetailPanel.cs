using System;
using MI.Data.UIRes;
using MI.Domain.Pickaxe;
using MI.Presentation.UI.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MI.Presentation.UI.Popup.Craft
{
    /// <summary>
    /// 하단 상세 패널. 제작 모드 / 장착 모드를 전환.
    /// </summary>
    public class MICraftDetailPanel : MonoBehaviour
    {
        [Header("공통")]
        [SerializeField] private TextMeshProUGUI _pickaxeName;
        [SerializeField] private Image _pickaxeIcon;

        [Header("제작 모드")]
        [SerializeField] private GameObject _craftModeRoot;
        [SerializeField] private MIButton _craftButton;

        // 재료 슬롯 (최대 2개)
        [SerializeField] private GameObject[] _materialSlots;
        [SerializeField] private MIImageGroups[] _materialAmountImageGroups; // 재료 수량 표기하는 이미지 그룹
        [SerializeField] private Image[] _materialIcons;

        // 재화 비용 영역 (없으면 숨김)
        [SerializeField] private GameObject _currencyRoot;
        [SerializeField] private TextMeshProUGUI _currencyAmount;
        [SerializeField] private Image _currencyIcon;

        [Header("장착 모드")]
        [SerializeField] private GameObject _equipModeRoot;
        [SerializeField] private MIButton _equipButton;
        [SerializeField] private MIButton _infoButton;

        private MIPickaxeUIDataTable _pickaxeIconDataTable;
        private MIItemIconDataTable _itemIconDataTable;
        private MIUINumberResources _uiNumberResources;
        #region Craft Mode

        public void InitDataTable(MIPickaxeUIDataTable iconDataTable, MIItemIconDataTable itemIconDataTable, MIUINumberResources uiNumberResources)
        {
            _pickaxeIconDataTable = iconDataTable;
            _itemIconDataTable = itemIconDataTable;
            _uiNumberResources = uiNumberResources;
        }

        /// <summary>
        /// 제작 모드로 전환. 재료/재화 정보 표시.
        /// </summary>
        public void ShowCraftMode(
            EPickaxeType type,
            FPickaxeCraftCost? cost,
            bool canCraft,
            IMIPickaxeCraftService craftService,
            Action onCraftClicked)
        {
            // 일단 제작 모드로 시작, 첫 포커스 슬롯의 제작 여부에 따라 달라짐
            _craftModeRoot.SetActive(true);
            _equipModeRoot.SetActive(false);

            // 곡괭이 이름/아이콘 설정
            _pickaxeName.text = _pickaxeIconDataTable.GetPickaxeName(type);
            _pickaxeIcon.sprite = _pickaxeIconDataTable.GetPickaxeIcon(type);

            if (!cost.HasValue) return;

            var c = cost.Value;

            // 재료 슬롯 표시
            for (int i = 0; i < _materialSlots.Length; i++)
            {
                if (c.Materials != null && i < c.Materials.Length)
                {
                    _materialSlots[i].SetActive(true);
                    var mat = c.Materials[i];

                    // TODO: 아이콘 설정
                    _materialIcons[i].sprite = _itemIconDataTable.GetItemIcon(mat.ItemType);

                    bool enough = craftService.HasEnoughMaterial(mat);
                    var requiredAmount = mat.Amount;
                    var currentAmount = craftService.GetMaterialAmount(mat);
                    string amountText = $"{currentAmount}/{requiredAmount}";
                    MITextSprite.SetTextSprite(amountText, _materialAmountImageGroups[i].ImageList, _uiNumberResources);
                }
                else
                {
                    _materialSlots[i].SetActive(false);
                }
            }

            //// 재화 비용 — 없으면 영역 자체 숨김
            //if (c.Currencies != null && c.Currencies.Length > 0)
            //{
            //    _currencyRoot.SetActive(true);
            //    var cur = c.Currencies[0];

            //    // TODO: 재화 아이콘 설정
            //    // _currencyIcon.sprite = ...;

            //    bool enough = craftService.HasEnoughCurrency(cur);
            //    _currencyAmount.text = cur.Amount.ToString();
            //    _currencyAmount.color = enough ? Color.white : Color.red;
            //}
            //else
            //{
            //    _currencyRoot.SetActive(false);
            //}

            // 제작 버튼
            _craftButton.interactable = canCraft;
            _craftButton.onClick.RemoveAllListeners();
            _craftButton.onClick.AddListener(() => onCraftClicked?.Invoke());
        }

        #endregion Craft Mode

        #region Equip Mode

        /// <summary>
        /// 장착 모드로 전환. 장착/정보/강화 버튼 표시.
        /// </summary>
        /// <param name="type">선택된 곡괭이 타입</param>
        /// <param name="onEquipClicked">장착 버튼 콜백</param>
        /// <param name="onInfoClicked">정보 버튼 콜백</param>
        /// <param name="onEnhanceClicked">강화 버튼 콜백 (null이면 버튼 숨김)</param>
        public void ShowEquipMode(
            EPickaxeType type,
            Action onEquipClicked,
            Action onInfoClicked,
            Action<EPickaxeType> onEnhanceClicked = null)
        {
            _craftModeRoot.SetActive(false);
            _equipModeRoot.SetActive(true);

            // TODO: 곡괭이 이름/아이콘 설정
            // _pickaxeName.text = ...;
            // _pickaxeIcon.sprite = ...;

            _equipButton.onClick.RemoveAllListeners();
            _equipButton.onClick.AddListener(() => onEquipClicked?.Invoke());

            _infoButton.onClick.RemoveAllListeners();
            _infoButton.onClick.AddListener(() => onInfoClicked?.Invoke());
        }

        #endregion Equip Mode
    }
}
