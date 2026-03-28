using MI.Data.UIRes;
using MI.Domain.Status;
using MI.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace MI.Presentation.UI.HUD.Status
{

    public class MIStatusHUD : MonoBehaviour, IMIStatusListener
    {
        [SerializeField] private MIUINumberResources _numberResources;
        [SerializeField] private Slider _expSlider;
        [SerializeField] private Image[] _numbers; // ���� ���� �̹��� �迭 (�ִ� 3�ڸ�)
        [SerializeField] private Image[] _depthNumbers; // ���� ���� �̹��� �迭 (�ִ� 7�ڸ�)

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
            UpdateDepthDisplay(newDepth);
        }

        private void UpdateLevelDisplay(int level)
        {
            int[] nums = new int[3];

            for (int i = 2; i >= 0; i--)
            {
                nums[i] = GetDigit(level, i);
                _numbers[i].sprite = _numberResources.GetMiddleNum(nums[i]);

                if (i == 0)
                    continue;
                if(i == 2)
                {
                    _numbers[i].gameObject.SetActive(nums[i] > 0);
                }
                else
                {
                    _numbers[i].gameObject.SetActive(nums[i] > 0 || nums[i + 1] > 0);
                }
            }
        }
        private void UpdateDepthDisplay(int depth)
        {
            int[] nums = new int[7];

            for (int i = 6; i >= 0; i--)
            {
                nums[i] = GetDigit(depth, i);
                _depthNumbers[i].sprite = _numberResources.GetBigNum(nums[i]);

                if (i == 0)
                    continue;

                if (i == 6)
                {
                    _depthNumbers[i].gameObject.SetActive(nums[i] > 0);
                }
                else
                {
                    _depthNumbers[i].gameObject.SetActive(nums[i] > 0 || nums[i + 1] > 0);
                }
            }
        }
        private int GetDigit(int value, int digit)
        {
            int pow = (int)Mathf.Pow(10, digit);
            return (value / pow) % 10;
        }

    }
}