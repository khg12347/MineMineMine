using MI.Domain.Tile;

namespace MI.Domain.UserState.Inventory
{
    // 타일 재료(100번대) + 광물(200번대) 통합 아이템 타입 열거형
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

        // 강화석 (300번대)
        EnhanceStone_Low  = 301, // 하급 강화석
        EnhanceStone_Mid  = 302, // 중급 강화석
        EnhanceStone_High = 303, // 상급 강화석
    }

    // ETileType / EMineralType → EItemType 변환
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
