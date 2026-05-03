using MI.Data.Pickaxe;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MI.Presentation.World.Pickaxe
{
    /// <summary>
    /// 곡괭이 부위별 콜라이더 컴포넌트.
    /// 자식 오브젝트(Head / Handle)에 부착하며,
    /// 충돌 발생 시 부모의 MIPickaxeController에 파트 정보를 전달한다.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class MIPickaxePartCollider : MonoBehaviour
    {
        [Title("부위 설정")]
        [EnumToggleButtons]
        [SerializeField] private EPickaxePart _part = EPickaxePart.Head;

        private MIPickaxeController _controller;
        private Collider2D _collider;

        public EPickaxePart Part => _part;
        public Collider2D Collider => _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _controller = GetComponentInParent<MIPickaxeController>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            _controller.OnPartCollision(_part, collision);
        }
    }
}
