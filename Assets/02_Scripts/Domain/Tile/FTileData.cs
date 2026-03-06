namespace MI.Domain.Tile
{
    public struct FTileData
    {
        public ETileType TileType;
        public int MaxDurability;
        public int CurrentDurability;
        public int DropScore;

        public bool IsDestroyed => CurrentDurability <= 0;

        public EBreakResult ApplyDamage(int damage)
        {
            CurrentDurability -= damage;
            return IsDestroyed ? EBreakResult.Destroyed : EBreakResult.Damaged;
        }
    }
}
