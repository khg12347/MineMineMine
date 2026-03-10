using MI.Domain.Pickaxe;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace MI.Data.Config
{
    [CreateAssetMenu(fileName = "PickaxeConfig", menuName = "MI/Config/Pickaxe")]
    public class MIPickaxeConfig : SerializedScriptableObject
    {
        [Title("공격 — 머리 (Head)")]
        [InfoBox("머리 부위 충돌 시 적용되는 데미지")]
        [SerializeField] private int _headDamage = 2;

        [Title("공격 — 자루 (Handle)")]
        [InfoBox("자루 부위 충돌 시 적용되는 데미지")]
        [SerializeField] private int _handleDamage = 1;

        [Title("물리")]
        [PropertyRange(0.5f, 10f)]
        [SerializeField] private float _gravityScale = 3f;

        [Title("바운스 설정")]
        [InfoBox("탄력: 0.0(튀김 없음) ~ 1.0(완전 탄성)")]
        [PropertyRange(0f, 1f)]
        [SerializeField] private float _bounciness = 0.6f;

        [PropertyRange(0f, 1f)]
        [SerializeField] private float _friction = 0f;

        [Title("블록 파괴 시 바운스")]
        [ToggleLeft]
        [SerializeField] private bool _bounceOnBreak = true;

        [ShowIf("_bounceOnBreak")]
        [PropertyRange(0f, 20f)]
        [SerializeField] private float _breakBounceForce = 5f;

        [Title("벽 반사")]
        [InfoBox("좌우 벽의 탄력: 0.0(흡수) ~ 1.0(완전 반사)")]
        [PropertyRange(0f, 1f)]
        [SerializeField] private float _wallBounciness = 0.8f;

        [Title("스폰 설정")]
        [InfoBox("화면 상단 바깥으로부터의 오프셋 (월드 유닛)")]
        [PropertyRange(0.5f, 5f)]
        [SerializeField] private float _spawnOffsetY = 2f;

        public FPickaxeStats CreateStats()
        {
            return new FPickaxeStats
            {
                HeadDamage = _headDamage,
                HandleDamage = _handleDamage,
                GravityScale = _gravityScale,
                Bounciness = _bounciness,
                Friction = _friction,
                BounceOnBreak = _bounceOnBreak,
                BreakBounceForce = _breakBounceForce,
                SpawnOffsetY = _spawnOffsetY,
                WallBounciness = _wallBounciness
            };
        }
    }
}
