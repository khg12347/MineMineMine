using MI.Presentation.UI.Popup;
using MI.Utility;
using UnityEngine;

namespace MI.Presentation.UI.HUD.Bottom
{
    /// <summary>
    /// HUD 하단의 버튼 요소를 담당하는 클래스
    /// </summary>
    public class MIButtonBottomElement : MonoBehaviour
    {
        [SerializeField] private MIPopupBase _popup;

        private Animator _animator;
        private MIButtonBottomController _controller;
        private static readonly int s_bSelected = Animator.StringToHash("bSelected");

        public bool IsSelected { get; private set; }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// 컨트롤러 참조를 설정
        /// </summary>
        public void Initialize(MIButtonBottomController controller)
        {
            _controller = controller;
        }

        public void OnClickButtonBottom()
        {
            MILog.Log("OnClickButtonBottom");
            _controller.OnButtonClicked(this);
        }

        public void Select()
        {
            IsSelected = true;
            _animator.SetBool(s_bSelected, true);
            _popup?.OnOpenPopup();
        }

        public void Deselect()
        {
            IsSelected = false;
            _animator.SetBool(s_bSelected, false);
            _popup?.OnClosePopup();
        }
    }
}
