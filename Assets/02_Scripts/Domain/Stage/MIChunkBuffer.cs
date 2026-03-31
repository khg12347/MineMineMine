using System.Collections.Generic;
using MI.Domain.Tile;

namespace MI.Domain.Stage
{
    // 알고리즘 생성 청크를 큐로 관리, MITileSpawner에 행 단위로 제공
    public class MIChunkBuffer
    {
        private readonly Queue<FChunkData> _chunks = new();

        // 현재 행 단위로 소비 중인 청크
        private FChunkData _activeChunk;
        private int        _activeChunkRow; // 현재 청크 내 다음 소비할 행 인덱스

        // 생성 완료된 최대 절대 행 인덱스 (exclusive)
        public int GeneratedUpToRow { get; private set; }

        #region Chunk Management

        // 청크를 큐에 추가
        public void Enqueue(FChunkData chunk)
        {
            if (chunk == null) return;
            _chunks.Enqueue(chunk);
            int chunkEnd = chunk.StartRow + chunk.Cells.GetLength(0);
            if (chunkEnd > GeneratedUpToRow)
                GeneratedUpToRow = chunkEnd;
        }

        // 지정 절대 행의 데이터를 소비. 순서 불일치 시 false 반환
        public bool TryDequeueRow(int row, out FTileData[] rowData, out List<FTreasurePlacement> treasures)
        {
            rowData   = null;
            treasures = null;

            // 활성 청크가 소진되었으면 다음 청크로 전환
            while (_activeChunk == null || _activeChunkRow >= _activeChunk.Cells.GetLength(0))
            {
                if (_chunks.Count == 0) return false;
                _activeChunk    = _chunks.Dequeue();
                _activeChunkRow = 0;
            }

            // 요청된 행이 현재 활성 청크의 다음 행과 일치하는지 확인
            int expectedRow = _activeChunk.StartRow + _activeChunkRow;
            if (row != expectedRow) return false;

            // 행 데이터 추출
            int cols = _activeChunk.Cells.GetLength(1);
            rowData = new FTileData[cols];
            for (int c = 0; c < cols; c++)
                rowData[c] = _activeChunk.Cells[_activeChunkRow, c];

            // 해당 행에 속하는 보물 추출
            treasures = new List<FTreasurePlacement>();
            if (_activeChunk.Treasures != null)
                foreach (var t in _activeChunk.Treasures)
                    if (t.Y == _activeChunkRow) treasures.Add(t);

            _activeChunkRow++;
            return true;
        }

        // 선행 생성이 부족하면 true 반환
        public bool NeedsMoreChunks(int currentDepthRow, int spawnAheadRows)
        {
            return GeneratedUpToRow <= currentDepthRow + spawnAheadRows;
        }

        #endregion Chunk Management
    }
}
