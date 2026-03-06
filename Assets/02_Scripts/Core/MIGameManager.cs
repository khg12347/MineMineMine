using System;

namespace MI.Core
{
    public sealed class MIGameManager : MISingleton
    {
        public event Action<int> OnScoreChanged;

        private int _currentScore;
        public int CurrentScore => _currentScore;

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
