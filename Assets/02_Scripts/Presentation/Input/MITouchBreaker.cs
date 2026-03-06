using MI.Domain.Tile;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MI.Presentation.Input
{
    public class MITouchBreaker : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private LayerMask _tileLayer;
        [SerializeField] private int _touchDamage = 1;

        private MIInputActions _inputActions;

        private void Awake()
        {
            _inputActions = new MIInputActions();
            if (_mainCamera == null)
                _mainCamera = Camera.main;
        }

        private void OnEnable() => _inputActions.Gameplay.Enable();
        private void OnDisable() => _inputActions.Gameplay.Disable();

        private void Update()
        {
            if (_inputActions.Gameplay.Tap.WasPerformedThisFrame())
            {
                var screenPos = _inputActions.Gameplay.Position.ReadValue<Vector2>();
                TryBreakTileAt(screenPos);
            }
        }

        private void TryBreakTileAt(Vector2 screenPos)
        {
            Vector2 worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
            var hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, _tileLayer);
            if (hit.collider != null &&
                hit.collider.TryGetComponent(out IMIBreakable breakable))
            {
                if (breakable.IsBreakable)
                    breakable.TakeDamage(_touchDamage);
            }
        }
    }
}
