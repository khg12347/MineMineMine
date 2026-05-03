using System.Collections.Generic;
using MI.Data.Tile;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MI.Data.Config
{
    /// <summary>
    /// 배경 타일맵 설정 ScriptableObject.
    /// 메인 배경 타일과 확률적으로 등장할 변형 타일 목록을 관리합니다.
    /// 가중치는 상대 비율로, 합계 기준으로 확률을 계산합니다.
    /// </summary>
    [CreateAssetMenu(fileName = "BackgroundConfig", menuName = "MI/Config/Background")]
    public class MIBackgroundConfig : SerializedScriptableObject
    {
        [Title("메인 배경 타일")]
        [PreviewField(50)]
        [LabelText("메인 타일")]
        [SerializeField] private TileBase _mainTile;

        [LabelText("메인 타일 가중치")]
        [InfoBox("변형 타일 가중치 합 대비 상대 비율. 값이 클수록 메인 타일이 자주 등장합니다.")]
        [PropertyRange(1f, 1000f)]
        [SerializeField] private float _mainWeight = 90f;

        [Title("변형 타일 목록")]
        [InfoBox("각 변형 타일의 가중치 합 + 메인 가중치 = 100%로 계산됩니다.")]
        [TableList]
        [SerializeField] private List<FBackgroundVariant> _variants = new();

        [Title("위치 오프셋")]
        [InfoBox("벽 타일맵 전체를 Grid 기준으로 이동합니다. 단위: 월드 좌표")]
        [LabelText("오프셋 (X / Y)")]
        [SerializeField] private Vector2 _positionOffset = Vector2.zero;

        public Vector2 PositionOffset => _positionOffset;
        public TileBase                       MainTile    => _mainTile;
        public float                          MainWeight  => _mainWeight;
        public IReadOnlyList<FBackgroundVariant> Variants => _variants;
    }
}
