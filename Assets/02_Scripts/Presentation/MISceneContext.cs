using System;
using MI.Data.Config;
using MI.Data.UIRes;
using MI.Domain.Pickaxe.Craft;
using MI.Domain.Pickaxe.Enhance;
using MI.Data.Pickaxe.Equipment;
using MI.Domain.Pickaxe.Equipment;
using MI.Domain.User;
using MI.Presentation.UI;
using MI.Presentation.UI.HUD.Wallet;
using MI.Presentation.UI.Popup.Craft;
using MI.Presentation.UI.Popup.Enhance;
using MI.Presentation.UI.Popup.Inventory;
using MI.Presentation.World.Pickaxe;
using MI.Presentation.World.Stage;
using MI.Utility;
using VContainer;

using UnityEngine;

namespace MI.Presentation
{
    /// <summary>
    /// 씬 Composition Root. 모든 Presentation 컴포넌트에 Domain 의존성을 주입한다.
    /// MIRootLifetimeScope의 RegisterBuildCallback에서 [Inject] 및 InitializeSceneContext()가 호출된다.
    /// </summary>
    public class MISceneContext : MonoBehaviour
    {
        [SerializeField] private Vector2Int _testBit;
        [Header("UI")]
        [SerializeField] private MIPopupInventory _popupInventory;
        [SerializeField] private MIWalletHUD _walletHUD;
        [SerializeField] private MIPopupCraft _popupCraft;
        [SerializeField] private MIEnhancePopup _popupEnhance;

        // UI 리소스가 많아지면 데이터 컨테이너 패턴으로 리팩토링
        [Header("Resources")]
        [SerializeField] private MIUINumberResources _numberResources;
        [SerializeField] private MIItemIconDataTable _itemIconDataTable;
        [SerializeField] private MIPickaxeUIDataTable _pickaxeIconDataTable;

        [Header("Stage")]
        [SerializeField] private MIStageOrchestrator _stageOrchestrator;

        [Header("Pickaxe")]
        [SerializeField] private GameObject _pickaxePrefab;

        private MIPickaxeEquipController _pickaxeEquipController;

        // VContainer 주입 필드
        private MIUserState _userState;
        private IMIPickaxeCraftService _craftService;
        private IMIPickaxeEnhanceService _enhanceService;
        private IMIPickaxeDataRegistry _pickaxeData;

        public static MISceneContext Current { get; private set; }

        public MonoBehaviour UIContextMonoBehaviour;
        public IMIUIContext UIContext => UIContextMonoBehaviour as IMIUIContext;

        #region Unity Events

        private void Awake()
        {
            Current = this;
        }

        private void OnDestroy()
        {
            if (Current == this)
                Current = null;
        }

        #endregion Unity Events

        #region DI

        /// <summary>VContainer에서 호출. Domain 서비스 참조를 저장한다.</summary>
        [Inject]
        public void Construct(
            MIUserState userState,
            IMIPickaxeCraftService craftService,
            IMIPickaxeEnhanceService enhanceService,
            IMIPickaxeDataRegistry pickaxeData)
        {
            _userState = userState;
            _craftService = craftService;
            _enhanceService = enhanceService;
            _pickaxeData = pickaxeData;

            InitializeSceneContext();
        }

        #endregion DI

        #region Scene Initialization

        /// <summary>씬 컨텍스트 초기화. UI/월드 컴포넌트에 의존성을 주입한다.</summary>
        public void InitializeSceneContext()
        {
            // 인벤토리 팝업
            _popupInventory.InjectResources(_numberResources, _itemIconDataTable);
            _popupInventory.InjectInventory(_userState.Inventory);

            // 지갑 HUD
            _walletHUD.InjectNumberResources(_numberResources);
            _walletHUD.InjectWallet(_userState.Wallet);

            // 제작 팝업
            if (_popupCraft != null)
            {
                _popupCraft.InjectResources(_numberResources, _pickaxeIconDataTable, _itemIconDataTable);
                _popupCraft.Initialize(_craftService, _userState.PickaxeInventory, _userState.PickaxeInventory, _pickaxeData.CraftConfig, _userState.Inventory);
            }

            // 강화 팝업
            if (_popupEnhance != null)
            {
                _popupEnhance.InjectResources(_numberResources, _pickaxeIconDataTable, _itemIconDataTable);
                _popupEnhance.Initialize(
                    _enhanceService,
                    _userState.PickaxeInventory,
                    _pickaxeData.EnhanceCostConfig,
                    _userState.Wallet,
                    _userState.Inventory);
            }

            // 곡괭이 장착 컨트롤러 생성 + 초기화
            var mainCamera = Camera.main;
            var goPickaxeEquipController = new GameObject("MIPickaxeEquipController");
            _pickaxeEquipController = goPickaxeEquipController.AddComponent<MIPickaxeEquipController>();
            _pickaxeEquipController.Initialize(_pickaxeData.SpecDataTable, _userState.PickaxeInventory, mainCamera, _pickaxePrefab);

            // StageOrchestrator에 메인 곡괭이 주입
            if (_stageOrchestrator != null)
            {
                var mainType = _userState.PickaxeInventory.GetEquipped(EEquipSlot.Main);
                var mainConfig = _pickaxeData.SpecDataTable.GetConfig(mainType);
                _stageOrchestrator.InjectPickaxe(_pickaxeEquipController.MainPickaxe, mainConfig);
            }
        }

        #endregion Scene Initialization
        
    }
}
