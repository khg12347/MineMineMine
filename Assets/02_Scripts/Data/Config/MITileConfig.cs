using MI.Domain.Tile;
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

        [Title("바운스 배율")]
        [InfoBox("Dirt=0.8 / Stone=1.0 / Iron=1.2 / Gold=1.5 / Diamond=2.0")]
        [PropertyRange(0.5f, 3f)]
        [SerializeField] private float _bounceMultiplier = 1f;

        [Title("스프라이트")]
        [PreviewField(50)]
        [LabelText("Base 스프라이트")]
        [SerializeField] private Sprite _baseSprite;

        [LabelText("광물별로 달라지는 타일 스프라이트")]
        [DictionaryDrawerSettings(KeyLabel = "광물 타입", ValueLabel = "타일 스프라이트")]
        [SerializeField] private Dictionary<EMineralDensity, Sprite> _mineralSlotSprites = new Dictionary<EMineralDensity, Sprite>();

        [LabelText("균열 단계별 내구도")]
        [SerializeField] private List<int> _crackLevelDurability = new List<int>();

        [Title("파괴 시 효과")]
        [LabelText("타일 파편 프리팹")]
        [SerializeField] private GameObject _prefabFxTileDebris;
        //사운드 필요

        public ETileType TileType => _tileType;
        public Sprite BaseSprite => _baseSprite;
        public GameObject PrefabFxTileDebris => _prefabFxTileDebris;

        public Sprite GetMinenralSlotSprites(EMineralDensity mineralDensity)
        {
            if (_mineralSlotSprites.TryGetValue(mineralDensity, out Sprite sprite))
            {
                return sprite;
            }
            else
            {
                Debug.LogWarning($"광물 타입 {mineralDensity}에 대한 스프라이트가 설정되지 않았습니다.");
                return _baseSprite; // 또는 기본 스프라이트 반환
            }
        }

        public FTileData CreateTileData()
        {
            return new FTileData
            {
                TileType          = _tileType,
                MaxDurability     = _maxDurability,
                CurrentDurability = _maxDurability,
                DropScore         = _dropScore,
                DropExp           = _dropExp,
                BounceMultiplier  = _bounceMultiplier,
                CrackLevelDurability = new List<int>(_crackLevelDurability)
            };
        }
    }
}
