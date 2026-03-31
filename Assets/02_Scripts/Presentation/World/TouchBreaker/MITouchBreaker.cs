using MI.Domain.Tile;
using MI.Domain.World.TouchBreaker;
using MI.Infrastructure.Input;
using UnityEngine;

namespace MI.Presentation.World.TouchBreaker
{
    using Camera = UnityEngine.Camera;

    // MIInputHandler에서 탭 의도를 받아 타일 파괴 처리
    // _inputHandler 미연결 시 같은 GameObject에서 자동 탐색
    public class MITouchBreaker : MonoBehaviour, IMIInputListener
    {
        [SerializeField] private MIInputHandler _inputHandler;
        [SerializeField] private Camera         _mainCamera;
        [SerializeField] private LayerMask      _tileLayer;
        [SerializeField] private int            _touchDamage = 1;
        [SerializeField] private GameObject _prefabTouchObj;
        private MITouchObjectSpawner _touchObjectSpawner;

        #region Unity Events

        private void Awake()
        {
            // Inspector에서 연결되지 않은 경우 같은 GameObject에서 자동 탐색
            if (_inputHandler == null)
                _inputHandler = GetComponent<MIInputHandler>();

            if (_mainCamera == null)
                _mainCamera = Camera.main;

            _touchObjectSpawner = new MITouchObjectSpawner(_prefabTouchObj, transform);
        }

        private void OnEnable()  => _inputHandler.RegisterListener(this);
        private void OnDisable() => _inputHandler.UnregisterListener(this);

        #endregion Unity Events

        #region IMIInputListener

        // 탭 감지 시 호출. 스크린 좌표로 타일 파괴 시도.
        public void OnTap(Vector2 screenPosition) => TryBreakTileAt(screenPosition);

        #endregion IMIInputListener

        #region Break Logic

        private void TryBreakTileAt(Vector2 screenPos)
        {
            Vector2 worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
            var hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, _tileLayer);

            //MILog.Log($"Tap at screen {screenPos}, world {worldPos} - Hit: {hit.collider?.name ?? "None"}");
            _touchObjectSpawner.Spawn(worldPos);

            if (hit.collider == null ||
                !hit.collider.TryGetComponent(out IMIBreakable breakable))
            {
                //MILog.Log($"No breakable tile at {worldPos}");
                return;
            }

            if (breakable.IsBreakable)
                breakable.TakeDamage(_touchDamage, worldPos);
        }

        #endregion Break Logic
    }
}
