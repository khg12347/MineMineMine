using System;
using MI.Data.User.Inventory;

namespace MI.Data.Pickaxe.Craft
{
    /// <summary>
    /// 제작 재료 1건. 아이템 타입과 필요 수량.
    /// </summary>
    [Serializable]
    public struct FMaterialCost
    {
        // 재료 아이템 타입
        public EItemType ItemType;

        // 필요 수량
        public int Amount;
    }
}
