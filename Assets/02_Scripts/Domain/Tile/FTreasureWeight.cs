using System;

namespace MI.Domain.Tile
{
    /// <summary>보물 상자 등급별 생성 가중치</summary>
    [Serializable]
    public struct FTreasureWeight
    {
        public ETreasureType TreasureType;
        public float         Weight;
    }
}
