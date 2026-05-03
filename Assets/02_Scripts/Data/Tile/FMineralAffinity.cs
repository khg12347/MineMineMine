using System;
using MI.Data.Tile;

namespace MI.Data.Tile
{
    /// <summary>
    /// 특정 타일 종류에서 특정 광물이 생성될 친화도(보정 가중치).
    /// MILevelData 의 MineralAffinities 딕셔너리 Value 요소로 사용됩니다.
    /// </summary>
    [Serializable]
    public struct FMineralAffinity
    {
        public EMineralType MineralType;
        public float        Weight;
    }
}
