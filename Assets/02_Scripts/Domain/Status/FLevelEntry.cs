using System;
using Sirenix.OdinInspector;

namespace MI.Domain.Status
{
    /// <summary>
    /// 특정 레벨에서 다음 레벨로 진급하는 데 필요한 EXP 정의.
    /// MIStatusConfig의 레벨 테이블 항목으로 사용.
    /// </summary>
    [Serializable]
    public struct FLevelEntry
    {
        [LabelText("레벨")]
        [PropertyRange(1, 999)]
        public int Level;

        [LabelText("필요 EXP")]
        [PropertyRange(1, 99999)]
        public int RequiredExp;
    }
}
