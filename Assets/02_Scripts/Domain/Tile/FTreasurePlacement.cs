using System;
using MI.Data.Tile;

namespace MI.Domain.Tile
{
    /// <summary>
    /// 청크 내 보물 상자 배치 정보.
    /// X, Y 는 청크 내 로컬 열/행 인덱스입니다.
    /// Width/Height 는 2×2 확장 대비 예약 필드이며, 현재는 항상 1입니다.
    /// </summary>
    [Serializable]
    public struct FTreasurePlacement
    {
        public int          X;
        public int          Y;
        public ETreasureType Type;
        public int          Width;
        public int          Height;
    }
}
