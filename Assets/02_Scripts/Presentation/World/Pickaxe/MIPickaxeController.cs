using MI.Data.Config;
using MI.Domain.Pickaxe;
using MI.Domain.Tile;
using UnityEngine;

namespace MI.Presentation.World.Pickaxe
{
    using Camera = UnityEngine.Camera;

    public class MIPickaxeController : MonoBehaviour
    {
        [SerializeField] private MIPickaxeConfig _config;
        [SerializeField] private Vector2 _minimumForce = new Vector2(0, 1f);
        [SerializeField] private float _autoRotateSpeed = 180f; // 초당 회전 속도 (도 단위)
        [SerializeField] private bool _flipRotationDir = false; // 회전 방향 반전 여부 (시계/반시계)

        private Rigidbody2D _rb;
        private FPickaxeStats _stats;
        private PhysicsMaterial2D _bounceMaterial;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            InitializeFromConfig();
        }
        private void Update()
        {
            // 자동 회전 설정 (프로토타입, 검증 후 정식 사양으로 수정 요망)
            PROTO_RotatePickaxe();
        }

        private void PROTO_RotatePickaxe()
        {
            // 자동 회전: 초당 일정 각도로 회전 (예시: 180도/초)
            float rotationThisFrame = _flipRotationDir ? -_autoRotateSpeed * Time.deltaTime : _autoRotateSpeed * Time.deltaTime;
            transform.Rotate(0f, 0f, rotationThisFrame);
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

            // 자식 오브젝트의 모든 콜라이더에 물리 재질 적용
            foreach (var col in GetComponentsInChildren<Collider2D>())
                col.sharedMaterial = _bounceMaterial;
        }

        /// <summary>화면 바깥 상단에 스폰</summary>
        public void SpawnAtOffScreen(Camera cam)
        {
            float camTopY = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, 0f)).y;
            float spawnX = cam.ViewportToWorldPoint(new Vector3(0.5f, 0f, 0f)).x;
            transform.position = new Vector3(spawnX, camTopY + _stats.SpawnOffsetY, 0f);
            _rb.linearVelocity = Vector2.zero;
        }

        /// <summary>부위별 충돌 처리 — MIPickaxePartCollider에서 호출</summary>
        public void OnPartCollision(EPickaxePart part, Collision2D collision)
        {
            if (!collision.gameObject.TryGetComponent(out IMIBreakable breakable))
                return;

            if (!breakable.IsBreakable)
                return;

            // 부위에 따라 데미지 결정
            int damage = part == EPickaxePart.Head ? _stats.HeadDamage : _stats.HandleDamage;

            // 충돌 지점: 첫 번째 접촉점, 없으면 충돌 오브젝트 위치로 fallback
            Vector3 hitPoint = collision.contacts.Length > 0
                ? (Vector3)collision.contacts[0].point
                : collision.transform.position;

            var result = breakable.TakeDamage(damage, hitPoint);
            float bounceMultiplier = breakable.BounceMultiplier;

            if (result == EBreakResult.Destroyed)
            {
                if (_stats.BounceOnBreak)
                {
                    // 튕김 방향: 충돌 대상으로부터 멀어지는 방향 (없으면 위쪽 fallback)
                    Vector2 bounceDir = collision.transform != null
                        ? ((Vector2)(transform.position - collision.transform.position)).normalized
                        : Vector2.up;
                    if (bounceDir == Vector2.zero) bounceDir = Vector2.up;

                    // 파괴 시 강화 바운스: BreakBounceForce × 타일 BounceMultiplier
                    _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0f);
                    _rb.AddForce(bounceDir * _stats.BreakBounceForce * bounceMultiplier, ForceMode2D.Impulse);
                }
                else
                {
                    // BounceOnBreak = false: 관통하여 그대로 낙하 (위쪽 속도 제거)
                    if (_rb.linearVelocity.y > 0f)
                        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, -Mathf.Abs(_rb.linearVelocity.y));
                }
            }
            else if(result == EBreakResult.Damaged)
            {
                // 내구도 감소만 발생: PhysicsMaterial2D가 기본 바운스 처리 후 배율 적용
                if (bounceMultiplier != 1f && _rb.linearVelocity.y > 0f)
                    _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * bounceMultiplier) + _minimumForce;
            }
            else if (result == EBreakResult.DestroyWithOneHit)
            {
                // 한 방에 파괴: 바운스 없음
                if (_rb.linearVelocity.y > 0f)
                    _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, -Mathf.Abs(_rb.linearVelocity.y));
            }
        }
    }
}
