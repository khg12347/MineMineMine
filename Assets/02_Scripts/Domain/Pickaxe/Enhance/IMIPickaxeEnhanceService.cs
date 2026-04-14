using System;

using MI.Domain.Pickaxe;
using MI.Domain.Pickaxe.Craft;

namespace MI.Domain.Pickaxe.Enhance
{
    /// <summary>
    /// 곡괭이 강화 조건 검증 및 실행 인터페이스.
    /// IMIPickaxeCraftService 패턴을 따른다.
    /// </summary>
    public interface IMIPickaxeEnhanceService
    {
        /// <summary>최대 강화 레벨</summary>
        int MaxLevel { get; }

        /// <summary>
        /// 강화 가능 여부. 보유 여부·최대 레벨·재료·재화를 모두 검증한다.
        /// UI 버튼 활성/비활성 판단에 사용.
        /// </summary>
        bool CanEnhance(EPickaxeType type);

        /// <summary>
        /// 강화 실행. 재료/재화 소모 → 확률 판정 → 성공 시 레벨 증가 + ResolveStats.
        /// 실패해도 재료/재화는 소모된다.
        /// NotOwned, MaxLevel, NoData 상태에서 호출하면 InvalidOperationException을 던진다.
        /// </summary>
        FEnhanceAttemptResult TryEnhance(EPickaxeType type);

        /// <summary>
        /// 해당 곡괭이의 현재 레벨에 대한 강화 비용 엔트리를 반환한다.
        /// UI에서 비용/확률 표시에 사용.
        /// 미보유 또는 데이터 없으면 null 반환.
        /// </summary>
        FEnhanceLevelEntry? GetCurrentLevelEntry(EPickaxeType type);

        /// <summary>해당 곡괭이의 현재 레벨을 반환한다. 미보유 시 0.</summary>
        int GetCurrentLevel(EPickaxeType type);

        /// <summary>개별 재료의 보유량이 충분한지 확인. UI 수량 텍스트 색상 결정에 사용.</summary>
        bool HasEnoughMaterial(FMaterialCost material);

        /// <summary>재화 충분 여부 확인.</summary>
        bool HasEnoughCurrency(FCurrencyCost currency);

        /// <summary>재료 현재 보유량 조회.</summary>
        int GetMaterialAmount(FMaterialCost material);

        /// <summary>
        /// 강화 시도 완료 시 발행. 성공/실패 모두 발행된다.
        /// UI에서 강화 연출 분기에 사용.
        /// </summary>
        event Action<FEnhanceAttemptResult> OnEnhanceAttempted;
    }
}
