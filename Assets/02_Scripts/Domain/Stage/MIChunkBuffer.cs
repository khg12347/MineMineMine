using System.Collections.Generic;
using MI.Domain.Tile;

namespace MI.Domain.Stage
{
    /// <summary>
    /// 알고리즘이 생성한 FChunkData 를 큐로 관리하고,
    /// MITileSpawner 에 행 단위로 데이터를 제공하는 버퍼 클래스.
    ///
    /// 내부적으로 현재 소비 중인 청크와 행 인덱스를 추적합니다.
    /// </summary>
    public class MIChunkBuffer
    {
        private readonly Queue<FChunkData> _chunks = new();

        // 현재 행 단위로 소비 중인 청크
        private FChunkData _activeChunk;
        private int        _activeChunkRow; // 현재 청크 내 다음 소비할 행 인덱스

        /// <summary>데이터가 생성된 최대 절대 행 인덱스 (exclusive)</summary>
        public int GeneratedUpToRow { get; private set; }

        // ── 청크 관리 ────────────────────────────────────────────────────

        /// <summary>청크를 큐 끝에 추가합니다.</summary>
        public void Enqueue(FChunkData chunk)
        {
            if (chunk == null) return;
            _chunks.Enqueue(chunk);
            int chunkEnd = chunk.StartRow + chunk.Cells.GetLength(0);
            if (chunkEnd > GeneratedUpToRow)
                GeneratedUpToRow = chunkEnd;
        }

        /// <summary>
        /// 지정된 절대 행 인덱스에 해당하는 행 데이터를 소비합니다.
        /// 행이 순서대로 요청되지 않으면 false 를 반환합니다.
        /// </summary>
        /// <param name="row">소비할 절대 행 인덱스</param>
        /// <param name="rowData">출력: 해당 행의 FTileData 배열 (열 순서)</param>
        /// <param name="treasures">출력: 해당 행에 배치된 보물 목록</param>
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

        /// <summary>
        /// 더 많은 청크를 생성해야 하는지 판단합니다.
        /// 생성된 데이터가 (currentDepthRow + spawnAheadRows) 보다 적으면 true 를 반환합니다.
        /// </summary>
        public bool NeedsMoreChunks(int currentDepthRow, int spawnAheadRows)
        {
            return GeneratedUpToRow <= currentDepthRow + spawnAheadRows;
        }
    }
}
