using System;
using MI.Untility;

namespace MI.Domain.Tile
{
    /// <summary>
    /// EMineralDensity 단계별 드랍 수량 범위 정의.
    /// MIMineralConfig ScriptableObject에서 설정하며,
    /// 타일 파괴 시 실제 드랍 수량을 결정하는 데 사용됩니다.
    /// </summary>
    [Serializable]
    public struct FMineralDensityRange
    {
        /// <summary>적용 대상 밀도 단계</summary>
        public EMineralDensity Density;

        /// <summary>드랍 수량 범위 (Min ~ Max)</summary>
        public MIIntRange DropRange;
    }
}
