using System.Collections.Generic;

namespace MI.Domain.Tile
{
    /// <summary>
    /// 타일 하나의 런타임 데이터.
    /// 내구도, 균열 단계, 드랍 정보(광물 + 타일 재료)를 포함합니다.
    /// 타일 파괴 시 MineralDrop / TileDrop 을 읽어 드랍 처리를 수행합니다.
    /// </summary>
    [System.Serializable]
    public struct FTileData
    {
        public ETileType TileType;
        public int MaxDurability;
        public int CurrentDurability;
        public int DropScore;
        public int DropExp;                // 파괴 시 플레이어에게 지급할 EXP
        public float BounceMultiplier;     // 타일별 바운스 배율 (Dirt=0.8, Stone=1.0, Iron=1.2, Gold=1.5, Diamond=2.0)
        public List<int> CrackLevelDurability;

        /// <summary>
        /// 매장 광물 드랍 정보.
        /// null 이면 광물이 없는 타일입니다.
        /// 광물 종류(EMineralType)와 밀도(EMineralDensity)를 포함합니다.
        /// </summary>
        public FMineralDropEntry? MineralDrop;

        /// <summary>
        /// 타일 재료 드랍 정보.
        /// 모든 타일에 존재합니다 (광물 없는 타일도 재료는 드랍).
        /// </summary>
        public FTileDropEntry TileDrop;

        public bool IsDestroyed => CurrentDurability <= 0;

        public EBreakResult ApplyDamage(int damage)
        {
            if (CurrentDurability >= MaxDurability && CurrentDurability - damage <= 0)
            {
                CurrentDurability = 0;
                return EBreakResult.DestroyWithOneHit;
            }
            CurrentDurability -= damage;

            return IsDestroyed ? EBreakResult.Destroyed : EBreakResult.Damaged;
        }

        public int GetCrackLevel()
        {
            for (int i = CrackLevelDurability.Count - 1; i >= 0; i--)
            {
                if (CurrentDurability <= CrackLevelDurability[i])
                    return i; // 현재 손상 단계 반환
            }
            return CrackLevelDurability.Count; // 최대 손상 단계 반환
        }
    }
}
