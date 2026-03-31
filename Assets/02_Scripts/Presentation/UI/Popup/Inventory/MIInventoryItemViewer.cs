using MI.Data.UIRes;
using MI.Domain.Inventory;
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

        public void InitializeData(MIUINumberResources numberResources)
        {
            _numberResources = numberResources;
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
            MINumberShaker.UpdateNumberDisplay(_amountNumbers, amount, _numberResources);
        }
        #endregion IMIItemViewer Implementation
    }
}
