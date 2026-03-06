using System;
using UnityEngine;

namespace MI.Core
{
    public class MIGameManager : MonoBehaviour
    {
        public static MIGameManager Instance { get; private set; }

        public event Action<int> OnScoreChanged;

        private int _currentScore;
        public int CurrentScore => _currentScore;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void AddScore(int amount)
        {
            _currentScore += amount;
            OnScoreChanged?.Invoke(_currentScore);
        }

        public void ResetGame()
        {
            _currentScore = 0;
            OnScoreChanged?.Invoke(_currentScore);
        }
    }
}
