using MI.Data.UIRes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MI.Presentation.UI.Common
{

    [RequireComponent(typeof(Animator))]
    public class MINumberShaker : MonoBehaviour
    {
        private enum ENumberSize : byte
        {
            Big,
            Middle,
            Small
        }

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

        private static Func<ENumberSize, int, MIUINumberResources, Sprite> _getNumberSprite
        {
            get
            {
                return (size, num, numberResources) =>
                {
                    switch (size)
                    {
                        case ENumberSize.Big:
                            return numberResources.GetBigNum(num);
                        case ENumberSize.Middle:
                            return numberResources.GetMiddleNum(num);
                        case ENumberSize.Small:
                            return numberResources.GetSmallNum(num);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(size), size, null);
                    }
                };
            }
        }

        [SerializeField] private Image _imageNum;
        [SerializeField] private Animator _animator;
        private int _currentNum;

        private static readonly int s_tShake = Animator.StringToHash("tShake");

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


        public static void UpdateBigNumberDisplay(MINumberShaker[] numShakers, int targetNumber, MIUINumberResources numberResources, bool enableLastNum = false)
        {
            UpdateNumberDisplay(numShakers, targetNumber, numberResources, _getNumberSprite, ENumberSize.Big, enableLastNum);
        }

        public static void UpdateMidNumberDisplay(MINumberShaker[] numShakers, int targetNumber, MIUINumberResources numberResources, bool enableLastNum = false)
        {
            UpdateNumberDisplay(numShakers, targetNumber, numberResources, _getNumberSprite, ENumberSize.Middle, enableLastNum);
        }

        public static void UpdateSmallNumberDisplay(MINumberShaker[] numShakers, int targetNumber, MIUINumberResources numberResources, bool enableLastNum = false)
        {
            UpdateNumberDisplay(numShakers, targetNumber, numberResources, _getNumberSprite, ENumberSize.Small, enableLastNum);
        }

        private static void UpdateNumberDisplay(MINumberShaker[] numShakers, int targetNumber,
            MIUINumberResources numberResources,
            Func<ENumberSize, int, MIUINumberResources, Sprite> getSpriteFunc,
            ENumberSize size, bool enableLastNum)
        {
            int length = numShakers.Length;

            int msb = -1;
            for (int i = length - 1; i >= 0; i--)
            {
                int digit = GetDigit(targetNumber, i);
                if (digit > 0)
                {
                    msb = i; 
                    break;
                }
            }

            for (int i = length - 1; i >= 0; i--)
            {
                int digit = GetDigit(targetNumber, i);
                numShakers[i].UpdateNumSprite(digit, getSpriteFunc(size, digit, numberResources));

                bool active = (i == 0) ? (enableLastNum || targetNumber > 0) : i <= msb;
                numShakers[i].gameObject.SetActive(active);
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
