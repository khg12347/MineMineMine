using MI.Utility;
using System;

namespace MI.Core
{
    public sealed class MIGameManager : MISingleton<MIGameManager>
    {
        public event Action<int> OnScoreChanged;

        private int _currentScore;
        public int CurrentScore => _currentScore;

        private void OnApplicationQuit()
        {
            MIAppLifeTime.OnApplicationQuit();
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
