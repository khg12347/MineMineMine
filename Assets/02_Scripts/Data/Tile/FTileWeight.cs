using System;
using MI.Data.Tile;

namespace MI.Data.Tile
{
    /// <summary>타일 종류별 생성 가중치</summary>
    [Serializable]
    public struct FTileWeight
    {
        public ETileType TileType;
        public float     Weight;
    }
}
