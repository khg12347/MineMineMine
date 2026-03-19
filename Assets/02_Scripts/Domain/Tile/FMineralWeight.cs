using System;

namespace MI.Domain.Tile
{
    /// <summary>섹션(레벨)별 광물 생성 가중치 및 매장량 범위</summary>
    [Serializable]
    public struct FMineralWeight
    {
        public EMineralType MineralType;
        public float        Weight;
        public int          DepositMin;
        public int          DepositMax;
    }
}
