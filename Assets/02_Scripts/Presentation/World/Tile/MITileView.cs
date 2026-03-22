using DamageNumbersPro;
using MI.Core.Pool;
using MI.Data.Config;
using MI.Domain.Tile;
using MI.Presentation.World.VFX;
using UnityEngine;

namespace MI.Presentation.World.Tile
{
    /// <summary>
    /// 타일의 순수 시각 표현 담당 (스프라이트, 데미지 텍스트, 파괴 이펙트).
    /// 데미지/파괴 로직은 MITileModel에 위임.
    /// </summary>
    public class MITileView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _tileRenderer;
        [SerializeField] private SpriteRenderer _mineralRenderer;
        [SerializeField] private DamageNumber _damageFloatingText;
        [SerializeField] private Animator _animatorCrack;
        [SerializeField] private Animator _animatorTile;

        private GameObject _fxDebris;
        

        public void UpdateTileData(MITileConfig config, FTileData tileData)
        {
            //_tileRenderer.sprite = config.GetMinenralSlotSprites(tileData.MineralType);
            _tileRenderer.sprite = config.BaseSprite;
            _fxDebris = config.PrefabFxTileDebris;
            
        }
        
        /// <summary>
        /// 플로팅 데미지 텍스트 진입점.
        /// hitPoint 위치에 데미지 숫자를 표시. 실제 텍스트 오브젝트 생성은 추후 구현.
        /// </summary>
        public void ShowDamageText(int damage, Vector3 hitPoint)
        {
            // TODO: hitPoint 위치에 플로팅 데미지 텍스트 오브젝트 생성
            _damageFloatingText.Spawn(hitPoint, damage);
        }

        public void SetShakeParameter()
        {
            _animatorTile.SetTrigger("tShake");
        }

        public void SetCrackParameter(int crackLevel)
        {
            _animatorCrack.SetInteger("icrack", crackLevel);
        }
        /// <summary>
        /// 파괴 시 호출. 이펙트/사운드/점수 이벤트 발행 추후 구현.
        /// 오브젝트 비활성화는 호출자(MITileModel.Break)가 MIPoolManager를 통해 처리.
        /// </summary>
        public void PlayBreakEffect()
        {
            // TODO: 파괴 이펙트 재생, 사운드 재생
            if(_fxDebris != null)
            {
                MIPoolManager.Instance.Get<MIFxAutoFade>(_fxDebris, transform.position, Quaternion.identity);
            }
        }
    }
}
