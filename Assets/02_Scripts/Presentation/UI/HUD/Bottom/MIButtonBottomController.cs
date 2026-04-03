using MI.Utility;
using UnityEngine;

namespace MI.Presentation.UI.HUD.Bottom
{
    /// <summary>
    /// 하단 버튼 5개를 관리하는 컨트롤러.
    /// 항상 0개 또는 1개만 선택된 상태를 유지한다.
    /// </summary>
    public class MIButtonBottomController : MonoBehaviour
    {
        [SerializeField] private MIButtonBottomElement[] _buttons;

        private MIButtonBottomElement _currentSelected;

        private void Awake()
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                _buttons[i].Initialize(this);
            }
        }

        /// <summary>
        /// 버튼이 클릭되었을 때 호출된다.
        /// 이미 선택된 버튼을 다시 누르면 해제, 다른 버튼을 누르면 기존 해제 후 새로 선택.
        /// </summary>
        public void OnButtonClicked(MIButtonBottomElement clicked)
        {
            // 이미 선택된 버튼을 다시 누른 경우 — 해제
            if (_currentSelected == clicked)
            {
                _currentSelected.Deselect();
                _currentSelected = null;
                MILog.Log("버튼 선택 해제");
                return;
            }

            // 기존 선택된 버튼이 있으면 해제
            if (_currentSelected != null)
            {
                _currentSelected.Deselect();
            }

            // 새 버튼 선택
            clicked.Select();
            _currentSelected = clicked;
            MILog.Log("버튼 선택");
        }
    }
}
