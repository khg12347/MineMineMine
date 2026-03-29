using UnityEngine.Tilemaps;

namespace MI.Domain.Tile
{
    /// <summary>
    /// 배경 변형 타일 엔트리.
    /// 메인 배경 타일 외에 확률적으로 등장할 변형 타일과 가중치를 보관합니다.
    /// </summary>
    [System.Serializable]
    public struct FBackgroundVariant
    {
        /// <summary>변형 배경 타일</summary>
        public TileBase Tile;

        /// <summary>등장 가중치 (메인 타일 가중치 대비 상대 비율)</summary>
        public float Weight;
    }
}
