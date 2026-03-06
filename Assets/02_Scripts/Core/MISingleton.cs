using UnityEngine;

namespace MI.Core
{
    public class MISingleton : MonoBehaviour
    {
        public static MISingleton Instance { get; private set; }
        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
    }
}