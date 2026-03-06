namespace MI.Domain.Tile
{
    public interface IMIBreakable
    {
        bool IsBreakable { get; }
        float BounceMultiplier { get; }
        EBreakResult TakeDamage(int damage);
        void Break();
    }
}
