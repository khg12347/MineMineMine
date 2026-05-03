using System;
using MI.Data.Pickaxe;

namespace MI.Data.Pickaxe.Craft
{
    /// <summary>
    /// 곡괭이 1종의 전체 제작 비용.
    /// Materials: 최대 2개, 비어있을 수 있음.
    /// Currencies: 0~1개, 비어있으면 재화 비용 없음 → UI에서 재화 영역 숨김.
    /// </summary>
    [Serializable]
    public struct FPickaxeCraftCost
    {
        public EPickaxeType PickaxeType;

        // 재료 비용 (최대 2개)
        public FMaterialCost[] Materials;

        // 재화 비용 (0~1개, null 또는 빈 배열이면 재화 불필요)
        public FCurrencyCost[] Currencies;
    }
}
