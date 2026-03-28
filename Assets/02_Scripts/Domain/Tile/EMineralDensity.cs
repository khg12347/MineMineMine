using Sirenix.OdinInspector;
using UnityEngine;

namespace MI.Domain.Tile
{
    public enum EMineralDensity : byte
    {
        None   = 0,
        [LabelText("���� ���差")]
        Low = 10,
        [LabelText("���� ���差")]
        Medium = 20,
        [LabelText("���� ���差")]
        High = 30,
        [LabelText("��(Ư��)")]
        Star = 50,
    }
}
