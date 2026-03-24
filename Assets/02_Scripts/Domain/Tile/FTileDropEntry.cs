using System;

namespace MI.Domain.Tile
{
    /// <summary>
    /// 타일 파괴 시 드랍되는 타일 재료 정보.
    /// 모든 타일에 존재하며, 타일 종류와 드랍 수량 범위를 정의합니다.
    /// </summary>
    [Serializable]
    public struct FTileDropEntry
    {
        /// <summary>드랍할 타일 재료 종류</summary>
        public ETileType TileType;

        /// <summary>최소 드랍 수량</summary>
        public int MinAmount;

        /// <summary>최대 드랍 수량</summary>
        public int MaxAmount;
    }
}
