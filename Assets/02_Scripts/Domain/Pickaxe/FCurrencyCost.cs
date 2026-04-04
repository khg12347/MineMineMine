using System;
using MI.Domain.UserState.Wallet;

namespace MI.Domain.Pickaxe
{
    /// <summary>
    /// 제작 재화 비용 1건. 재화 타입과 필요 수량.
    /// 재화가 불필요한 곡괭이는 이 값을 포함하지 않음 → UI에서 재화 영역 숨김.
    /// </summary>
    [Serializable]
    public struct FCurrencyCost
    {
        // 재화 타입 (Gold / Diamond). 기존 ECurrencyType 재사용.
        public ECurrencyType CurrencyType;

        // 필요 수량 (대량 재화 안전성을 위해 long)
        public long Amount;
    }
}
