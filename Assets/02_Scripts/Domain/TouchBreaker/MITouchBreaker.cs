using MI.Domain.Tile;
using MI.Infrastructure.Input;
using MI.Utility;
using UnityEngine;

namespace MI.Domain.TouchBreaker
{
    /// <summary>
    /// <see cref="MIInputHandler"/>로부터 탭 의도를 전달받아 타일 파괴를 수행합니다.
    /// 입력 감지에는 관여하지 않으며, MIInputHandler에 단방향으로 의존합니다.
    ///
    /// 씬 설정:
    ///   MIInputHandler와 같은 또는 별개의 GameObject에 배치 가능.
    ///   Inspector에서 _inputHandler를 연결하거나,
    ///   비워두면 같은 GameObject의 MIInputHandler를 자동으로 찾습니다.
    /// </summary>
    public class MITouchBreaker : MonoBehaviour, IMIInputListener
    {
        [SerializeField] private MIInputHandler _inputHandler;
        [SerializeField] private Camera         _mainCamera;
        [SerializeField] private LayerMask      _tileLayer;
        [SerializeField] private int            _touchDamage = 1;

        // ── Unity 생명주기 ─────────────────────────────────────────────

        private void Awake()
        {
            // Inspector에서 연결되지 않은 경우 같은 GameObject에서 자동 탐색
            if (_inputHandler == null)
                _inputHandler = GetComponent<MIInputHandler>();

            if (_mainCamera == null)
                _mainCamera = Camera.main;
        }

        private void OnEnable()  => _inputHandler.RegisterListener(this);
        private void OnDisable() => _inputHandler.UnregisterListener(this);

        // ── IMIInputListener 구현 ──────────────────────────────────────

        /// <summary>
        /// MIInputHandler가 탭을 감지하면 호출됩니다.
        /// 전달받은 스크린 좌표로 타일 파괴를 시도합니다.
        /// </summary>
        public void OnTap(Vector2 screenPosition) => TryBreakTileAt(screenPosition);

        // ── 파괴 로직 ──────────────────────────────────────────────────

        private void TryBreakTileAt(Vector2 screenPos)
        {
            Vector2 worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
            var hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, _tileLayer);

            //MILog.Log($"Tap at screen {screenPos}, world {worldPos} - Hit: {hit.collider?.name ?? "None"}");

            if (hit.collider == null ||
                !hit.collider.TryGetComponent(out IMIBreakable breakable))
            {
                //MILog.Log($"No breakable tile at {worldPos}");
                return;
            }

            if (breakable.IsBreakable)
                breakable.TakeDamage(_touchDamage, worldPos);
        }
    }
}
