using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MI.Core.Pool
{
    /// <summary>
    /// 풀별 초기화 설정.
    /// InitialSize: 미리 생성해둘 오브젝트 수.
    /// GrowSize: autoExpand 시 한 번에 추가 생성할 단위.
    /// </summary>
    public struct FPoolConfig
    {
        public int InitialSize; // 초기 생성 수량
        public int GrowSize;    // 확장 단위
    }

    /// <summary>
    /// 프리팹별 오브젝트 풀을 관리하는 범용 매니저.
    /// 타일 등 특정 도메인에 대한 지식을 갖지 않으며, 제네릭 Get/Return 인터페이스만 제공.
    /// MISingleton 패턴으로 씬 전역에서 접근 가능.
    /// </summary>
    public class MIPoolManager : MISingleton<MIPoolManager>
    {
        [Title("풀 기본값 설정 (InitPool 미호출 시 적용)")]
        [InfoBox("타일 기준: 스테이지 폭 × 여유 행 수 (기본: 8열 × 24행 = 192)")]
        [PropertyRange(1, 256)]
        [SerializeField] private int _defaultInitialSize = 192;

        [InfoBox("autoExpand 시 한 번에 추가 생성할 기본 단위")]
        [PropertyRange(1, 128)]
        [SerializeField] private int _defaultGrowSize = 32;

        [InfoBox("풀 자동 확장 시 넘지 않을 최대 오브젝트 수")]
        [SerializeField] private int _maxPoolSize = 512;

        [ToggleLeft]
        [SerializeField] private bool _autoExpand = true;

        // 프리팹 instanceID → 풀 (object로 타입 소거하여 여러 타입 통합 관리)
        private readonly Dictionary<int, object> _pools = new();

        // 오브젝트 instanceID → 풀 역매핑 (Return 시 올바른 풀로 돌려보내기 위함)
        private readonly Dictionary<int, object> _instanceToPool = new();

        private Transform _poolRoot;

        protected override void Awake()
        {
            base.Awake();

            // 비활성 풀 오브젝트들을 담는 부모 생성
            _poolRoot = new GameObject("[PoolRoot]").transform;
            _poolRoot.SetParent(transform);
        }

        /// <summary>
        /// 실제 스폰 전에 풀을 미리 생성합니다 (워밍업).
        /// Get() 호출 전 Awake/Start 단계에서 호출하는 것을 권장합니다.
        /// 이미 해당 프리팹의 풀이 존재하면 무시됩니다.
        /// </summary>
        /// <param name="prefab">풀링할 프리팹</param>
        /// <param name="config">초기 사이즈 및 확장 단위 설정</param>
        public void InitPool<T>(GameObject prefab, FPoolConfig config) where T : Component
        {
            int prefabId = prefab.GetInstanceID();
            if (_pools.ContainsKey(prefabId))
            {
                Debug.LogWarning($"[MIPoolManager] '{prefab.name}' 풀이 이미 존재합니다. InitPool을 무시합니다.");
                return;
            }
            CreatePool<T>(prefab, prefabId, config.InitialSize, config.GrowSize);
        }

        /// <summary>
        /// 프리팹 T 컴포넌트에 대응하는 풀에서 오브젝트를 꺼냄.
        /// 해당 프리팹의 풀이 없으면 자동으로 생성.
        /// </summary>
        public T Get<T>(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
            where T : Component
        {
            int prefabId = prefab.GetInstanceID();
            if (!_pools.TryGetValue(prefabId, out var poolObj))
                poolObj = CreatePool<T>(prefab, prefabId, _defaultInitialSize, _defaultGrowSize);

            var pool = (MIObjectPool<T>)poolObj;
            var obj  = pool.Get(position, rotation, parent);

            // 역매핑 등록 (같은 인스턴스 반복 등록 시 덮어쓰기로 문제 없음)
            _instanceToPool[obj.GetInstanceID()] = pool;
            return obj;
        }

        /// <summary>
        /// 오브젝트를 대응하는 풀에 반환.
        /// 이미 비활성화된 오브젝트(중복 반환)는 MIObjectPool 내부에서 무시됨.
        /// </summary>
        public void Return<T>(T obj) where T : Component
        {
            if (obj == null) return;

            if (_instanceToPool.TryGetValue(obj.GetInstanceID(), out var poolObj))
            {
                ((MIObjectPool<T>)poolObj).Return(obj);
            }
            else
            {
                Debug.LogWarning($"[MIPoolManager] '{obj.name}'에 대응하는 풀을 찾지 못했습니다. 비활성화만 처리합니다.");
                obj.gameObject.SetActive(false);
            }
        }

        private object CreatePool<T>(GameObject prefab, int prefabId, int initialSize, int growSize) where T : Component
        {
            var prefabComp = prefab.GetComponent<T>();
            if (prefabComp == null)
            {
                Debug.LogError($"[MIPoolManager] '{prefab.name}' 프리팹에 {typeof(T).Name} 컴포넌트가 없습니다.");
                return null;
            }

            // 프리팹별 부모 오브젝트 생성 (하이어라키 가독성 향상)
            var poolParent = new GameObject($"[Pool] {prefab.name} <{typeof(T).Name}>").transform;
            poolParent.SetParent(_poolRoot);

            var pool = new MIObjectPool<T>(prefabComp, poolParent, initialSize, growSize, _autoExpand, _maxPoolSize);
            _pools[prefabId] = pool;
            return pool;
        }

#if UNITY_EDITOR
        [Title("풀 현황 (에디터 전용)")]
        [Button("풀 상태 출력")]
        private void PrintPoolStatus()
        {
            if (_pools.Count == 0)
            {
                Debug.Log("[MIPoolManager] 등록된 풀이 없습니다.");
                return;
            }
            foreach (var kv in _pools)
            {
                // CountAll/CountActive/CountInactive는 타입별로 읽기 어려우므로 이름만 출력
                Debug.Log($"[MIPoolManager] prefabID={kv.Key} pool={kv.Value?.GetType().Name}");
            }
        }
#endif
    }
}
