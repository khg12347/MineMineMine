using System;
using System.Collections.Generic;
using MI.Utility;

namespace MI.Domain.UserState.Inventory
{
    // 플레이어 인벤토리.
    // 드랍한 재료를 누적하여 보관하는 기능 담당.
    // TODO: Save/Load 기능 구현 예정
    public class MIUserInventory : IMIItemDropEventListener
    {
        private readonly Dictionary<EItemType, int> _items = new();

        public event Action<Dictionary<EItemType, int>> OnInventoryUpdated;

        #region Event Listener

        public void Enable() => MIItemDropEvent.Register(this);
        public void Disable() => MIItemDropEvent.Unregister(this);

        public void OnItemDropped(FDropItemData data)
        {
            AddItem(data.ItemType, data.Amount);
        }

        #endregion Event Listener

        #region Public API

        public int GetAmount(EItemType type)
        {
            return _items.TryGetValue(type, out int amount) ? amount : 0;
        }

        public IReadOnlyDictionary<EItemType, int> GetAll() => _items;

        /// <summary>아이템 보유량이 충분한지 확인.</summary>
        public bool HasEnough(EItemType type, int amount) => GetAmount(type) >= amount;

        /// <summary>
        /// 아이템 수량 차감. 보유량 부족 시 false 반환, 차감하지 않음.
        /// </summary>
        public bool TryConsume(EItemType type, int amount)
        {
            if (type == EItemType.None || amount <= 0) return false;

            int current = GetAmount(type);
            if (current < amount) return false;

            _items[type] = current - amount;
            if (_items[type] <= 0) _items.Remove(type);
            OnInventoryUpdated?.Invoke(_items);
            MILog.Log($"[MIUserInventory] {type} -{amount} (총 {GetAmount(type)})");
            return true;
        }

        #endregion Public API

        private void AddItem(EItemType type, int amount)
        {
            if (type == EItemType.None || amount <= 0)
                return;

            if (_items.ContainsKey(type))
            {
                _items[type] += amount;
            }
            else
            {
                _items[type] = amount;
            }
            OnInventoryUpdated?.Invoke(_items);
            //MILog.Log($"[MIUserInventory] {type.ToString()} +{amount} (총 {_items[type]})");
        }

    }
}
