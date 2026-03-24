using UnityEngine;

namespace MI.Untility
{
    [System.Serializable]
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
