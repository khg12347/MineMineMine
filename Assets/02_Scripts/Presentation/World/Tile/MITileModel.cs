using MI.Core;
using MI.Core.Pool;
using MI.Data.Config;
using MI.Domain.Tile;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MI.Presentation.World.Tile
{
    /// <summary>
    /// 타일의 데미지 수신 / HP 관리 / 파괴 판정 담당.
    /// IMIBreakable을 구현하며, 시각 이벤트는 MITileView에 위임.
    ///
    /// 클래스 관계:
    ///   MIPickaxeController
    ///       └─ IMIBreakable.TakeDamage(damage, hitPoint)
    ///                   ↓
    ///           [MITileModel]  ──직접 참조──▶  [MITileView]
    ///           · FTileData (HP)               · UpdateVisual(spriteIndex)
    ///           · IMIBreakable                 · ShowDamageText(damage, hitPoint)  ← 플로팅 텍스트 진입점
    ///           · 파괴 판정                     · PlayBreakEffect()
    /// </summary>
    [RequireComponent(typeof(MITileView))]
    public class MITileModel : MonoBehaviour, IMIBreakable
    {
        [SerializeField, ReadOnly] private FTileData _data;
        private MIStageManager _parent;
        private MITileView _view;

        public bool IsBreakable       => !_data.IsDestroyed;
        public float BounceMultiplier => _data.BounceMultiplier;

        private void Awake()
        {
            _view = GetComponent<MITileView>();
        }

        /// <summary>스테이지 생성 시 MIStageManager에서 호출 (내부적으로 ResetTile 위임)</summary>
        public void Initialize(MITileConfig tileConfig) => ResetTile(tileConfig);

        /// <summary>
        /// 풀에서 꺼낼 때 타일 상태를 초기화합니다.
        /// HP, 스프라이트, 데미지 단계를 모두 초기값으로 되돌립니다.
        /// </summary>
        public void ResetTile(MITileConfig tileConfig)
        {
            _data = tileConfig.CreateTileData();
            _view.UpdateTileData(tileConfig);
            _view.UpdateVisual(0); // 초기 상태: 온전한 스프라이트
        }
        public void SetParent(MIStageManager parent) => _parent = parent;

        public EBreakResult TakeDamage(int damage, Vector3 hitPoint = default)
        {
            var result = _data.ApplyDamage(damage);

            // 데미지 텍스트는 결과에 관계없이 표시
            _view.ShowDamageText(damage, hitPoint);

            if (result == EBreakResult.Destroyed)
                Break();
            else
                _view.UpdateVisual(_data.MaxDurability - _data.CurrentDurability);

            return result;
        }

        public void Break()
        {
            // EXP 연동: MIStatusManager에 타일 파괴 EXP 전달
            MIStatusManager.Instance.AddExp(_data.DropExp);

            // 파괴 이펙트 재생 (비주얼만, 오브젝트 파괴 없음)
            _view.PlayBreakEffect();

            // Destroy 대신 풀에 반환하여 GC 부담 제거
            MIPoolManager.Instance.Return(this);
            _parent.OnDestroyTileInTilesByRow(this);
        }
    }
}
