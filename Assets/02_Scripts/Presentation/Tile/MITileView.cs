using MI.Domain.Tile;
using UnityEngine;

namespace MI.Presentation.Tile
{
    public class MITileView : MonoBehaviour, IMIBreakable
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite[] _damageSprites;

        private FTileData _data;

        public bool IsBreakable => !_data.IsDestroyed;

        public void Initialize(FTileData data)
        {
            _data = data;
            UpdateVisual();
        }

        public EBreakResult TakeDamage(int damage)
        {
            var result = _data.ApplyDamage(damage);
            if (result == EBreakResult.Destroyed)
                Break();
            else
                UpdateVisual();
            return result;
        }

        public void Break()
        {
            // TODO: 파괴 이펙트 재생, 점수 이벤트 발행
            Destroy(gameObject);
        }

        private void UpdateVisual()
        {
            // index 0 = 온전한 상태, 1 = 1회 피격, ...
            int index = _data.MaxDurability - _data.CurrentDurability;
            if (_damageSprites != null && index < _damageSprites.Length)
                _spriteRenderer.sprite = _damageSprites[index];
        }
    }
}
