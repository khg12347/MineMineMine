using UnityEngine;
using UnityEngine.UI;

namespace MI.Presentation.UI.Common
{
    [RequireComponent(typeof(Animator))]
    public class MINumberShaker : MonoBehaviour
    {
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
    }
}
