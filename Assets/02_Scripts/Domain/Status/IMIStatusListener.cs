using UnityEngine;

namespace MI.Domain.Status
{
    public interface IMIStatusListener
    {
        void OnExpChanged(int currentExp, int requiredExp, float ratio);

        void OnLevelUp(int newLevel);

        void OnDepthUpdated(int newDepth);
    }
}
