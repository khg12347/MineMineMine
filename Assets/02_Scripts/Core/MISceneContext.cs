using UnityEngine;

namespace MI.Core
{
    public class MISceneContext : MonoBehaviour
    {
        public static MISceneContext Current { get; private set; }

        public MonoBehaviour UIContextMonoBehaviour;
        public IMIUIContext UIContext => UIContextMonoBehaviour as IMIUIContext;

        private void Awake()
        {
            Current = this;
        }
        private void OnDestroy()
        {
            if (Current == this)
                Current = null;
        }


    }

}