using MI.Data.Config;
using MI.Domain.Tile;

namespace MI.Domain.Stage
{
    /// <summary>
    /// 타일 청크 생성 알고리즘의 계약 인터페이스.
    /// MonoBehaviour 의존 없이 순수 C# 으로 구현합니다.
    /// </summary>
    public interface IMITileAlgorithm
    {
        /// <summary>
        /// 지정된 시작 행부터 chunkRows 행만큼의 타일 데이터를 생성합니다.
        /// </summary>
        /// <param name="startRow">청크 첫 번째 행의 절대 행 인덱스</param>
        /// <param name="chunkRows">생성할 행 수</param>
        /// <param name="gridWidth">가로 타일 수</param>
        /// <param name="config">스테이지 전체 설정 (레벨 정보 포함)</param>
        FChunkData Generate(int startRow, int chunkRows, int gridWidth, MIStageConfig config);
    }
}
