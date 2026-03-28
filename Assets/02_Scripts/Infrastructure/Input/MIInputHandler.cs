using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MI.Infrastructure.Input
{
    /// <summary>
    /// 터치/마우스 입력을 감지하여 등록된 리스너에게 "의도(Intent)"만 전달합니다.
    /// 입력이 어떤 결과를 낳는지(타일 파괴, UI 클릭 등)는 관여하지 않습니다.
    ///
    /// 사용 패턴:
    ///   1. MonoBehaviour에서 <see cref="IMIInputListener"/>를 구현
    ///   2. OnEnable/OnDisable에서 RegisterListener/UnregisterListener 호출
    /// </summary>
    public class MIInputHandler : MonoBehaviour
    {
        // ── 내부 상태 ──────────────────────────────────────────────────

        private MIInputActions _inputActions;

        // Tap.performed(버튼 release) 시점에 Position이 (0,0)으로 리셋될 수 있으므로
        // Position.performed 콜백에서 미리 캐시해 둡니다.
        private Vector2 _lastPointerPosition;

        // 역방향 순회: 콜백 내 UnregisterListener 호출 시 안전하게 처리됩니다.
        private readonly List<IMIInputListener> _listeners = new();

        // ── Unity 생명주기 ─────────────────────────────────────────────

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

        // ── 리스너 등록/해제 API ──────────────────────────────────────

        /// <summary>
        /// 입력 리스너를 등록합니다. 중복 등록은 무시됩니다.
        /// </summary>
        public void RegisterListener(IMIInputListener listener)
        {
            if (listener == null || _listeners.Contains(listener)) return;
            _listeners.Add(listener);
        }

        /// <summary>
        /// 입력 리스너를 해제합니다.
        /// </summary>
        public void UnregisterListener(IMIInputListener listener)
        {
            _listeners.Remove(listener);
        }

        // ── 입력 콜백 ─────────────────────────────────────────────────

        /// <summary>포인터 이동 콜백 — 항상 최신 위치를 캐시합니다.</summary>
        private void OnPositionPerformed(InputAction.CallbackContext ctx)
        {
            _lastPointerPosition = ctx.ReadValue<Vector2>();
        }

        /// <summary>
        /// 탭 완료 콜백 — 캐시된 위치를 모든 리스너에게 전달합니다.
        /// 역방향 순회로 콜백 내 UnregisterListener 호출이 안전합니다.
        /// </summary>
        private void OnTapPerformed(InputAction.CallbackContext ctx)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
                _listeners[i].OnTap(_lastPointerPosition);
        }

        // ── 에디터 디버그 ─────────────────────────────────────────────

#if UNITY_EDITOR
        [Title("디버그"), PropertyOrder(100)]
        [ShowInInspector, ReadOnly, LabelText("등록된 리스너 수")]
        private int ListenerCount => _listeners.Count;
#endif
    }
}
