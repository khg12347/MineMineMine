using UnityEngine;

namespace MI.Presentation.UI.Popup.Enhance
{
    public class MIEnhanceAnimationEvent : MonoBehaviour
    {
        private MIEnhanceTab _enhanceTab;
        private void Awake()
        {
            _enhanceTab = GetComponentInParent<MIEnhanceTab>();
        }

        public void OnAnimationComplete()
        {
            _enhanceTab.OnAnimationComplete();
        }
    }
}
