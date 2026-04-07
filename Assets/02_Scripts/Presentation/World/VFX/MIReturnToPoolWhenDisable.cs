using UnityEngine;

namespace MI.Core.Pool
{
    public class MIReturnToPoolWhenDisable : MonoBehaviour
    {
        private void OnDisable()
        {
            MIPoolManager.Instance.Return(this);
        }
    }
}
