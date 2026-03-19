using System.Collections.Generic;

namespace MI.Domain.Tile
{
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
        public EMineralType MineralType;   // 포함된 광물 종류 (None = 광물 없음)
        public int DepositAmount;          // 광물 매장량

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
