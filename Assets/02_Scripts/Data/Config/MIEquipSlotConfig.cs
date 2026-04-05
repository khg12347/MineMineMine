using MI.Domain.Pickaxe.Equipment;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MI.Data.Config
{
    /// <summary>
    /// 장착 슬롯별 데미지 비율·스킬 사용 여부 설정.
    /// 에셋 위치: Assets/03_Resources/03_Datas/EquipSlotConfig.asset
    /// </summary>
    [CreateAssetMenu(
        fileName = "EquipSlotConfig",
        menuName = "MI/Config/EquipSlotConfig")]
    public class MIEquipSlotConfig : SerializedScriptableObject
    {
        [TableList]
        [SerializeField] private FEquipSlotConfig[] _slotConfigs;

        /// <summary>해당 슬롯의 설정값 반환. 데이터 없으면 기본값 반환.</summary>
        public FEquipSlotConfig GetConfig(EEquipSlot slot)
        {
            if (_slotConfigs != null)
            {
                for (int i = 0; i < _slotConfigs.Length; i++)
                {
                    if (_slotConfigs[i].Slot == slot)
                        return _slotConfigs[i];
                }
            }

            // 기본값: 데미지 100%, 스킬 불가
            return new FEquipSlotConfig
            {
                Slot = slot,
                DamageRatio = 1f,
                CanUseSkill = false,
            };
        }
    }
}
