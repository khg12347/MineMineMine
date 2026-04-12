
using MI.Data.Config;
using MI.Domain.Pickaxe;
using MI.Domain.Pickaxe.Craft;
using MI.Domain.Pickaxe.Equipment;
using MI.Domain.User;
using UnityEngine;

namespace MI.Domain.GameRoot
{
    /// <summary>
    /// 게임 루트 진입점. Non-MB 객체들을 직접 생성하고 관리합니다.
    /// BootStrap 씬에 배치하여 사용합니다.
    /// </summary>
    public class MIGameRoot : MonoBehaviour
    {
        [SerializeField] private MIPickaxeCraftConfig _pickaxeCraftConfig;
        [SerializeField] private MIPickaxeSpecDataTable _pickaxeSpecDataTable;

        private MIUserState _userState;
        private MIPickaxeCraftService _craftService;

        #region Lifecycle

        //private void Awake()
        //{
        //    DontDestroyOnLoad(gameObject);
        //    _userState = new MIUserState();
        //    MIServiceLocator.Register(_userState);

        //    _craftService = new MIPickaxeCraftService(
        //        _pickaxeCraftConfig,
        //        _userState.Inventory,
        //        _userState.Wallet,
        //        _userState.PickaxeInventory);
        //    MIServiceLocator.Register<IMIPickaxeCraftService>(_craftService);

        //    // 기본 곡괭이 지급 (Domain 로직)
        //    GrantDefaultPickaxe();

        //    // 씬 초기화 (인터페이스를 통해 Presentation에 위임)
        //    var sceneInitializer = MIServiceLocator.Get<IMISceneInitializer>();
        //    sceneInitializer.InitializeSceneContext();
        //}

        //private void OnDestroy()
        //{
        //    MIServiceLocator.Unregister<IMIPickaxeCraftService>();
        //    MIServiceLocator.Unregister<MIUserState>();
        //    _userState?.Dispose();
        //    _userState = null;
        //    _craftService = null;
        //}

        #endregion Lifecycle

        #region Helper

        /// <summary>기본 곡괭이를 지급하고 Main 슬롯에 장착한다. 이미 보유 중이면 스킵.</summary>
        private void GrantDefaultPickaxe()
        {
            if (_pickaxeSpecDataTable == null) return;

            var defaultType = _pickaxeSpecDataTable.DefaultPickaxeType;
            if (defaultType == EPickaxeType.None) return;
            if (_userState.PickaxeInventory.IsOwned(defaultType)) return;

            var stats = _pickaxeSpecDataTable.GetStats(defaultType);
            if (!stats.HasValue) return;

            var instance = FPickaxeInstance.Create(defaultType, stats.Value);
            _userState.PickaxeInventory.AddPickaxe(instance);
            _userState.PickaxeInventory.Equip(defaultType, EEquipSlot.Main);
        }

        #endregion Helper
    }
}
