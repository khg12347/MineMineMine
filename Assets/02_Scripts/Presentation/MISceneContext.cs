using MI.Core.ServiceLocator;
using MI.Data.Config;
using MI.Data.UIRes;
using MI.Domain.GameRoot;
using MI.Domain.Pickaxe.Craft;
using MI.Domain.Pickaxe.Equipment;
using MI.Domain.User;
using MI.Presentation.UI;
using MI.Presentation.UI.HUD.Wallet;
using MI.Presentation.UI.Popup.Craft;
using MI.Presentation.UI.Popup.Inventory;
using MI.Presentation.World.Pickaxe;
using MI.Presentation.World.Stage;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MI.Presentation
{
    /// <summary>
    /// 씬 Composition Root. 모든 Presentation 컴포넌트에 Domain 의존성을 주입한다.
    /// Domain 레이어의 IMISceneInitializer를 구현하여 역방향 의존을 제거한다.
    /// </summary>
    public class MISceneContext : MonoBehaviour, IMISceneInitializer
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
            MIServiceLocator.Register<IMISceneInitializer>(this);
        }

        private void OnDestroy()
        {
            if (Current == this)
            {
                MIServiceLocator.Unregister<IMISceneInitializer>();
                Current = null;
            }
        }

        #endregion Unity Events

        #region IMISceneInitializer

        /// <inheritdoc/>
        public void InitializeSceneContext()
        {
            var userState = MIServiceLocator.Get<MIUserState>();

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

        #endregion IMISceneInitializer
    }
}