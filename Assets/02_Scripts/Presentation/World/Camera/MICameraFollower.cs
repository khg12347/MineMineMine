using MI.Core;
using UnityEngine;

namespace MI.Presentation.World.Camera
{
    using Camera = UnityEngine.Camera;
    /// <summary>
    /// IMICameraFollower 의 MonoBehaviour 구현체.
    /// 씬의 임의 GameObject 에 배치하고, MIStageManager 의 Inspector 에서 참조합니다.
    ///
    /// 특성:
    ///   - 곡괭이가 더 아래로 내려갈 때만 카메라 타깃을 갱신 (위로 튀어도 카메라는 유지)
    ///   - Mathf.Lerp 로 부드러운 추적
    /// </summary>
    public class MICameraFollower : MonoBehaviour, IMICameraFollower
    {
        private Camera _camera;
        private float _followSpeed;
        private float _cameraTargetY;
        private bool _initialized;

        // ── IMICameraFollower 구현 ────────────────────────────────────────

        public void Initialize(Camera camera, float followSpeed)
        {
            _camera = camera;
            _followSpeed = followSpeed;
            _cameraTargetY = camera.transform.position.y;
            _initialized = true;
        }

        public void Follow(float targetY)
        {
            if (!_initialized) return;

            // 더 아래로 내려갔을 때만 타깃 갱신 (위로 튀면 유지)
            _cameraTargetY = Mathf.Min(_cameraTargetY, targetY);

            var camPos = _camera.transform.position;
            camPos.y = Mathf.Lerp(camPos.y, _cameraTargetY, Time.deltaTime * _followSpeed);
            _camera.transform.position = camPos;
        }
    }
}
