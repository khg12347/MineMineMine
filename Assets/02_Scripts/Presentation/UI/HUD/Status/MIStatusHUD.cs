using MI.Core;
using MI.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace MI.Presentation.UI.HUD.Status
{

    public class MIStatusHUD : MonoBehaviour, IMIStatusListener
    {
        [SerializeField] private Slider _expSlider;
        [SerializeField] private TMPro.TextMeshProUGUI _levelText;

        private string _levelFormat = "Lv.{0}";

        private void Start()
        {
            _expSlider.value = MIStatusManager.Instance.ExpRatio;
            _levelText.text = string.Format(_levelFormat, MIStatusManager.Instance.CurrentLevel);

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
            _levelText.text = string.Format(_levelFormat, newLevel);
        }
    }
}