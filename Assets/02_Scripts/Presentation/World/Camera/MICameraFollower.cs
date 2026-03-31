using MI.Core;
using UnityEngine;

namespace MI.Presentation.World.Camera
{
    using Camera = UnityEngine.Camera;

    // IMICameraFollower MonoBehaviour 구현체
    // 곡괭이가 아래로 내려갈 때만 타깃 갱신 (위로 튀어도 카메라 고정)
    public class MICameraFollower : MonoBehaviour, IMICameraFollower
    {
        private Camera _camera;
        private float _followSpeed;
        private float _cameraTargetY;
        private bool _initialized;

        #region IMICameraFollower

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

        #endregion IMICameraFollower
    }
}
