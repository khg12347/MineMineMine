using System;

namespace MI.Domain.Pickaxe.Equipment
{
    /// <summary>
    /// 장착 슬롯별 설정값.
    /// Main: DamageRatio=1.0, CanUseSkill=true
    /// Sub1/Sub2: DamageRatio=SO에서 설정 (예: 0.5), CanUseSkill=false
    /// </summary>
    [Serializable]
    public struct FEquipSlotConfig
    {
        public EEquipSlot Slot;

        /// <summary>데미지 배율 (1.0 = 100%)</summary>
        public float DamageRatio;

        /// <summary>스킬 사용 가능 여부 (Main만 true 예정)</summary>
        public bool CanUseSkill;
    }
}
