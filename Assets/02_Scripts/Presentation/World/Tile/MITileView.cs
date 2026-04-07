using DamageNumbersPro;
using MI.Core.Pool;
using MI.Data.Config;
using MI.Domain.Tile;
using UnityEngine;

namespace MI.Presentation.World.Tile
{
    /// <summary>
    /// 타일의 순수 시각 표현 담당 (스프라이트, 데미지 텍스트, 파괴 이펙트).
    /// 데미지/파괴 로직은 MITileModel에 위임.
    ///
    /// 광물 스프라이트는 MIMineralConfig를 통해
    /// EMineralType × EMineralDensity → Sprite 매핑으로 결정됩니다.
    /// </summary>
    public class MITileView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _tileRenderer;
        [SerializeField] private SpriteRenderer _mineralRenderer;
        [SerializeField] private SpriteRenderer _crackLightRenderer;
        [SerializeField] private DamageNumber   _damageFloatingText;
        [SerializeField] private Animator       _animatorCrack;
        [SerializeField] private Animator       _animatorTile;

        /// <summary>광물 스프라이트 매핑을 담당하는 Config (씬 또는 프리팹에서 할당)</summary>
        [SerializeField] private MIMineralConfig _mineralConfig;

        private GameObject _fxDebris;
        private MITileConfig _config;

        /// <summary>
        /// 타일 데이터를 받아 스프라이트를 갱신합니다.
        ///   - _tileRenderer : 타일 기본 스프라이트 (광물 밀도가 있으면 밀도 오버레이 스프라이트)
        ///   - _mineralRenderer : 광물 오버레이 스프라이트 (EMineralType × EMineralDensity → Sprite)
        /// </summary>
        public void UpdateTileData(MITileConfig config, FTileData tileData)
        {
            _fxDebris = config.PrefabFxTileDebris;
            _config = config;
            _crackLightRenderer.sprite = _config.GetCrackLevelSprite(0);
            if (tileData.MineralDrop.HasValue)
            {
                var drop = tileData.MineralDrop.Value;

                // 타일 바탕 스프라이트: 광물 밀도에 따라 달라지는 오버레이 스프라이트
                _tileRenderer.sprite = config.GetMineralSlotSprite(drop.Density);

                // 광물 렌더러: EMineralType × EMineralDensity 조합 스프라이트
                if (_mineralConfig != null)
                {
                    var mineralSprite = _mineralConfig.GetMineralSprite(drop.MineralType, drop.Density);
                    _mineralRenderer.sprite  = mineralSprite;
                    _mineralRenderer.enabled = mineralSprite != null;
                }
                else
                {
                    _mineralRenderer.enabled = false;
                }
            }
            else
            {
                // 광물 없음 — 기본 타일 스프라이트 표시
                _tileRenderer.sprite     = config.BaseSprite;
                _mineralRenderer.sprite  = null;
                _mineralRenderer.enabled = false;
            }
        }

        /// <summary>
        /// 플로팅 데미지 텍스트 진입점.
        /// hitPoint 위치에 데미지 숫자를 표시.
        /// </summary>
        public void ShowDamageText(int damage, Vector3 hitPoint)
        {
            _damageFloatingText.Spawn(hitPoint, damage);
        }

        public void SetShakeParameter()
        {
            _animatorTile.SetTrigger("tShake");
        }

        public void SetCrackParameter(int crackLevel)
        {
            _crackLightRenderer.sprite = _config.GetCrackLevelSprite(crackLevel);
            _animatorCrack.SetInteger("icrack", crackLevel);
        }

        /// <summary>
        /// 파괴 시 호출. 이펙트/사운드/점수 이벤트 발행 추후 구현.
        /// 오브젝트 비활성화는 호출자(MITileModel.Break)가 MIPoolManager를 통해 처리.
        /// </summary>
        public void PlayBreakEffect()
        {
            // TODO: 파괴 이펙트 재생, 사운드 재생
            if (_fxDebris != null)
            {
                MIPoolManager.Instance.Get<MIReturnToPoolWhenDisable>(_fxDebris, transform.position, Quaternion.identity);
            }
        }
    }
}
