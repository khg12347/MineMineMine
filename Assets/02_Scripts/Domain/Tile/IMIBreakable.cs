using UnityEngine;

namespace MI.Domain.Tile
{
    // NOTE: Vector3 사용으로 UnityEngine 의존성 포함. 추후 asmdef 분리 시 별도 처리 필요.
    public interface IMIBreakable
    {
        bool IsBreakable { get; }
        float BounceMultiplier { get; }

        /// <param name="hitPoint">충돌 지점 월드 좌표 (플로팅 텍스트 등 연동용). 기존 호출부는 생략 가능 (기본값 = Vector3.zero)</param>
        EBreakResult TakeDamage(int damage, Vector3 hitPoint = default);
        void Break();
    }
}
