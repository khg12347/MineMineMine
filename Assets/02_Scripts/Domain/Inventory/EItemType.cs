using MI.Domain.Tile;

namespace MI.Domain.Inventory
{
    /// <summary>
    /// 타일 재료와 광물을 통합하는 아이템 타입 열거형.
    /// 대역 분리: 타일 재료 = 100번대, 광물 = 200번대.
    /// </summary>
    public enum EItemType
    {
        None = 0,
        // 타일 재료 (ETileType 값 + 100 % 10)
        Soil  = 110,
        Wood  = 120,
        Stone = 130,

        // 광물 (EMineralType 값 + 200)
        Iron   = 201,
        Copper = 202,
        Silver = 203,
        Gold   = 204,
    }

    /// <summary>
    /// ETileType / EMineralType → EItemType 변환 유틸리티.
    /// </summary>
    public static class MIItemTypeConverter
    {
        private const int TILE_OFFSET    = 100;
        private const int MINERAL_OFFSET = 200;

        public static EItemType FromTileType(ETileType type)
            => (EItemType)(((int)type / 10 * 10) + TILE_OFFSET);

        public static EItemType FromMineralType(EMineralType type)
            => (EItemType)((int)type + MINERAL_OFFSET);
    }
}
