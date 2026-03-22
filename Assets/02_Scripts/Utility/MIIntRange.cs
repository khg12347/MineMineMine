using UnityEngine;

namespace MI.Untility
{
    public class MIIntRange
    {
        public int Min;
        public int Max;
        public MIIntRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public bool IsInRange(int value)
        {
            return value >= Min && value <= Max;
        }

    }
}
