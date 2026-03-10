using System.Collections.Generic;
using UnityEngine;

namespace MI.Core.Pool
{
    /// <summary>
    /// 범용 오브젝트 풀.
    /// Component를 상속하는 타입에 대해 동작하며, 타일 외에도 재사용 가능.
    /// </summary>
    public class MIObjectPool<T> where T : Component
    {
        private readonly Queue<T> _pool;
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly bool _autoExpand;
        private readonly int _maxSize;
        private int _totalCreated;
        private int _initialSize;

        public int CountInactive => _pool.Count;
        public int CountAll => _totalCreated;
        public int CountActive => _totalCreated - _pool.Count;

        /// <summary>
        /// 풀 생성자.
        /// </summary>
        /// <param name="prefab">생성 기반 프리팹 컴포넌트</param>
        /// <param name="parent">비활성 오브젝트를 보관할 부모 Transform</param>
        /// <param name="initialSize">시작 시 미리 생성해둘 오브젝트 수</param>
        /// <param name="autoExpand">풀 소진 시 자동 확장 여부</param>
        /// <param name="maxSize">자동 확장 시 넘지 않을 최대 오브젝트 수</param>
        public MIObjectPool(T prefab, Transform parent, int initialSize, bool autoExpand = true, int maxSize = 256)
        {
            _prefab = prefab;
            _parent = parent;
            _autoExpand = autoExpand;
            _maxSize = maxSize;
            _initialSize = initialSize;
            _pool = new Queue<T>(initialSize);

            // 초기 풀 채우기
            for (int i = 0; i < initialSize; i++)
            {
                var obj = CreateNew();
                obj.gameObject.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        /// <summary>
        /// 풀에서 오브젝트를 꺼내 활성화하여 반환.
        /// 풀이 비었으면 autoExpand 설정에 따라 자동 확장하거나 경고 후 강제 생성.
        /// </summary>
        public T Get(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            T obj;
            if (_pool.Count > 0)
            {
                obj = _pool.Dequeue();
            }
            else if (_autoExpand && _totalCreated < _maxSize)
            {
                for(int i = 0; i < _initialSize && _totalCreated < _maxSize; i++)
                {
                    var newObj = CreateNew();
                    newObj.gameObject.SetActive(false);
                    _pool.Enqueue(newObj);
                }
                obj = _pool.Dequeue();
            }
            else
            {
                Debug.LogWarning($"[MIObjectPool<{typeof(T).Name}>] 풀 최대 크기({_maxSize}) 초과. 강제 생성합니다.");
                obj = CreateNew();
            }

            var t = obj.transform;
            t.SetParent(parent != null ? parent : _parent);
            t.SetPositionAndRotation(position, rotation);
            obj.gameObject.SetActive(true);
            return obj;
        }

        /// <summary>
        /// 오브젝트를 비활성화하고 풀에 반환.
        /// 이미 비활성화된 오브젝트(중복 반환)는 무시합니다.
        /// </summary>
        public void Return(T obj)
        {
            if (obj == null) return;

            // 이미 비활성화된 경우 중복 반환 방지
            if (_pool.Contains(obj)) return;

            obj.gameObject.SetActive(false);
            obj.transform.SetParent(_parent);
            _pool.Enqueue(obj);
        }

        private T CreateNew()
        {
            var go = Object.Instantiate(_prefab.gameObject, _parent);
            go.SetActive(false);
            _totalCreated++;

            return go.GetComponent<T>();
        }
    }
}
