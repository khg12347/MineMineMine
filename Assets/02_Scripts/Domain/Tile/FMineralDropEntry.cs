using System;

namespace MI.Domain.Tile
{
    /// <summary>
    /// 타일에 매장된 광물 드랍 테이블의 한 항목.
    /// 광물 종류와 밀도(드랍 수량 범위)를 함께 저장합니다.
    /// 타일 파괴 시 이 정보를 읽어 실제 드랍을 처리합니다.
    /// </summary>
    [Serializable]
    public struct FMineralDropEntry
    {
        /// <summary>드랍할 광물 종류</summary>
        public EMineralType  MineralType;

        /// <summary>광물 밀도 — 실제 드랍 수량의 min~max 범위를 결정합니다 (MIMineralConfig 참조)</summary>
        public EMineralDensity Density;
    }
}
