using UnityEngine;

namespace MI.Utility
{
    public static class MILog
    {
        public static void Log(string message)
        {
            Debug.Log($"<color=cyan>[MI]</color> {message}");
        }

        public static void LogWarning(string message)
        {
            Debug.LogWarning($"<color=yellow>[MI]</color> {message}");
        }

        public static void LogError(string message)
        {
            Debug.LogError($"<color=red>[MI]</color> {message}");
        }
    }
}