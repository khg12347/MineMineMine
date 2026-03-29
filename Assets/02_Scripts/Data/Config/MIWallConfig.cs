using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MI.Data.Config
{
    /// <summary>
    /// 벽 타일맵 설정 ScriptableObject.
    /// 좌우 벽은 각각 배열로 등록하며, row % length 순환 구조로 반복됩니다.
    /// </summary>
    [CreateAssetMenu(fileName = "WallConfig", menuName = "MI/Config/Wall")]
    public class MIWallConfig : SerializedScriptableObject
    {
        [Title("벽 타일 설정")]
        [InfoBox("배열 순서대로 반복됩니다. (row % 배열길이 인덱스 사용)")]
        [LabelText("왼쪽 벽 타일 배열")]
        [SerializeField] private TileBase[] _leftWallTiles;

        [LabelText("오른쪽 벽 타일 배열")]
        [SerializeField] private TileBase[] _rightWallTiles;

        [LabelText("양쪽 벽 남는 부분 채우는 타일")]
        [SerializeField] private TileBase _wallTempTile;

        [Title("위치 오프셋")]
        [InfoBox("벽 타일맵 전체를 Grid 기준으로 이동합니다. 단위: 월드 좌표")]
        [LabelText("타일맵 오프셋 (X / Y)")]
        [SerializeField] private Vector2 _positionOffset = Vector2.zero;

        public Vector2  PositionOffset => _positionOffset;
        public TileBase WallTempTile   => _wallTempTile;

        /// <summary>행 인덱스에 맞는 왼쪽 벽 타일 반환 (순환)</summary>
        public TileBase GetLeftWallTile(int row)  => _leftWallTiles[row % _leftWallTiles.Length];

        /// <summary>행 인덱스에 맞는 오른쪽 벽 타일 반환 (순환)</summary>
        public TileBase GetRightWallTile(int row) => _rightWallTiles[row % _rightWallTiles.Length];
    }
}
