namespace MI.Domain.Pickaxe.Enhance
{
    /// <summary>
    /// 확률 판정 추상화.
    /// 테스트 시 결정론적 구현체를 주입하여 강화 성공/실패를 제어할 수 있다.
    /// </summary>
    public interface IMIRandomProvider
    {
        /// <summary>0.0 이상 1.0 미만의 랜덤 값을 반환한다.</summary>
        float NextFloat();
    }
}
