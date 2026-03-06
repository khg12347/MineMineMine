using MI.Domain.Tile;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace MI.Data.Config
{
    [CreateAssetMenu(fileName = "TileConfig", menuName = "MI/Config/Tile")]
    public class MITileConfig : SerializedScriptableObject
    {
        [Title("타일 기본 설정")]
        [EnumToggleButtons]
        [SerializeField] private ETileType _tileType;

        [PropertyRange(1, 10)]
        [SerializeField] private int _maxDurability = 1;

        [SerializeField] private int _dropScore = 10;

        [Title("바운스 배율")]
        [InfoBox("Dirt=0.8 / Stone=1.0 / Iron=1.2 / Gold=1.5 / Diamond=2.0")]
        [PropertyRange(0.5f, 3f)]
        [SerializeField] private float _bounceMultiplier = 1f;

        [Title("스프라이트")]
        [PreviewField(50)]
        [SerializeField] private Sprite _baseSprite;

        [SerializeField] private Sprite[] _damageSprites;

        public ETileType TileType => _tileType;
        public Sprite BaseSprite => _baseSprite;
        public Sprite[] DamageSprites => _damageSprites;

        public FTileData CreateTileData()
        {
            return new FTileData
            {
                TileType = _tileType,
                MaxDurability = _maxDurability,
                CurrentDurability = _maxDurability,
                DropScore = _dropScore,
                BounceMultiplier = _bounceMultiplier
            };
        }
    }
}
