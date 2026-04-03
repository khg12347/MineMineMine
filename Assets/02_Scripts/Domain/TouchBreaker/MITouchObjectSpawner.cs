using MI.Core.Pool;
using MI.Presentation.World.TouchBreaker;
using UnityEngine;

namespace MI.Domain.World.TouchBreaker
{
    public class MITouchObjectSpawner
    {
        private GameObject _prefabTouchObj;
        private Transform _parent;
        private FPoolConfig _poolConfig = new FPoolConfig(){GrowSize = 10, InitialSize = 10};
        
        public MITouchObjectSpawner(GameObject prefabTouchObj, Transform parent)
        {
            _prefabTouchObj = prefabTouchObj;
            _parent = parent;
            
            MIPoolManager.Instance.InitPool<MITouchObjectViewer>(_prefabTouchObj, _poolConfig);
        }

        public void Spawn(Vector3 worldPositioin)
        {
            var objViewer = MIPoolManager.Instance.Get<MITouchObjectViewer>(_prefabTouchObj,  worldPositioin, Quaternion.identity, _parent);

            objViewer.Activate();
        }
    }
}
