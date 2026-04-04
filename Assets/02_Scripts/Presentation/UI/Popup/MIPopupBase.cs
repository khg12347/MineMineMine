using UnityEngine;

namespace MI.Presentation.UI.Popup
{
    /// <summary>
    /// 모든 팝업의 기본 클래스.
    /// 외부에서 OnOpenPopup / OnClosePopup으로 열고 닫으며,
    /// 하위 클래스에서 OpenPopup / ClosePopup을 override하여 추가 동작을 구현한다.
    /// </summary>
    public class MIPopupBase : MonoBehaviour
    {
        #region Public API

        /// <summary>팝업 열기. 내부 OpenPopup()을 호출한다.</summary>
        public void OnOpenPopup() => OpenPopup();

        /// <summary>팝업 닫기. 내부 ClosePopup()을 호출한다.</summary>
        public void OnClosePopup() => ClosePopup();

        #endregion Public API

        #region Protected 내부 구현

        /// <summary>팝업 열기 기본 구현. override하여 확장 가능.</summary>
        protected virtual void OpenPopup()
        {
            gameObject.SetActive(true);
        }

        /// <summary>팝업 닫기 기본 구현. override하여 확장 가능.</summary>
        protected virtual void ClosePopup()
        {
            gameObject.SetActive(false);
        }

        #endregion Protected 내부 구현
    }
}
