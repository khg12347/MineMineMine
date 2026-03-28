using System;
using System.Collections;
using MI.Data.UIRes;
using MI.Domain.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MI.Presentation.UI.HUD.Items
{
    [RequireComponent(typeof(CanvasGroup))]
    public class MIDropItemViewer : MonoBehaviour,  IMIItemViewer
    {
        [SerializeField] private Image _imageIcon;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private string _textFormat = "+{0}";
        [SerializeField] private float _initialViewDelay = 0.5f;
        [SerializeField] private float _hideDuration = 1f;
        private MIItemIconDataTable _iconDataTable;

        private CanvasGroup _canvasGroup;

        public event Action<GameObject> OnHideAction;
        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        private void OnEnable()
        {
            StartCoroutine(ShowViwerCo());
        }

        private IEnumerator ShowViwerCo()
        {
            _canvasGroup.alpha = 1f;
            yield return new WaitForSecondsRealtime(_hideDuration);
            var alpha = 1f;
            
            while (alpha > 0f)
            {
                alpha -= Time.deltaTime / _hideDuration;
                _canvasGroup.alpha = alpha;
                yield return null;
            }
            OnHideAction?.Invoke(gameObject);
            OnHideAction = null;
        }
        
        #region IMIItemViewer Implementation
        public void SetIconDataTable(MIItemIconDataTable dataTable)
        {
            _iconDataTable = dataTable;
        }
        
        public void UpdateItemViewer(EItemType itemType, int amount)
        {
            if (itemType == EItemType.None || amount <= 0) return;
            
            _imageIcon.sprite = _iconDataTable.GetItemIcon(itemType);
            _text.text = string.Format(_textFormat, amount);
        }
        #endregion IMIItemViewer Implementation
    }
}
