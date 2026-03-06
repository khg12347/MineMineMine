namespace MI.Domain.Tile
{
    public interface IMIBreakable
    {
        bool IsBreakable { get; }
        EBreakResult TakeDamage(int damage);
        void Break();
    }
}
