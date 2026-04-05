using System;
using System.Collections.Generic;
using MI.Domain.Pickaxe;

namespace MI.Domain.Pickaxe.Equipment
{
    /// <summary>
    /// 곡괭이 보유/장착 상태 조회 전용 인터페이스. (ISP — 조회와 명령 분리)
    /// </summary>
    public interface IMIPickaxeInventory
    {
        /// <summary>해당 곡괭이 보유 여부</summary>
        bool IsOwned(EPickaxeType type);

        /// <summary>해당 곡괭이의 인스턴스 반환. 미보유 시 null.</summary>
        FPickaxeInstance? GetInstance(EPickaxeType type);

        /// <summary>보유 곡괭이 목록 (읽기 전용)</summary>
        IReadOnlyCollection<EPickaxeType> OwnedPickaxes { get; }

        /// <summary>해당 슬롯에 장착된 곡괭이 반환. 없으면 None.</summary>
        EPickaxeType GetEquipped(EEquipSlot slot);

        /// <summary>해당 곡괭이가 장착된 슬롯 반환. 미장착이면 null.</summary>
        EEquipSlot? GetEquippedSlot(EPickaxeType type);

        /// <summary>곡괭이 획득 시 발행</summary>
        event Action<EPickaxeType> OnPickaxeAdded;

        /// <summary>장착 슬롯 변경 시 발행 (슬롯, 이전 곡괭이, 새 곡괭이)</summary>
        event Action<EEquipSlot, EPickaxeType, EPickaxeType> OnEquipChanged;
    }
}
