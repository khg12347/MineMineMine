using Sirenix.OdinInspector;
using UnityEngine;

namespace MI.Domain.Tile
{
    public enum EMineralDensity : byte
    {
        None   = 0,
        [LabelText("낮은 매장량")]
        Low = 10,
        [LabelText("보통 매장량")]
        Medium = 20,
        [LabelText("높은 매장량")]
        High = 30,
        [LabelText("별(특수)")]
        Star = 50,
    }
}
