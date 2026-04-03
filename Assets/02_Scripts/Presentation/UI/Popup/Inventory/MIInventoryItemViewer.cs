using MI.Data.UIRes;
using MI.Domain.UserState.Inventory;
using MI.Presentation.UI.Common;
using MI.Presentation.UI.Interface;
using System;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

namespace MI.Presentation.UI.Popup.Inventory
{
    public class MIInventoryItemViewer : MonoBehaviour, IMIItemViewer
    {
        [SerializeField] MINumberShaker[] _amountNumbers;
        [SerializeField] private Image _imageItemIcon;

        private MIUINumberResources _numberResources;
        private MIItemIconDataTable _itemIconDataTable;

        /// <summary>
        /// 리소스 초기화. 인벤토리 팝업에서 주입받음
        /// </summary>
        /// <param name="numberResources"></param>
        /// <param name="dataTable"></param>
        public void InitializeData(MIUINumberResources numberResources, MIItemIconDataTable dataTable)
        {
            _numberResources = numberResources;
            SetIconDataTable(dataTable);
        }

        #region IMIItemViewer Implementation
        public event Action<GameObject> OnHideAction;
        public void SetIconDataTable(MIItemIconDataTable dataTable)
        {
            _itemIconDataTable = dataTable;
        }

        public void UpdateItemViewer(EItemType itemType, int amount)
        {
            _imageItemIcon.sprite = _itemIconDataTable.GetItemIcon(itemType);
            MINumberShaker.UpdateSmallNumberDisplay(_amountNumbers, amount, _numberResources);

            _amountNumbers[0].gameObject.SetActive(amount > 0);
        }
        #endregion IMIItemViewer Implementation
    }
}
