using MI.Data.Config;
using MI.Data.UIRes;
using MI.Data.Pickaxe;
using MI.Domain.Pickaxe.Enhance;
using MI.Domain.Pickaxe.Equipment;
using MI.Domain.UserState.Inventory;
using MI.Domain.UserState.Wallet;
using UnityEngine;

namespace MI.Presentation.UI.Popup.Enhance
{
    /// <summary>
    /// 곡괭이 강화 팝업. MIPopupBase 상속.
    /// 좌측 Pickaxe Zone(선택) + 우측 Enhance/Ability 탭으로 구성.
    /// </summary>
    public class MIEnhancePopup : MIPopupBase
    {
        [SerializeField] private MIEnhancePickaxeSelector _selector;
        [SerializeField] private MIEnhanceTab _enhanceTab;
        [SerializeField] private MIAbilityTab _abilityTab;

        private IMIPickaxeEnhanceService _enhanceService;
        private IMIPickaxeInventory _pickaxeInventory;
        private MIEnhanceCostConfig _costConfig;
        private MIUserWallet _userWallet;
        private MIUserInventory _userInventory;

        private MIUINumberResources _numberResources;
        private MIPickaxeUIDataTable _pickaxeIconTable;
        private MIItemIconDataTable _itemIconTable;

        #region Initialization

        public void InjectResources(
            MIUINumberResources numberResources,
            MIPickaxeUIDataTable pickaxeIconTable,
            MIItemIconDataTable itemIconTable)
        {
            _numberResources = numberResources;
            _pickaxeIconTable = pickaxeIconTable;
            _itemIconTable = itemIconTable;
        }

        /// <summary>
        /// 의존성 주입. MISceneContext.InitializeSceneContext()에서 호출.
        /// </summary>
        public void Initialize(
            IMIPickaxeEnhanceService enhanceService,
            IMIPickaxeInventory pickaxeInventory,
            MIEnhanceCostConfig costConfig,
            MIUserWallet userWallet,
            MIUserInventory userInventory)
        {
            _enhanceService = enhanceService;
            _pickaxeInventory = pickaxeInventory;
            _costConfig = costConfig;
            _userWallet = userWallet;
            _userInventory = userInventory;

            if (_selector != null)
            {
                _selector.Initialize(_pickaxeInventory, _pickaxeIconTable);
                _selector.OnSelectionChanged += HandleSelectionChanged;
            }

            _enhanceService.OnEnhanceAttempted += HandleEnhanceAttempted;

            if (_enhanceTab != null)
            {
                _enhanceTab.Initialize(
                    _enhanceService,
                    _pickaxeInventory,
                    _userInventory,
                    _userWallet,
                    _costConfig,
                    _itemIconTable);
            }

            // Step 6 까지 잠재능력 탭은 비활성.
            if (_abilityTab != null)
                _abilityTab.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_selector != null)
                _selector.OnSelectionChanged -= HandleSelectionChanged;

            if (_enhanceService != null)
                _enhanceService.OnEnhanceAttempted -= HandleEnhanceAttempted;
        }

        #endregion Initialization

        #region MIPopupBase

        protected override void OpenPopup()
        {
            base.OpenPopup();
            RefreshAll();
        }

        #endregion MIPopupBase

        #region Event Handlers

        /// <summary>좌/우 버튼으로 선택 곡괭이 변경 → 강화 탭 갱신.</summary>
        private void HandleSelectionChanged(EPickaxeType type)
        {
            if (_enhanceTab != null)
                _enhanceTab.Refresh(type);
        }

        /// <summary>강화 완료 → selector 공격력 텍스트 갱신.</summary>
        private void HandleEnhanceAttempted(FEnhanceAttemptResult result)
        {
            if (_selector != null)
                _selector.RefreshVisual();
        }

        #endregion Event Handlers

        #region Helper

        private void RefreshAll()
        {
            if (_selector == null || _enhanceTab == null) return;

            _selector.RefreshVisual();
            _selector.RefreshOwned();
            _enhanceTab.Refresh(_selector.Current);
        }

        #endregion Helper
    }
}
