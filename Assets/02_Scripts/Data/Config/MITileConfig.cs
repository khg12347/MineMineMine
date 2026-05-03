using MI.Data.Tile;
using MI.Domain.Tile;
using MI.Untility;
using MI.Utility;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace MI.Data.Config
{
    [CreateAssetMenu(fileName = "TileConfig", menuName = "MI/Config/Tile")]
    public class MITileConfig : SerializedScriptableObject
    {
        [Title("타일 기본 설정")]
        [SerializeField] private ETileType _tileType;

        [LabelText("내구도")]
        [SerializeField] private int _maxDurability = 1;

        [LabelText("점수")]
        [SerializeField] private int _dropScore = 10;

        [LabelText("획득 EXP")]
        [InfoBox("타일 파괴 시 플레이어에게 지급할 EXP")]
        [PropertyRange(0, 9999)]
        [SerializeField] private int _dropExp = 10;

        [LabelText("균열 단계별 내구도")]
        [SerializeField] private List<int> _crackLevelDurability = new List<int>();

        [Title("바운스 배율")]
        [InfoBox("Dirt=0.8 / Stone=1.0 / Iron=1.2 / Gold=1.5 / Diamond=2.0")]
        [PropertyRange(0.5f, 3f)]
        [SerializeField] private float _bounceMultiplier = 1f;

        [Title("타일 재료 드랍")]
        [InfoBox("타일 파괴 시 드랍되는 재료 수량 범위입니다.")]
        [LabelText("드랍 수량 범위 (Min / Max)")]
        [SerializeField] private MIIntRange _tileDropAmountRange = new MIIntRange(1, 3);

        [Title("골드 드랍")]
        [InfoBox("타일 파괴 시 드랍되는 골드 수량 범위입니다. 0이면 드랍하지 않습니다.")]
        [LabelText("골드 드랍 수량 범위 (Min / Max)")]
        [SerializeField] private MIIntRange _goldDropAmountRange = new MIIntRange(0, 0);

        [Title("스프라이트")]
        [PreviewField(50)]
        [LabelText("Base 스프라이트")]
        [SerializeField] private Sprite _baseSprite;

        [LabelText("광물 밀도별 타일 스프라이트 오버레이")]
        [InfoBox("광물이 매장된 타일에 사용할 스프라이트. Key: EMineralDensity")]
        [DictionaryDrawerSettings(KeyLabel = "광물 밀도", ValueLabel = "타일 스프라이트")]
        [SerializeField] private Dictionary<EMineralDensity, Sprite> _mineralSlotSprites = new Dictionary<EMineralDensity, Sprite>();

        [LabelText("균열 단계별 스프라이트")]
        [SerializeField] private List<Sprite> _crackLevelSprites = new List<Sprite>();

        [Title("파괴 시 효과")]
        [LabelText("타일 파편 프리팹")]
        [SerializeField] private GameObject _prefabFxTileDebris;
        // 사운드 필요

        public ETileType TileType          => _tileType;
        public Sprite    BaseSprite        => _baseSprite;
        public GameObject PrefabFxTileDebris => _prefabFxTileDebris;

        /// <summary>
        /// 광물 밀도에 따라 타일 위에 표시할 스프라이트를 반환합니다.
        /// 설정이 없으면 BaseSprite를 반환합니다.
        /// </summary>
        public Sprite GetMineralSlotSprite(EMineralDensity mineralDensity)
        {
            if (_mineralSlotSprites.TryGetValue(mineralDensity, out Sprite sprite))
                return sprite;

            MILog.LogWarning($"[MITileConfig] 광물 밀도 {mineralDensity}에 대한 스프라이트가 설정되지 않았습니다.");
            return _baseSprite;
        }
        public Sprite GetCrackLevelSprite(int crackLevel)
        {
            if (crackLevel < 0 || crackLevel >= _crackLevelSprites.Count)
            {
                MILog.LogWarning($"[MITileConfig] 균열 단계 {crackLevel}에 대한 스프라이트가 설정되지 않았습니다.");
                return null;
            }
            return _crackLevelSprites[crackLevel];
        }

        /// <summary>
        /// 타일 Config로부터 FTileData를 생성합니다.
        /// 광물 정보(MineralDrop)는 생성 시점에 null로 초기화되며,
        /// MIFloodFillAlgorithm의 Phase 3에서 별도로 세팅됩니다.
        /// </summary>
        public FTileData CreateTileData()
        {
            return new FTileData
            {
                TileType             = _tileType,
                MaxDurability        = _maxDurability,
                CurrentDurability    = _maxDurability,
                DropScore            = _dropScore,
                DropExp              = _dropExp,
                BounceMultiplier     = _bounceMultiplier,
                CrackLevelDurability = new List<int>(_crackLevelDurability),
                MineralDrop          = null, // Phase 3(ApplyMinerals)에서 세팅
                TileDrop             = new FTileDropEntry
                {
                    TileType  = _tileType,
                    MinAmount = _tileDropAmountRange?.Min ?? 1,
                    MaxAmount = _tileDropAmountRange?.Max ?? 1,
                },
                GoldDropMin          = _goldDropAmountRange?.Min ?? 0,
                GoldDropMax          = _goldDropAmountRange?.Max ?? 0,
            };
        }
    }
}
