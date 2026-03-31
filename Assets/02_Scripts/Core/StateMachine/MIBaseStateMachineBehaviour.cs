using MI.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

using Debug = UnityEngine.Debug;

namespace MI.Core.StateMachine
{
    // AnimatorStateInfo 전달용 UnityEvent. OnEnter/OnExit 인스펙터 연결 시 사용.
    [Serializable]
    public class MIAnimatorStateInfoEvent : UnityEvent<AnimatorStateInfo> { }

    // StateMachineBehaviour 범용 베이스. 이벤트 발행·디버그·컴포넌트 캐싱 제공.
    public abstract class MIBaseStateMachineBehaviour : StateMachineBehaviour
    {
        #region Events

        // 상태 진입 시 발생하는 C# 이벤트
        public event Action<AnimatorStateInfo> OnEnterState;

        // 상태 종료 시 발생하는 C# 이벤트
        public event Action<AnimatorStateInfo> OnExitState;

        [SerializeField]
        private MIAnimatorStateInfoEvent onEnterStateEvent = new();

        [SerializeField]
        private MIAnimatorStateInfoEvent onExitStateEvent = new();

        #endregion Events

        #region Debug

        [SerializeField] private bool enableDebugLog;

        #endregion Debug

        #region Component Cache

        private Dictionary<Type, Component> _componentCache;

        // 지정 타입 컴포넌트를 캐싱하여 반환. 첫 호출 시 Animator에서 탐색, 이후 캐시 재사용.
        protected TComponent GetCachedComponent<TComponent>(Animator animator)
            where TComponent : Component
        {
            _componentCache ??= new Dictionary<Type, Component>();

            var type = typeof(TComponent);
            if (!_componentCache.TryGetValue(type, out var cached))
            {
                cached = animator.GetComponent<TComponent>()
                      ?? (Component)animator.GetComponentInParent<TComponent>();
                _componentCache[type] = cached;

                if (cached == null)
                    MILog.LogError(
                        $"[BaseStateMachineBehaviour] '{typeof(TComponent).Name}' 컴포넌트를 찾을 수 없습니다." +
                        $" Animator: {animator.name}");
            }

            return cached as TComponent;
        }

        #endregion Component Cache

        #region StateMachineBehaviour Callbacks

        public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            LogCallback("OnStateEnter", stateInfo);
            OnEnterState?.Invoke(stateInfo);
            onEnterStateEvent?.Invoke(stateInfo);
            OnEnter(animator, stateInfo, layerIndex);
        }

        public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            LogCallback("OnStateExit", stateInfo);
            OnExitState?.Invoke(stateInfo);
            onExitStateEvent?.Invoke(stateInfo);
            OnExit(animator, stateInfo, layerIndex);
        }

        public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //LogCallback("OnStateUpdate", stateInfo);
            OnUpdate(animator, stateInfo, layerIndex);
        }

        public sealed override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //LogCallback("OnStateMove", stateInfo);
            OnMove(animator, stateInfo, layerIndex);
        }

        public sealed override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //LogCallback("OnStateIK", stateInfo);
            OnIK(animator, stateInfo, layerIndex);
        }

        public sealed override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            //LogStateMachineCallback("OnStateMachineEnter");
            OnMachineEnter(animator, stateMachinePathHash);
        }

        public sealed override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            //LogStateMachineCallback("OnStateMachineExit");
            OnMachineExit(animator, stateMachinePathHash);
        }

        #endregion StateMachineBehaviour Callbacks

        #region Virtual Callbacks

        // OnStateEnter 전달 콜백. 서브클래스 오버라이드용.
        protected virtual void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        // OnStateExit 전달 콜백. 서브클래스 오버라이드용.
        protected virtual void OnExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        // OnStateUpdate 전달 콜백. 서브클래스 오버라이드용.
        protected virtual void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        // OnStateMove 전달 콜백. 서브클래스 오버라이드용.
        protected virtual void OnMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        // OnStateIK 전달 콜백. 서브클래스 오버라이드용.
        protected virtual void OnIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        // OnStateMachineEnter 전달 콜백. 서브클래스 오버라이드용.
        protected virtual void OnMachineEnter(Animator animator, int stateMachinePathHash) { }

        // OnStateMachineExit 전달 콜백. 서브클래스 오버라이드용.
        protected virtual void OnMachineExit(Animator animator, int stateMachinePathHash) { }

        #endregion Virtual Callbacks

        #region Debug Logging

        [Conditional("UNITY_EDITOR")]
        private void LogCallback(string callbackName, AnimatorStateInfo stateInfo)
        {
            if (!enableDebugLog) return;
            MILog.Log(
                $"[BaseStateMachineBehaviour] {callbackName}" +
                $" | State: {stateInfo.shortNameHash}" +
                $" | NormalizedTime: {stateInfo.normalizedTime:F3}");
        }

        [Conditional("UNITY_EDITOR")]
        private void LogStateMachineCallback(string callbackName)
        {
            if (!enableDebugLog) return;
            MILog.Log($"[BaseStateMachineBehaviour] {callbackName}");
        }

        #endregion Debug Logging
    }

    // BaseStateMachineBehaviour 제네릭 확장. T 타입 MonoBehaviour를 Owner로 자동 캐싱.
    // OnEnterWithOwner 등에서 null 체크 없이 Owner 직접 사용 가능.
    // ⚠️ 동일 AnimatorController를 공유하는 Animator 간 SMB 인스턴스가 공유됨
    public abstract class MIBaseStateMachineBehaviour<T> : MIBaseStateMachineBehaviour
        where T : MonoBehaviour
    {
        #region Owner

        [Tooltip("true: GetComponentInParent로 검색 / false: GetComponent로 검색")]
        [SerializeField] private bool searchParent;

        // 캐싱된 MonoBehaviour 참조. WithOwner 콜백 진입 시 항상 유효.
        protected T Owner { get; private set; }

        private bool _isCached;

        // Owner 캐시 무효화. OnDestroy/OnDisable 등에서 호출해야 함.
        // 다음 OnStateEnter 시 자동 재캐싱.
        public void InvalidateOwnerCache()
        {
            Owner = null;
            _isCached = false;
        }

        #endregion Owner

        #region Caching

        private void EnsureOwner(Animator animator)
        {
            if (_isCached) return;
            _isCached = true;

            Owner = searchParent
                ? animator.GetComponentInParent<T>()
                : animator.GetComponent<T>();

            if (Owner == null)
                MILog.LogError(
                    $"[BaseStateMachineBehaviour<{typeof(T).Name}>] 대상 컴포넌트를 찾을 수 없습니다." +
                    $" Animator: {animator.name} | searchParent: {searchParent}");
        }

        #endregion Caching

        #region Base Callback Overrides

        protected override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            EnsureOwner(animator);
            if (Owner != null)
                OnEnterWithOwner(animator, stateInfo, Owner);
        }

        protected override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Owner != null)
                OnUpdateWithOwner(animator, stateInfo, Owner);
        }

        protected override void OnExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Owner != null)
                OnExitWithOwner(animator, stateInfo, Owner);
        }

        #endregion Base Callback Overrides

        #region Owner Callbacks

        // Owner 유효 시에만 호출. null 체크 불필요.
        protected virtual void OnEnterWithOwner(Animator animator, AnimatorStateInfo stateInfo, T owner) { }

        // Owner 유효 시에만 호출. null 체크 불필요.
        protected virtual void OnUpdateWithOwner(Animator animator, AnimatorStateInfo stateInfo, T owner) { }

        // Owner 유효 시에만 호출. null 체크 불필요.
        protected virtual void OnExitWithOwner(Animator animator, AnimatorStateInfo stateInfo, T owner) { }

        #endregion Owner Callbacks
    }
}
