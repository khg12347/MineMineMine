using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using MI.Data.UIRes;
using MI.Data.User.Inventory;
using MI.Presentation.UI.Interface;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MI.Presentation.UI.HUD.Items
{
    [RequireComponent(typeof(CanvasGroup))]
    public class MIDropItemViewer : MonoBehaviour, IMIItemViewer
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
            //StartCoroutine(ShowViwerCo());
            ShowViewerAsync().Forget();
        }
        private async UniTask ShowViewerAsync()
        {
            var token = this.GetCancellationTokenOnDestroy();

            try
            {
                _canvasGroup.alpha = 1f;
                await UniTask.Delay(TimeSpan.FromSeconds(_initialViewDelay), cancellationToken: token);
                var alpha = 1f;

                while (alpha > 0f)
                {
                    alpha -= Time.deltaTime / _hideDuration;
                    _canvasGroup.alpha = alpha;
                    await UniTask.Yield(cancellationToken: token);
                }
                OnHideAction?.Invoke(gameObject);
                OnHideAction = null;

            }
            catch (OperationCanceledException)
            {
                // �ı��� ���� ���
            }
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
