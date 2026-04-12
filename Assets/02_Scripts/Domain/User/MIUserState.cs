using System;
using MI.Domain.Pickaxe.Equipment;
using MI.Domain.UserState.Inventory;
using MI.Domain.UserState.Wallet;

namespace MI.Domain.User
{
    /// <summary>
    /// 플레이어 상태 관리. Non-MonoBehaviour, 비싱글톤.
    /// VContainer에서 생성자 주입으로 MIUserInventory, MIUserWallet, MIPickaxeInventory를 받는다.
    /// </summary>
    public class MIUserState : IDisposable
    {
        public MIUserInventory Inventory { get; private set; }
        public MIUserWallet Wallet { get; private set; }

        /// <summary>곡괭이 보유/장착 관리</summary>
        public MIPickaxeInventory PickaxeInventory { get; private set; }

        public MIUserState(
            MIUserInventory inventory,
            MIUserWallet wallet,
            MIPickaxeInventory pickaxeInventory)
        {
            Inventory = inventory;
            Wallet = wallet;
            PickaxeInventory = pickaxeInventory;
        }

        public void Dispose()
        {
            Inventory.Disable();
            Wallet.Disable();
        }
    }
}
