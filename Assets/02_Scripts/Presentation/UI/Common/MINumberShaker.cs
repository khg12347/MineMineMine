using MI.Data.UIRes;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MI.Presentation.UI.Common
{
    [RequireComponent(typeof(Animator))]
    public class MINumberShaker : MonoBehaviour
    {
        private static Dictionary<int, int> s_pow = new Dictionary<int, int>
        {
            { 0, 1 },
            { 1, 10 },
            { 2, 100 },
            { 3, 1000 },
            { 4, 10000 },
            { 5, 100000 },
            { 6, 1000000 },
            { 7, 10000000 },
            { 8, 100000000 },
            { 9, 1000000000}
        };

        [SerializeField] private Image _imageNum;
        [SerializeField] private Animator _animator;
        private int _currentNum;

        private readonly int s_tShake = Animator.StringToHash("tShake");

        public void UpdateNumSprite(int num, Sprite sprite)
        {
            if (num == _currentNum) return;
            _currentNum = num;
            _imageNum.sprite = sprite;
            Shake(); // 숫자가 변경될 때만 흔들기
        }

        private void Shake()
        {
            _animator.SetTrigger(s_tShake);
        }


        public static void UpdateBigNumberDisplay(MINumberShaker[] numShakers, int targetNumber, MIUINumberResources numberResources)
        {
            int length = numShakers.Length;
            int[] nums = new int[length];

            for (int i = length - 1; i >= 0; i--)
            {
                nums[i] = GetDigit(targetNumber, i);
                numShakers[i].UpdateNumSprite(nums[i], numberResources.GetBigNum(nums[i]));

                if (i == 0)
                    continue;

                if (i == length - 1)
                {
                    numShakers[i].gameObject.SetActive(nums[i] > 0);
                }
                else
                {
                    numShakers[i].gameObject.SetActive(nums[i] > 0 || nums[i + 1] > 0);
                }
            }
        }

        public static void UpdateMidNumberDisplay(MINumberShaker[] numShakers, int targetNumber, MIUINumberResources numberResources)
        {
            int length = numShakers.Length;
            int[] nums = new int[length];

            for (int i = length - 1; i >= 0; i--)
            {
                nums[i] = GetDigit(targetNumber, i);
                numShakers[i].UpdateNumSprite(nums[i], numberResources.GetMiddleNum(nums[i]));

                if (i == 0)
                    continue;

                if (i == length - 1)
                {
                    numShakers[i].gameObject.SetActive(nums[i] > 0);
                }
                else
                {
                    numShakers[i].gameObject.SetActive(nums[i] > 0 || nums[i + 1] > 0);
                }
            }
        }

        public static void UpdateSmallNumberDisplay(MINumberShaker[] numShakers, int targetNumber, MIUINumberResources numberResources)
        {
            int length = numShakers.Length;
            int[] nums = new int[length];
            for (int i = length - 1; i >= 0; i--)
            {
                nums[i] = GetDigit(targetNumber, i);
                numShakers[i].UpdateNumSprite(nums[i], numberResources.GetSmallNum(nums[i]));
                if (i == 0)
                    continue;
                if (i == length - 1)
                {
                    numShakers[i].gameObject.SetActive(nums[i] > 0);
                }
                else
                {
                    numShakers[i].gameObject.SetActive(nums[i] > 0 || nums[i + 1] > 0);
                }
            }
        }

        private static int GetDigit(int value, int digit)
        {
            //자리수 초과시 에러 처리
            if (digit > 9) { throw new System.ArgumentOutOfRangeException(nameof(digit), "Digit must be between 0 and 9."); }
            //int pow = (int)Mathf.Pow(10, digit);
            int cachePow = s_pow[digit];
            return (value / cachePow) % 10;
        }
    }
}
