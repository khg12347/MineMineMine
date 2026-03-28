using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MI.Data.Config
{
    /// <summary>
    /// 스테이지 전체 설정을 보관하는 ScriptableObject.
    /// GridWidth, RowHeight, ChunkRows 와 레벨 목록을 가집니다.
    /// </summary>
    [CreateAssetMenu(fileName = "StageConfig", menuName = "MI/Config/Stage")]
    public class MIStageConfig : SerializedScriptableObject
    {
        [Title("그리드 기본 설정")]
        [LabelText("가로 타일 수")]
        [PropertyRange(1, 20)]
        [SerializeField] private int _gridWidth = 8;

        [LabelText("타일 높이 (행 간격)")]
        [InfoBox("타일 1칸의 월드 단위 높이. 기본 0.75 f")]
        [PropertyRange(0.1f, 2f)]
        [SerializeField] private float _rowHeight = 0.75f;

        [LabelText("청크 행 수")]
        [InfoBox("한 번에 알고리즘이 생성할 행 수")]
        [PropertyRange(4, 64)]
        [SerializeField] private int _chunkRows = 16;

        [Title("레벨 목록")]
        [InfoBox("위에서부터 순서대로 적용되며, 마지막 레벨은 이후 깊이에서 반복됩니다.")]
        [SerializeField] private List<MILevelData> _levels = new();

        public int                    GridWidth => _gridWidth;
        public float                  RowHeight  => _rowHeight;
        public int                    ChunkRows  => _chunkRows;
        public IReadOnlyList<MILevelData> Levels => _levels;
    }
}
