namespace MI.Domain.Tile
{
    public enum EBreakResult
    {
        Damaged = 0,    // 내구도 감소만 발생
        Destroyed = 1,   // 타일 완전 파괴
        DestroyWithOneHit = 2 // 한 방에 파괴 ("처형"이라는 옵션 일괄 적용 목적)
    }
}
