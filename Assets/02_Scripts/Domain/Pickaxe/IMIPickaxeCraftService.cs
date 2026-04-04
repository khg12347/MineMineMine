using System;

namespace MI.Domain.Pickaxe
{
    /// <summary>
    /// 곡괭이 제작 조건 검증 및 실행 인터페이스.
    /// </summary>
    public interface IMIPickaxeCraftService
    {
        /// <summary>제작 가능 여부. UI 버튼 활성/비활성 판단에 사용.</summary>
        bool CanCraft(EPickaxeType type);

        /// <summary>
        /// 제작 실행. 재료+재화 소모 → 곡괭이 보유 등록.
        /// 조건 불충족 시 false 반환.
        /// </summary>
        bool TryCraft(EPickaxeType type);

        /// <summary>개별 재료의 보유량이 충분한지 확인. UI 수량 텍스트 색상 결정에 사용.</summary>
        bool HasEnoughMaterial(FMaterialCost material);

        /// <summary>재화 충분 여부 확인.</summary>
        bool HasEnoughCurrency(FCurrencyCost currency);

        /// <summary>
        /// 제작 완료 시 발행.
        /// 강화 시스템은 이 이벤트를 구독하여 새 곡괭이 등장 시 초기 데이터를 세팅한다. (OCP 확장 포인트)
        /// </summary>
        event Action<EPickaxeType> OnPickaxeCrafted;
    }
}
