using MI.Core.ServiceLocator;
using MI.Data.Config;
using MI.Data.UIRes;
using MI.Domain.Pickaxe;
using MI.Presentation.UI;
using MI.Presentation.UI.HUD.Wallet;
using MI.Presentation.UI.Popup.Craft;
using MI.Presentation.UI.Popup.Inventory;
using UnityEngine;

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

        [Header("Config")]
        [SerializeField] private MIPickaxeCraftConfig _pickaxeCraftConfig;

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
                var craftService = MIServiceLocator.Get<IMIPickaxeCraftService>();
                _popupCraft.Initialize(
                    craftService,
                    _pickaxeCraftConfig,
                    userState.PickaxeInventory,
                    userState.PickaxeInventory);
            }
        }
    }
}