namespace MI.Domain.Tile
{
    public struct FTileData
    {
        public ETileType TileType;
        public int MaxDurability;
        public int CurrentDurability;
        public int DropScore;
        public int DropExp;                // 파괴 시 플레이어에게 지급할 EXP
        public float BounceMultiplier;     // 타일별 바운스 배율 (Dirt=0.8, Stone=1.0, Iron=1.2, Gold=1.5, Diamond=2.0)

        public bool IsDestroyed => CurrentDurability <= 0;

        public EBreakResult ApplyDamage(int damage)
        {
            CurrentDurability -= damage;
            return IsDestroyed ? EBreakResult.Destroyed : EBreakResult.Damaged;
        }
    }
}
