using MI.Core.ServiceLocator;
using MI.Domain.UserState.Inventory;
using MI.Presentation.UI;
using MI.Presentation.UI.Popup.Inventory;
using UnityEngine;

namespace MI.Domain.GameRoot
{
    public class MISceneContext : MonoBehaviour
    {
        [SerializeField] private MIPopupInventory _popupInventory;

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
            _popupInventory.InjectInventory(inventory);
        }

    }
}