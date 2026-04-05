using MI.Core.ServiceLocator;
using MI.Data.Config;
using MI.Data.UIRes;
using MI.Domain.Pickaxe;
using MI.Domain.Pickaxe.Craft;
using MI.Domain.Pickaxe.Equipment;
using MI.Presentation.UI;
using MI.Presentation.UI.HUD.Wallet;
using MI.Presentation.UI.Popup.Craft;
using MI.Presentation.UI.Popup.Inventory;
using MI.Presentation.World.Pickaxe;
using MI.Presentation.World.Stage;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MI.Domain.GameRoot
{
    public class MISceneContext : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private MIPopupInventory _popupInventory;
        [SerializeField] private MIWalletHUD _walletHUD;
        [SerializeField] private MIPopupCraft _popupCraft;

        // UI 리소스가 많아지면 데이터 컨테이너 패턴으로 리팩토링
        [Header("Resources")]
        [SerializeField] private MIUINumberResources _numberResources;
        [SerializeField] private MIItemIconDataTable _itemIconDataTable;
        [SerializeField] private MIPickaxeUIDataTable _pickaxeIconDataTable;

        [Header("Stage")]
        [SerializeField] private MIStageOrchestrator _stageOrchestrator;

        [Header("Pickaxe")]
        [SerializeField] private GameObject _pickaxePrefab;

        [Header("Config")]
        [SerializeField] private MIPickaxeCraftConfig _pickaxeCraftConfig;
        [SerializeField] private MIPickaxeSpecDataTable _pickaxeSpecDataTable;

        private MIPickaxeEquipController _pickaxeEquipController;

        private const string SCENE_NAME = "Main";

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

        #region Public API

        public void InitializeSceneContext()
        {
            var userState = MIServiceLocator.Get<User.MIUserState>();

            // 인벤토리 팝업
            _popupInventory.InjectResources(_numberResources, _itemIconDataTable);
            _popupInventory.InjectInventory(userState.Inventory);

            // 지갑 HUD
            _walletHUD.InjectNumberResources(_numberResources);
            _walletHUD.InjectWallet(userState.Wallet);

            // 제작 팝업
            if (_popupCraft != null)
            {
                _popupCraft.InjectResources(_numberResources, _pickaxeIconDataTable, _itemIconDataTable);
                var craftService = MIServiceLocator.Get<IMIPickaxeCraftService>();
                _popupCraft.Initialize(craftService, userState.PickaxeInventory, userState.PickaxeInventory, _pickaxeCraftConfig, userState.Inventory);
            }

            // 기본 곡괭이 지급
            GrantDefaultPickaxe(userState);

            // 곡괭이 매니저 생성 + 초기화
            var mainCamera = UnityEngine.Camera.main;
            var goPickaxeEquipController = new GameObject("MIPickaxeEquipController");

            // 부트스트랩 씬에서 생성되는 문제가 있어 임시 처리, 씬 로드 시스템 구현 시 변경 예정
            SceneManager.MoveGameObjectToScene(goPickaxeEquipController, SceneManager.GetSceneByName(SCENE_NAME));

            _pickaxeEquipController = goPickaxeEquipController.AddComponent<MIPickaxeEquipController>();
            _pickaxeEquipController.Initialize(_pickaxeSpecDataTable, userState.PickaxeInventory, mainCamera, _pickaxePrefab);

            // StageOrchestrator에 메인 곡괭이 주입
            if (_stageOrchestrator != null)
            {
                var mainType = userState.PickaxeInventory.GetEquipped(EEquipSlot.Main);
                var mainConfig = _pickaxeSpecDataTable.GetConfig(mainType);
                _stageOrchestrator.InjectPickaxe(_pickaxeEquipController.MainPickaxe, mainConfig);
            }
        }

        #endregion Public API

        #region Helper

        /// <summary>기본 곡괭이를 지급하고 Main 슬롯에 장착한다. 이미 보유 중이면 스킵.</summary>
        private void GrantDefaultPickaxe(User.MIUserState userState)
        {
            if (_pickaxeSpecDataTable == null) return;

            var defaultType = _pickaxeSpecDataTable.DefaultPickaxeType;
            if (defaultType == EPickaxeType.None) return;
            if (userState.PickaxeInventory.IsOwned(defaultType)) return;

            var stats = _pickaxeSpecDataTable.GetStats(defaultType);
            if (!stats.HasValue) return;

            var instance = FPickaxeInstance.Create(defaultType, stats.Value);
            userState.PickaxeInventory.AddPickaxe(instance);
            userState.PickaxeInventory.Equip(defaultType, EEquipSlot.Main);
        }

        #endregion Helper
    }
}