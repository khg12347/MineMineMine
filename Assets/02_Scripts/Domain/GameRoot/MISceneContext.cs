using MI.Core.ServiceLocator;
using MI.Data.UIRes;
using MI.Presentation.UI;
using MI.Presentation.UI.HUD.Wallet;
using MI.Presentation.UI.Popup.Inventory;
using UnityEngine;

namespace MI.Domain.GameRoot
{
    public class MISceneContext : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private MIPopupInventory _popupInventory;
        [SerializeField] private MIWalletHUD _walletHUD;

        // UI 리소스가 많아지면 데이터 컨테이너 패턴으로 리팩토링
        [Header("Resources")]
        [SerializeField] private MIUINumberResources _numberResources;
        [SerializeField] private MIItemIconDataTable _itemIconDataTable;
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
            var inventory = MIServiceLocator.Get<User.MIUserState>().Inventory;
            _popupInventory.InjectResources(_numberResources, _itemIconDataTable);
            _popupInventory.InjectInventory(inventory);

            var wallet = MIServiceLocator.Get<User.MIUserState>().Wallet;
            _walletHUD.InjectNumberResources(_numberResources);
            _walletHUD.InjectWallet(wallet);
        }

    }
}