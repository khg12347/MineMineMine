using MI.Data.UIRes;
using MI.Domain.Status;
using MI.Presentation.UI.Common;
using MI.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace MI.Presentation.UI.HUD.Status
{

    public class MIStatusHUD : MonoBehaviour, IMIStatusListener
    {
        [SerializeField] private MIUINumberResources _numberResources;
        [SerializeField] private Slider _expSlider;
        [SerializeField] private MINumberShaker[] _numbers; // 레벨 숫자 이미지 배열 (최대 3자리)
        [SerializeField] private MINumberShaker[] _depthNumbers; // 깊이 숫자 이미지 배열 (최대 7자리)

        private void Start()
        {
            _expSlider.value = MIStatusManager.Instance.ExpRatio;
            var level = MIStatusManager.Instance.CurrentLevel;
            UpdateLevelDisplay(level);

            MIStatusManager.Instance.RegisterListener(this);
        }

        private void OnDestroy()
        {
            if(MIAppLifeTime.IsQuitting) return;

            MIStatusManager.Instance.UnregisterListener(this);
        
        }

        public void OnExpChanged(int currentExp, int requiredExp, float ratio)
        {
            _expSlider.value = ratio;
        }

        public void OnLevelUp(int newLevel)
        {
            UpdateLevelDisplay(newLevel);
        }
        public void OnDepthUpdated(int newDepth)
        {
            MINumberShaker.UpdateBigNumberDisplay(_depthNumbers, newDepth, _numberResources);
        }

        private void UpdateLevelDisplay(int level)
        {
            MINumberShaker.UpdateMidNumberDisplay(_numbers, level, _numberResources);
        }
    }
}