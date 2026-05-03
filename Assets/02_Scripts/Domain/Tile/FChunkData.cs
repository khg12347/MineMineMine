using System.Collections.Generic;

using MI.Data.Tile;

namespace MI.Domain.Tile
{
    /// <summary>
    /// 알고리즘이 생성한 청크 1개의 데이터.
    /// Cells[행, 열] 형태의 2D 배열로 FTileData 를 보유합니다.
    /// StartRow 는 이 청크의 첫 번째 행의 절대 행 인덱스입니다.
    /// </summary>
    public class FChunkData
    {
        public FTileData[,]            Cells;
        public List<FTreasurePlacement> Treasures;
        public int                     StartRow;
    }
}
