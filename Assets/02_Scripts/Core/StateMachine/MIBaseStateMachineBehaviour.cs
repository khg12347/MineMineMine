using MI.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

using Debug = UnityEngine.Debug;

namespace MI.Core.StateMachine
{
    /// <summary>
    /// AnimatorStateInfo를 파라미터로 전달하는 직렬화 가능한 UnityEvent.
    /// 인스펙터에서 OnEnter / OnExit 이벤트를 연결할 때 사용합니다.
    /// </summary>
    [Serializable]
    public class MIAnimatorStateInfoEvent : UnityEvent<AnimatorStateInfo> { }

    /// <summary>
    /// StateMachineBehaviour의 범용 베이스 클래스.
    /// <para>이벤트 발행, 디버그 로깅, 다중 컴포넌트 캐싱 기능을 제공합니다.</para>
    /// <para>캐릭터 애니메이션, 월드 오브젝트, UI 등 다양한 Animator 상태 머신의 공통 부모로 사용됩니다.</para>
    /// </summary>
    public abstract class MIBaseStateMachineBehaviour : StateMachineBehaviour
    {
        // ────────────────────────────────────────────────────────────────
        #region 이벤트

        /// <summary>상태 진입 시 호출되는 C# 이벤트. AnimatorStateInfo를 인자로 전달합니다.</summary>
        public event Action<AnimatorStateInfo> OnEnterState;

        /// <summary>상태 종료 시 호출되는 C# 이벤트. AnimatorStateInfo를 인자로 전달합니다.</summary>
        public event Action<AnimatorStateInfo> OnExitState;

        [SerializeField]
        private MIAnimatorStateInfoEvent onEnterStateEvent = new();

        [SerializeField]
        private MIAnimatorStateInfoEvent onExitStateEvent = new();

        #endregion

        // ────────────────────────────────────────────────────────────────
        #region 디버그

        [SerializeField] private bool enableDebugLog;

        #endregion

        // ────────────────────────────────────────────────────────────────
        #region 다중 컴포넌트 캐시

        private Dictionary<Type, Component> _componentCache;

        /// <summary>
        /// 지정 타입의 컴포넌트를 캐싱하여 반환합니다.
        /// 첫 호출 시 Animator GameObject와 부모에서 검색하고, 이후에는 캐시를 재사용합니다.
        /// 제네릭 버전과 비제네릭 버전 모두에서 사용 가능합니다.
        /// </summary>
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

        #endregion

        // ────────────────────────────────────────────────────────────────
        #region StateMachineBehaviour 콜백

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

        #endregion

        // ────────────────────────────────────────────────────────────────
        #region 서브클래스용 가상 콜백

        /// <summary>OnStateEnter에서 호출됩니다. 서브클래스에서 오버라이드하세요.</summary>
        protected virtual void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        /// <summary>OnStateExit에서 호출됩니다. 서브클래스에서 오버라이드하세요.</summary>
        protected virtual void OnExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        /// <summary>OnStateUpdate에서 호출됩니다. 서브클래스에서 오버라이드하세요.</summary>
        protected virtual void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        /// <summary>OnStateMove에서 호출됩니다. 서브클래스에서 오버라이드하세요.</summary>
        protected virtual void OnMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        /// <summary>OnStateIK에서 호출됩니다. 서브클래스에서 오버라이드하세요.</summary>
        protected virtual void OnIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

        /// <summary>OnStateMachineEnter에서 호출됩니다. 서브클래스에서 오버라이드하세요.</summary>
        protected virtual void OnMachineEnter(Animator animator, int stateMachinePathHash) { }

        /// <summary>OnStateMachineExit에서 호출됩니다. 서브클래스에서 오버라이드하세요.</summary>
        protected virtual void OnMachineExit(Animator animator, int stateMachinePathHash) { }

        #endregion

        // ────────────────────────────────────────────────────────────────
        #region 디버그 로깅

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

        #endregion
    }

    // ════════════════════════════════════════════════════════════════════

    /// <summary>
    /// BaseStateMachineBehaviour의 제네릭 확장.
    /// <para>T 타입의 MonoBehaviour를 Animator에서 자동 캐싱하여 <see cref="Owner"/> 프로퍼티로 제공합니다.</para>
    /// <para>서브클래스에서는 <see cref="OnEnterWithOwner"/>, <see cref="OnUpdateWithOwner"/>, <see cref="OnExitWithOwner"/>를 오버라이드하여
    /// null 체크 없이 Owner를 직접 사용할 수 있습니다.</para>
    /// <para>⚠️ Unity는 동일한 AnimatorController를 참조하는 Animator 간에 SMB 인스턴스를 공유합니다.
    /// 여러 Animator가 같은 컨트롤러를 사용한다면 Owner 캐시가 마지막 Animator를 참조할 수 있습니다.</para>
    /// </summary>
    /// <typeparam name="T">연결할 MonoBehaviour 타입. 예: <c>PlayerController</c></typeparam>
    public abstract class MIBaseStateMachineBehaviour<T> : MIBaseStateMachineBehaviour
        where T : MonoBehaviour
    {
        // ────────────────────────────────────────────────────────────────
        #region Owner 설정

        [Tooltip("true: GetComponentInParent로 검색 / false: GetComponent로 검색")]
        [SerializeField] private bool searchParent;

        /// <summary>
        /// 캐싱된 MonoBehaviour 참조.
        /// OnEnterWithOwner / OnUpdateWithOwner / OnExitWithOwner 호출 시점에는 반드시 유효합니다.
        /// </summary>
        protected T Owner { get; private set; }

        private bool _isCached;

        /// <summary>
        /// Owner 캐시를 초기화합니다.
        /// Owner(MonoBehaviour)의 OnDestroy, OnDisable 등에서 호출하여 캐시를 무효화하세요.
        /// 다음 OnStateEnter 시점에 자동으로 재캐싱됩니다.
        /// </summary>
        public void InvalidateOwnerCache()
        {
            Owner = null;
            _isCached = false;
        }

        #endregion

        // ────────────────────────────────────────────────────────────────
        #region 캐싱 로직

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

        #endregion

        // ────────────────────────────────────────────────────────────────
        #region 베이스 콜백 오버라이드 (Owner 캐싱 + 안전 콜백 연결)

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

        #endregion

        // ────────────────────────────────────────────────────────────────
        #region Owner 보장 콜백

        /// <summary>
        /// Owner가 유효할 때만 호출되는 안전한 Enter 콜백.
        /// null 체크 없이 <paramref name="owner"/>를 직접 사용하세요.
        /// </summary>
        protected virtual void OnEnterWithOwner(Animator animator, AnimatorStateInfo stateInfo, T owner) { }

        /// <summary>
        /// Owner가 유효할 때만 호출되는 안전한 Update 콜백.
        /// null 체크 없이 <paramref name="owner"/>를 직접 사용하세요.
        /// </summary>
        protected virtual void OnUpdateWithOwner(Animator animator, AnimatorStateInfo stateInfo, T owner) { }

        /// <summary>
        /// Owner가 유효할 때만 호출되는 안전한 Exit 콜백.
        /// null 체크 없이 <paramref name="owner"/>를 직접 사용하세요.
        /// </summary>
        protected virtual void OnExitWithOwner(Animator animator, AnimatorStateInfo stateInfo, T owner) { }

        #endregion
    }
}
