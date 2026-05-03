using MI.Data.Pickaxe;
using MI.Data.Pickaxe.Enhance;

namespace MI.Domain.Pickaxe.Enhance
{
    /// <summary>
    /// 강화 시도 결과 상세 정보.
    /// UI에서 연출/메시지 분기에 사용한다.
    /// </summary>
    public readonly struct FEnhanceAttemptResult
    {
        /// <summary>강화 결과 코드</summary>
        public readonly EEnhanceResult Result;

        /// <summary>대상 곡괭이 타입</summary>
        public readonly EPickaxeType PickaxeType;

        /// <summary>강화 전 레벨</summary>
        public readonly int PreviousLevel;

        /// <summary>강화 후 레벨 (실패 시 PreviousLevel과 동일)</summary>
        public readonly int CurrentLevel;

        /// <summary>성공 여부 간편 확인 (일반 성공 + 대성공 모두 포함)</summary>
        public bool IsSuccess => Result == EEnhanceResult.Success || Result == EEnhanceResult.PerfectlySuccess;

        public FEnhanceAttemptResult(
            EEnhanceResult result,
            EPickaxeType pickaxeType,
            int previousLevel,
            int currentLevel)
        {
            Result        = result;
            PickaxeType   = pickaxeType;
            PreviousLevel = previousLevel;
            CurrentLevel  = currentLevel;
        }
    }
}
