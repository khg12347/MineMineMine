using System.Collections.Generic;
using MI.Domain.Tile;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace MI.Data.Config
{
    // 레벨 1개의 타일 생성 규칙 정의 ScriptableObject. MIStageConfig.Levels에 등록.
    [CreateAssetMenu(fileName = "LevelData", menuName = "MI/Config/LevelData")]
    public class MILevelData : SerializedScriptableObject
    {
        #region Basic Info

        [FoldoutGroup("기본")]
        [LabelText("레벨 이름 (에디터 식별용)")]
        [SerializeField] private string _levelName = "Level";

        [FoldoutGroup("기본")]
        [LabelText("행 수 (이 레벨이 차지하는 깊이)")]
        [PropertyRange(1, 500)]
        [SerializeField] private int _rowCount = 50;

        #endregion Basic Info

        #region Tile

        [FoldoutGroup("타일")]
        [LabelText("타일 가중치")]
        [TableList]
        [SerializeField] private List<FTileWeight> _tileWeights = new();

        [FoldoutGroup("타일")]
        [LabelText("타일 Config 참조 (타일 종류별 설정)")]
        [SerializeField] private List<MITileConfig> _tileConfigs = new();

        #endregion Tile

        #region Cluster

        [FoldoutGroup("군집")]
        [LabelText("군집 크기 최솟값")]
        [PropertyRange(1, 20)]
        [SerializeField] private int _clusterSizeMin = 2;

        [FoldoutGroup("군집")]
        [LabelText("군집 크기 최댓값")]
        [PropertyRange(1, 40)]
        [SerializeField] private int _clusterSizeMax = 8;

        [FoldoutGroup("군집")]
        [LabelText("시드 밀도 (0.01 ~ 1.0)")]
        [InfoBox("값이 클수록 시드가 많아져 더 잘게 쪼개진 패턴이 생성됩니다.")]
        [PropertyRange(0.01f, 1f)]
        [SerializeField] private float _seedDensity = 0.3f;

        #endregion Cluster

        #region Mineral

        [FoldoutGroup("광물")]
        [LabelText("광물 가중치 (섹션 기준)")]
        [TableList]
        [SerializeField] private List<FMineralWeight> _mineralWeights = new();

        [FoldoutGroup("광물")]
        [LabelText("타일별 광물 친화도")]
        [InfoBox("Key: ETileType, Value: 해당 타일에서 각 광물이 생성될 친화도 목록")]
        [OdinSerialize]
        [DictionaryDrawerSettings(KeyLabel = "타일 타입", ValueLabel = "광물 친화도 목록")]
        private Dictionary<ETileType, List<FMineralAffinity>> _mineralAffinities = new();

        #endregion Mineral

        #region Treasure

        [FoldoutGroup("보물 상자")]
        [LabelText("보물 상자 생성 확률 (셀당)")]
        [PropertyRange(0f, 1f)]
        [SerializeField] private float _treasureChance = 0.005f;

        [FoldoutGroup("보물 상자")]
        [LabelText("보물 상자 가중치")]
        [TableList]
        [SerializeField] private List<FTreasureWeight> _treasureWeights = new();

        [FoldoutGroup("보물 상자")]
        [LabelText("청크당 최대 보물 수")]
        [PropertyRange(0, 10)]
        [SerializeField] private int _maxTreasuresPerChunk = 2;

        #endregion Treasure

        #region Blending

        [FoldoutGroup("블렌딩")]
        [LabelText("다음 레벨과의 블렌딩 행 수")]
        [InfoBox("이 레벨 끝에서 N행 동안 다음 레벨 가중치와 선형 보간합니다.")]
        [PropertyRange(0, 20)]
        [SerializeField] private int _blendRows = 5;

        #endregion Blending

        #region Properties

        public string                LevelName            => _levelName;
        public int                   RowCount             => _rowCount;
        public IReadOnlyList<FTileWeight>    TileWeights          => _tileWeights;
        public IReadOnlyList<MITileConfig>   TileConfigs          => _tileConfigs;
        public int                   ClusterSizeMin       => _clusterSizeMin;
        public int                   ClusterSizeMax       => _clusterSizeMax;
        public float                 SeedDensity          => _seedDensity;
        public IReadOnlyList<FMineralWeight> MineralWeights       => _mineralWeights;
        public Dictionary<ETileType, List<FMineralAffinity>> MineralAffinities => _mineralAffinities;
        public float                 TreasureChance       => _treasureChance;
        public IReadOnlyList<FTreasureWeight> TreasureWeights     => _treasureWeights;
        public int                   MaxTreasuresPerChunk => _maxTreasuresPerChunk;
        public int                   BlendRows            => _blendRows;

        #endregion Properties
    }
}
