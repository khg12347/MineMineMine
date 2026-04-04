using MI.Domain.Pickaxe;
using MI.Domain.UserState.Inventory;
using MI.Domain.UserState.Wallet;

namespace MI.Domain.User
{
    /// <summary>
    /// 플레이어 상태 관리. Non-MonoBehaviour, 비싱글톤.
    /// MIGameRoot에서 직접 생성되며 MIUserInventory, MIUserWallet, MIPickaxeInventory를 소유합니다.
    /// </summary>
    public class MIUserState
    {
        public MIUserInventory Inventory { get; private set; }
        public MIUserWallet Wallet { get; private set; }

        /// <summary>곡괭이 보유/장착 관리</summary>
        public MIPickaxeInventory PickaxeInventory { get; private set; }

        public MIUserState()
        {
            Inventory = new MIUserInventory();
            Inventory.Enable();

            Wallet = new MIUserWallet();
            Wallet.Enable();

            PickaxeInventory = new MIPickaxeInventory();
        }

        public void Dispose()
        {
            Inventory.Disable();
            Wallet.Disable();
        }
    }
}
