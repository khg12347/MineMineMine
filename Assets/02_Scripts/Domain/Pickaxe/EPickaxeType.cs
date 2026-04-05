using Sirenix.OdinInspector;

namespace MI.Domain.Pickaxe
{
    /// <summary>
    /// 곡괭이 종류. 0~10은 제작 가능, None은 빈 상태.
    /// </summary>
    public enum EPickaxeType
    {
        None = 0,
        [LabelText("나무 곡괭이")]           Pickaxe01 = 1,
        [LabelText("돌 곡괭이")]             Pickaxe02 = 2,
        [LabelText("철 곡괭이")]             Pickaxe03 = 3,
        [LabelText("구리 곡괭이")]           Pickaxe04 = 4,
        [LabelText("은 곡괭이")]             Pickaxe05 = 5,
        [LabelText("금 곡괭이")]             Pickaxe06 = 6,
        [LabelText("플래티넘 곡괭이")]       Pickaxe07 = 7,
        [LabelText("에메랄드 곡괭이")]       Pickaxe08 = 8,
        [LabelText("루비 곡괭이")]           Pickaxe09 = 9,
        [LabelText("다이아몬드 곡괭이")]     Pickaxe10 = 10,
    }
}
