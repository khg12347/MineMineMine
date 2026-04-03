using Sirenix.OdinInspector;
using UnityEngine;

namespace MI.Domain.Tile
{
    public enum EMineralDensity : byte
    {
        None   = 0,
        [LabelText("낮은 밀도")]
        Low = 10,
        [LabelText("중간 밀도")]
        Medium = 20,
        [LabelText("높은 밀도")]
        High = 30,
        [LabelText("별(특수)")]
        Star = 50,
    }
}
