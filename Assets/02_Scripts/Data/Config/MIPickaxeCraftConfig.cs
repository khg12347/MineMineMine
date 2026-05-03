using MI.Data.Pickaxe;
using MI.Data.Pickaxe.Craft;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MI.Data.Config
{
    /// <summary>
    /// 곡괭이 제작 비용 데이터 테이블.
    /// </summary>
    [CreateAssetMenu(
        fileName = "PickaxeCraftConfig",
        menuName = "MI/Config/PickaxeCraftConfig")]
    public class MIPickaxeCraftConfig : SerializedScriptableObject
    {
        [TableList]
        [SerializeField] private FPickaxeCraftCost[] _craftCosts;

        /// <summary>해당 곡괭이의 제작 비용 반환. 없으면 null.</summary>
        public FPickaxeCraftCost? GetCost(EPickaxeType type)
        {
            if (_craftCosts == null) return null;

            for (int i = 0; i < _craftCosts.Length; i++)
            {
                if (_craftCosts[i].PickaxeType == type)
                    return _craftCosts[i];
            }
            return null;
        }
    }
}
