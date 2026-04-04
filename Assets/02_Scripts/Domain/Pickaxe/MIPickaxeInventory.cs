using System;
using System.Collections.Generic;

namespace MI.Domain.Pickaxe
{
    /// <summary>
    /// 곡괭이 보유 목록 + 장착 슬롯 관리.
    /// IMIPickaxeInventory (조회), IMIPickaxeEquipment (명령) 동시 구현.
    /// MIUserState가 소유한다.
    /// </summary>
    public class MIPickaxeInventory : IMIPickaxeInventory, IMIPickaxeEquipment
    {
        private readonly Dictionary<EPickaxeType, FPickaxeInstance> _owned = new();

        private readonly Dictionary<EEquipSlot, EPickaxeType> _equipped = new()
        {
            { EEquipSlot.Main, EPickaxeType.None },
            { EEquipSlot.Sub1, EPickaxeType.None },
            { EEquipSlot.Sub2, EPickaxeType.None },
        };

        #region Events

        /// <summary>곡괭이 획득 시 발행</summary>
        public event Action<EPickaxeType> OnPickaxeAdded;

        /// <summary>장착 슬롯 변경 시 발행 (슬롯, 이전 곡괭이, 새 곡괭이)</summary>
        public event Action<EEquipSlot, EPickaxeType, EPickaxeType> OnEquipChanged;

        #endregion Events

        #region IMIPickaxeInventory — Query Owned

        /// <inheritdoc/>
        public bool IsOwned(EPickaxeType type) => _owned.ContainsKey(type);

        /// <inheritdoc/>
        public FPickaxeInstance? GetInstance(EPickaxeType type)
        {
            if (_owned.TryGetValue(type, out var instance)) return instance;
            return null;
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<EPickaxeType> OwnedPickaxes => _owned.Keys;

        #endregion IMIPickaxeInventory — Query Owned

        #region IMIPickaxeInventory — Query Equipment

        /// <inheritdoc/>
        public EPickaxeType GetEquipped(EEquipSlot slot) => _equipped[slot];

        /// <inheritdoc/>
        public EEquipSlot? GetEquippedSlot(EPickaxeType type)
        {
            foreach (var kvp in _equipped)
            {
                if (kvp.Value == type) return kvp.Key;
            }
            return null;
        }

        #endregion IMIPickaxeInventory — Query Equipment

        #region IMIPickaxeEquipment — Commands

        /// <inheritdoc/>
        public bool Equip(EPickaxeType type, EEquipSlot targetSlot)
        {
            if (type == EPickaxeType.None) return false;
            if (!IsOwned(type)) return false;

            // 이미 다른 슬롯에 장착되어 있으면 기존 슬롯 해제
            var currentSlot = GetEquippedSlot(type);
            if (currentSlot.HasValue && currentSlot.Value != targetSlot)
            {
                var prevInCurrent = _equipped[currentSlot.Value];
                _equipped[currentSlot.Value] = EPickaxeType.None;
                OnEquipChanged?.Invoke(currentSlot.Value, prevInCurrent, EPickaxeType.None);
            }

            // 대상 슬롯의 기존 곡괭이 기록
            var prevInTarget = _equipped[targetSlot];

            // 장착
            _equipped[targetSlot] = type;
            OnEquipChanged?.Invoke(targetSlot, prevInTarget, type);

            return true;
        }

        /// <inheritdoc/>
        public void Unequip(EEquipSlot slot)
        {
            var prev = _equipped[slot];
            if (prev == EPickaxeType.None) return;

            _equipped[slot] = EPickaxeType.None;
            OnEquipChanged?.Invoke(slot, prev, EPickaxeType.None);
        }

        #endregion IMIPickaxeEquipment — Commands

        #region Internal — Add Pickaxe

        /// <summary>
        /// 곡괭이 보유 등록. 제작 완료 시 MIPickaxeCraftService에서 호출.
        /// 이미 보유 중이면 false 반환.
        /// </summary>
        public bool AddPickaxe(FPickaxeInstance instance)
        {
            if (instance.PickaxeType == EPickaxeType.None) return false;
            if (_owned.ContainsKey(instance.PickaxeType)) return false;

            _owned[instance.PickaxeType] = instance;
            OnPickaxeAdded?.Invoke(instance.PickaxeType);
            return true;
        }

        #endregion Internal — Add Pickaxe
    }
}
