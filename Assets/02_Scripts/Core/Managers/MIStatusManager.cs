using System.Collections.Generic;
using MI.Data.Config;
using MI.Domain.Status;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MI.Core
{
    /// <summary>
    /// 플레이어의 EXP / 레벨을 관리하는 싱글톤 매니저.
    ///
    /// ── 외부 연동 방법 ────────────────────────────────────────────────
    ///
    ///  [EXP 추가]
    ///    MIStatusManager.Instance.AddExp(50);
    ///
    ///  [상태 조회]
    ///    int   level    = MIStatusManager.Instance.CurrentLevel;
    ///    int   exp      = MIStatusManager.Instance.CurrentExp;
    ///    float ratio    = MIStatusManager.Instance.ExpRatio;
    ///    var   snapshot = MIStatusManager.Instance.TakeSnapshot();
    ///
    ///  [UI 리스너 등록 (IMIStatusListener 구현 후)]
    ///    MIStatusManager.Instance.RegisterListener(this);
    ///    MIStatusManager.Instance.UnregisterListener(this);   // OnDestroy에서 해제
    /// </summary>
    public sealed class MIStatusManager : MISingleton<MIStatusManager>
    {
        // ── Inspector ─────────────────────────────────────────────────

        [Title("설정")]
        [Required]
        [SerializeField] private MIStatusConfig _config;


        // ── 런타임 상태 ───────────────────────────────────────────────

        private int  _currentLevel = 1;
        private int  _currentExp;
        private long _totalExp;

        private readonly List<IMIStatusListener> _listeners = new();

        // ── 프로퍼티 ──────────────────────────────────────────────────

        /// <summary>현재 레벨</summary>
        public int CurrentLevel => _currentLevel;

        /// <summary>현재 레벨 내 누적 EXP</summary>
        public int CurrentExp => _currentExp;

        /// <summary>현재 레벨에서 다음 레벨까지 필요한 EXP</summary>
        public int RequiredExp => _config != null ? _config.GetRequiredExp(_currentLevel) : 0;

        /// <summary>현재 레벨 내 EXP 진행률 [0, 1]</summary>
        public float ExpRatio => RequiredExp > 0 ? (float)_currentExp / RequiredExp : 1f;

        /// <summary>게임 시작 이후 획득한 총 누적 EXP</summary>
        public long TotalExp => _totalExp;


        // ── 공개 API ──────────────────────────────────────────────────

        /// <summary>
        /// EXP를 추가하고 레벨업을 자동 처리한다.
        /// 한 번에 여러 레벨을 건너뛰는 경우도 순서대로 처리된다.
        /// </summary>
        public void AddExp(int amount)
        {
            if (amount <= 0) return;

            _currentExp += amount;
            _totalExp   += amount;

            ProcessLevelUp();
            NotifyExpChanged();
        }

        /// <summary>
        /// 특정 레벨에서 다음 레벨로 진급하는 데 필요한 EXP를 반환.
        /// Config의 테이블 범위를 벗어나면 배율 계산 값을 반환.
        /// </summary>
        public int GetRequiredExp(int level) => _config.GetRequiredExp(level);

        /// <summary>현재 상태의 불변 스냅샷을 반환 (UI 갱신, 저장 등에 활용).</summary>
        public FStatusSnapshot TakeSnapshot()
            => new FStatusSnapshot(_currentLevel, _currentExp, RequiredExp, _totalExp);

        /// <summary>EXP와 레벨을 초기 상태로 되돌린다.</summary>
        public void Reset()
        {
            _currentLevel = 1;
            _currentExp   = 0;
            _totalExp     = 0;
            NotifyExpChanged();
        }

        // ── 리스너 관리 ───────────────────────────────────────────────

        /// <summary>상태 변경 알림을 수신할 리스너를 등록한다. 중복 등록은 무시.</summary>
        public void RegisterListener(IMIStatusListener listener)
        {
            if (listener == null || _listeners.Contains(listener)) return;
            _listeners.Add(listener);
        }

        /// <summary>리스너를 해제한다. OnDestroy 등에서 반드시 호출.</summary>
        public void UnregisterListener(IMIStatusListener listener)
        {
            _listeners.Remove(listener);
        }

        // ── 내부 처리 ─────────────────────────────────────────────────

        /// <summary>
        /// 현재 EXP가 RequiredExp 이상이면 레벨업을 반복 처리.
        /// 다중 레벨업(EXP 폭발적 획득)도 누락 없이 처리된다.
        /// </summary>
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
            int   current  = _currentExp;
            int   required = RequiredExp;
            float ratio    = ExpRatio;

            // 역방향 순회: 콜백 내부에서 UnregisterListener가 호출돼도 안전
            for (int i = _listeners.Count - 1; i >= 0; i--)
                _listeners[i].OnExpChanged(current, required, ratio);
        }

        private void NotifyLevelUp(int newLevel)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
                _listeners[i].OnLevelUp(newLevel);
        }

        // ── 에디터 전용 ───────────────────────────────────────────────

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
