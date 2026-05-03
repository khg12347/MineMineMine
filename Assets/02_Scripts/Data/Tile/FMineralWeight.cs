using System;
using MI.Data.Tile;

namespace MI.Data.Tile
{
    /// <summary>
    /// 섹션(레벨)별 광물 생성 가중치 및 밀도 설정.
    /// Weight는 해당 광물이 선택될 확률 가중치,
    /// Density는 선택된 광물의 드랍 수량 범위를 결정합니다 (MIMineralConfig 참조).
    /// </summary>
    [Serializable]
    public struct FMineralWeight
    {
        public EMineralType    MineralType;
        public float           Weight;

        /// <summary>이 광물이 배치될 때 적용할 밀도 단계 (드랍 수량 범위 결정)</summary>
        public EMineralDensity Density;
    }
}
