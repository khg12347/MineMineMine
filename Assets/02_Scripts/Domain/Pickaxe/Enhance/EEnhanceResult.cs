namespace MI.Domain.Pickaxe.Enhance
{
    /// <summary>
    /// 강화 시도 결과. UI에서 분기 처리에 사용.
    /// </summary>
    public enum EEnhanceResult
    {
        // 정상 결과 (UI 분기용)

        /// <summary>강화 성공 — 레벨 상승</summary>
        Success              = 1,

        /// <summary>강화 실패 — 레벨 유지, 재료/재화 소모</summary>
        Fail                 = 2,

        /// <summary>강화석 재료 부족</summary>
        InsufficientMaterial = 3,

        /// <summary>재화(골드/다이아) 부족</summary>
        InsufficientCurrency = 4,

        // 버그 방지용 (도달 시 throw InvalidOperationException)
        // UI에서 강화 버튼 자체를 차단해야 하는 상태

        /// <summary>곡괭이 미보유 — UI에서 버튼 차단 전제</summary>
        NotOwned             = 90,

        /// <summary>이미 최대 레벨 — UI에서 버튼 차단 전제</summary>
        MaxLevel             = 91,

        /// <summary>비용 데이터 누락 — 설정 오류</summary>
        NoData               = 92,
    }
}
