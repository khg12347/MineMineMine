using MI.Core.ServiceLocator;
using MI.Data.Config;
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

        private MIUserState _userState;
        private MIPickaxeCraftService _craftService;

        #region Lifecycle

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _userState = new MIUserState();
            MIServiceLocator.Register(_userState);

            _craftService = new MIPickaxeCraftService(
                _pickaxeCraftConfig,
                _userState.Inventory,
                _userState.Wallet,
                _userState.PickaxeInventory);
            MIServiceLocator.Register<IMIPickaxeCraftService>(_craftService);

            MISceneContext.Current.InitializeSceneContext();
        }
        private void Start()
        {
            
        }

        private void OnDestroy()
        {
            MIServiceLocator.Unregister<IMIPickaxeCraftService>();
            MIServiceLocator.Unregister<MIUserState>();
            _userState?.Dispose();
            _userState = null;
            _craftService = null;
        }

        #endregion Lifecycle
    }
}
