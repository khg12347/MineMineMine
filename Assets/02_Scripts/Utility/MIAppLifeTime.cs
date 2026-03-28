using UnityEngine;

namespace MI.Utility
{
    public static class MIAppLifeTime
    {
        public static bool IsQuitting { get; private set; }
        
        public static void OnApplicationQuit()
        {
            IsQuitting = true;
        }
    }
}
