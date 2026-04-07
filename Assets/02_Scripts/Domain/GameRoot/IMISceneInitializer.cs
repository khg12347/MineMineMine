namespace MI.Domain.GameRoot
{
    /// <summary>
    /// 씬 초기화 인터페이스. Domain → Presentation 역방향 의존을 제거하기 위한 추상화.
    /// Presentation 레이어의 MISceneContext가 구현한다.
    /// </summary>
    public interface IMISceneInitializer
    {
        /// <summary>씬 컨텍스트 초기화. UI/월드 컴포넌트에 의존성을 주입한다.</summary>
        void InitializeSceneContext();
    }
}
