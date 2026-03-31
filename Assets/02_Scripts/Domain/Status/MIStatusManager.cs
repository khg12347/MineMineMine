using System.Collections.Generic;
using MI.Core;
using MI.Data.Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MI.Domain.Status
{
    // EXP/레벨 관리 싱글톤
    // AddExp()로 EXP 추가, RegisterListener()로 UI 알림 수신
    public sealed class MIStatusManager : MISingleton<MIStatusManager>
    {
        // Inspector
        [Title("설정")]
        [Required]
        [SerializeField] private MIStatusConfig _config;

        // 런타임 상태
        private int _currentLevel = 1;
        private int _currentDepth = 0;
        private int _currentExp;
        private long _totalExp;

        private readonly List<IMIStatusListener> _listeners = new();

        // 프로퍼티
        // 현재 레벨
        public int CurrentLevel => _currentLevel;

        // 현재 레벨 내 누적 EXP
        public int CurrentExp => _currentExp;

        // 다음 레벨까지 필요한 EXP
        public int RequiredExp => _config != null ? _config.GetRequiredExp(_currentLevel) : 0;

        // EXP 진행률 [0, 1]
        public float ExpRatio => RequiredExp > 0 ? (float)_currentExp / RequiredExp : 1f;

        // 게임 시작 후 총 누적 EXP
        public long TotalExp => _totalExp;

        public int CurrentDepth => _currentDepth; // TODO: 깊이 시스템 도입 시 구현

        #region Public API

        // EXP 추가 + 다중 레벨업 자동 처리
        public void AddExp(int amount)
        {
            if (amount <= 0) return;

            _currentExp += amount;
            _totalExp += amount;

            ProcessLevelUp();
            NotifyExpChanged();
        }

        public void UpdateDepth(int newDepth)
        {
            if (newDepth <= 0 || newDepth == _currentDepth) return;
            _currentDepth = newDepth;
            NotifyUpdateDepth(newDepth);
        }

        public int GetRequiredExp(int level) => _config.GetRequiredExp(level);

        // 현재 상태 불변 스냅샷 반환
        public FStatusSnapshot TakeSnapshot()
            => new FStatusSnapshot(_currentLevel, _currentExp, RequiredExp, _totalExp);

        // EXP·레벨 초기화
        public void Reset()
        {
            _currentLevel = 1;
            _currentExp = 0;
            _totalExp = 0;
            NotifyExpChanged();
        }

        #endregion Public API

        #region Listener Management

        // 리스너 등록. 중복 무시.
        public void RegisterListener(IMIStatusListener listener)
        {
            if (listener == null || _listeners.Contains(listener)) return;
            _listeners.Add(listener);
        }

        // 리스너 해제. OnDestroy에서 호출해야 함.
        public void UnregisterListener(IMIStatusListener listener)
        {
            _listeners.Remove(listener);
        }

        #endregion Listener Management

        #region Internal

        // RequiredExp 이상이면 레벨업 반복 처리. 다중 레벨업도 누락 없음.
        private void ProcessLevelUp()
        {
            while (_currentExp >= _config.GetRequiredExp(_currentLevel))
            {
                _currentExp -= _config.GetRequiredExp(_currentLevel);
                _currentLevel++;
                NotifyLevelUp(_currentLevel);
            }
        }

        private void NotifyExpChanged()
        {
            int current = _currentExp;
            int required = RequiredExp;
            float ratio = ExpRatio;

            // 역방향 순회: 콜백 내부에서 UnregisterListener가 호출돼도 안전
            for (int i = _listeners.Count - 1; i >= 0; i--)
                _listeners[i].OnExpChanged(current, required, ratio);
        }

        private void NotifyLevelUp(int newLevel)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
                _listeners[i].OnLevelUp(newLevel);
        }

        private void NotifyUpdateDepth(int newDepth)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
                _listeners[i].OnDepthUpdated(newDepth);
        }

        #endregion Internal

#if UNITY_EDITOR
        [Title("디버그")]
        [Button("EXP +100 테스트"), PropertyOrder(99)]
        private void Debug_AddExp100() => AddExp(100);

        [Button("상태 초기화"), PropertyOrder(99)]
        private void Debug_Reset() => Reset();

        [ShowInInspector, ReadOnly, PropertyOrder(98)]
        [LabelText("현재 상태 미리보기")]
        private string Debug_StatusPreview =>
            $"Lv.{_currentLevel}  EXP {_currentExp} / {RequiredExp}  (총 {_totalExp})";
#endif
    }
}
