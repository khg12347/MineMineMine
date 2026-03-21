using UnityEngine;

namespace MI.Core
{
    /// <summary>
    /// 카메라 추적기의 계약 인터페이스.
    /// 구현체(MICameraFollower)는 Presentation 레이어에 위치합니다.
    /// </summary>
    public interface IMICameraFollower
    {
        /// <summary>카메라와 추적 속도를 초기화합니다.</summary>
        void Initialize(Camera camera, float followSpeed);

        /// <summary>타깃 Y 좌표를 기준으로 카메라를 추적합니다.</summary>
        void Follow(float targetY);
    }
}
