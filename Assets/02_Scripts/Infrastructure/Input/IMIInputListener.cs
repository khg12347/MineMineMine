using UnityEngine;

namespace MI.Infrastructure.Input
{
    /// <summary>
    /// <see cref="MIInputHandler"/>로부터 입력 의도(Intent)를 전달받는 리스너 계약.
    ///
    /// 구현체는 입력 장치의 종류(마우스/터치)를 알 필요 없이
    /// "무엇을, 어디서" 입력했는지만 전달받습니다.
    ///
    /// 등록 방법:
    ///   _inputHandler.RegisterListener(this);
    /// 해제 방법:
    ///   _inputHandler.UnregisterListener(this);
    /// </summary>
    public interface IMIInputListener
    {
        /// <summary>
        /// 탭 또는 클릭이 완료됐을 때 호출됩니다.
        /// </summary>
        /// <param name="screenPosition">스크린 좌표계 기준 입력 위치 (pixels)</param>
        void OnTap(Vector2 screenPosition);
    }
}
