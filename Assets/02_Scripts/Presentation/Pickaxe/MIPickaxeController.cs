using MI.Data.Config;
using MI.Domain.Pickaxe;
using MI.Domain.Tile;
using UnityEngine;

namespace MI.Presentation.Pickaxe
{
    public class MIPickaxeController : MonoBehaviour
    {
        [SerializeField] private MIPickaxeConfig _config;

        private Rigidbody2D _rb;
        private Collider2D _collider;
        private FPickaxeStats _stats;
        private PhysicsMaterial2D _bounceMaterial;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            InitializeFromConfig();
        }

        private void InitializeFromConfig()
        {
            _stats = _config.CreateStats();

            // 중력 설정
            _rb.gravityScale = _stats.GravityScale;

            // PhysicsMaterial2D 생성 및 탄력 적용
            _bounceMaterial = new PhysicsMaterial2D("PickaxeBounce")
            {
                bounciness = _stats.Bounciness,
                friction = _stats.Friction
            };
            _collider.sharedMaterial = _bounceMaterial;
        }

        /// <summary>화면 바깥 상단에 스폰</summary>
        public void SpawnAtOffScreen(Camera cam)
        {
            float camTopY = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, 0f)).y;
            float spawnX = cam.ViewportToWorldPoint(new Vector3(0.5f, 0f, 0f)).x;
            transform.position = new Vector3(spawnX, camTopY + _stats.SpawnOffsetY, 0f);
            _rb.linearVelocity = Vector2.zero;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.gameObject.TryGetComponent(out IMIBreakable breakable))
                return;

            if (!breakable.IsBreakable)
                return;

            var result = breakable.TakeDamage(_stats.Damage);
            float bounceMultiplier = breakable.BounceMultiplier;

            if (result == EBreakResult.Destroyed)
            {
                if (_stats.BounceOnBreak)
                {
                    // 파괴 시 강화 바운스: BreakBounceForce × 타일 BounceMultiplier
                    _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
                    _rb.AddForce(Vector2.up * _stats.BreakBounceForce * bounceMultiplier, ForceMode2D.Impulse);
                }
                else
                {
                    // BounceOnBreak = false: 관통하여 그대로 낙하 (위쪽 속도 제거)
                    if (_rb.linearVelocity.y > 0f)
                        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, -Mathf.Abs(_rb.linearVelocity.y));
                }
            }
            else
            {
                // 내구도 감소만 발생: PhysicsMaterial2D가 기본 바운스 처리 후 배율 적용
                if (bounceMultiplier != 1f && _rb.linearVelocity.y > 0f)
                    _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * bounceMultiplier);
            }
        }
    }
}
