using Sirenix.OdinInspector;

using UnityEngine;

namespace MI.Data.Config
{
    /// <summary>
    /// 곡괭이 관련 Config를 묶어 관리하는 DataRegistry. VContainer 등록 단위.
    /// </summary>
    [CreateAssetMenu(
        fileName = "PickaxeDataRegistry",
        menuName = "MI/Config/PickaxeDataRegistry")]
    public class MIPickaxeDataRegistry : SerializedScriptableObject, IMIPickaxeDataRegistry
    {
        [Title("제작 설정")]
        [Required]
        [SerializeField] private MIPickaxeCraftConfig _craftConfig;

        [Title("스펙 테이블")]
        [Required]
        [SerializeField] private MIPickaxeSpecDataTable _specDataTable;

        [Title("강화 설정")]
        [Required]
        [SerializeField] private MIEnhanceConfig _enhanceConfig;

        [Title("강화 비용 설정")]
        [Required]
        [SerializeField] private MIEnhanceCostConfig _enhanceCostConfig;

        public MIPickaxeCraftConfig CraftConfig => _craftConfig;
        public MIPickaxeSpecDataTable SpecDataTable => _specDataTable;
        public MIEnhanceConfig EnhanceConfig => _enhanceConfig;
        public MIEnhanceCostConfig EnhanceCostConfig => _enhanceCostConfig;
    }
}
