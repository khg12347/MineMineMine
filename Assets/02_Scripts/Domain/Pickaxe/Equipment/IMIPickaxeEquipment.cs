using MI.Domain.Pickaxe;

namespace MI.Domain.Pickaxe.Equipment
{
    /// <summary>
    /// 곡괭이 장착/해제 명령 인터페이스. (ISP — 조회 IMIPickaxeInventory와 분리)
    /// </summary>
    public interface IMIPickaxeEquipment
    {
        /// <summary>
        /// 곡괭이를 슬롯에 장착.
        /// 미보유 곡괭이는 장착 불가 → false 반환.
        /// 이미 다른 슬롯에 장착되어 있으면 기존 슬롯 해제 후 새 슬롯에 장착.
        /// </summary>
        bool Equip(EPickaxeType type, EEquipSlot slot);

        /// <summary>슬롯 장착 해제</summary>
        void Unequip(EEquipSlot slot);
    }
}
