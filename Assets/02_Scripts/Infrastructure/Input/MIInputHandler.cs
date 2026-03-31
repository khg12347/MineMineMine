using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MI.Infrastructure.Input
{
    // 터치/마우스 입력 → 리스너에 탭 의도만 전달 (결과 판정은 각 리스너 담당)
    // OnEnable/OnDisable에서 RegisterListener/UnregisterListener 호출해야 함
    public class MIInputHandler : MonoBehaviour
    {
        #region Fields

        private MIInputActions _inputActions;

        // Tap.performed(버튼 release) 시점에 Position이 (0,0)으로 리셋될 수 있으므로
        // Position.performed 콜백에서 미리 캐시해 둡니다.
        private Vector2 _lastPointerPosition;

        // 역방향 순회: 콜백 내 UnregisterListener 호출 시 안전하게 처리됩니다.
        private readonly List<IMIInputListener> _listeners = new();

        #endregion Fields

        #region Unity Lifecycle

        private void Awake()
        {
            _inputActions = new MIInputActions();
        }

        private void OnEnable()
        {
            _inputActions.Gameplay.Enable();
            _inputActions.Gameplay.Tap.performed      += OnTapPerformed;
            _inputActions.Gameplay.Position.performed += OnPositionPerformed;
        }

        private void OnDisable()
        {
            _inputActions.Gameplay.Tap.performed      -= OnTapPerformed;
            _inputActions.Gameplay.Position.performed -= OnPositionPerformed;
            _inputActions.Gameplay.Disable();
        }

        #endregion Unity Lifecycle

        #region Listener API

        // 리스너 등록. 중복 무시.
        public void RegisterListener(IMIInputListener listener)
        {
            if (listener == null || _listeners.Contains(listener)) return;
            _listeners.Add(listener);
        }

        public void UnregisterListener(IMIInputListener listener)
        {
            _listeners.Remove(listener);
        }

        #endregion Listener API

        #region Input Callbacks

        // 포인터 위치 캐시
        private void OnPositionPerformed(InputAction.CallbackContext ctx)
        {
            _lastPointerPosition = ctx.ReadValue<Vector2>();
        }

        // 탭 완료 시 캐시 위치를 전 리스너에 전달
        private void OnTapPerformed(InputAction.CallbackContext ctx)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
                _listeners[i].OnTap(_lastPointerPosition);
        }

        #endregion Input Callbacks

        #region Editor Debug

#if UNITY_EDITOR
        [Title("디버그"), PropertyOrder(100)]
        [ShowInInspector, ReadOnly, LabelText("등록된 리스너 수")]
        private int ListenerCount => _listeners.Count;
#endif

        #endregion Editor Debug
    }
}
