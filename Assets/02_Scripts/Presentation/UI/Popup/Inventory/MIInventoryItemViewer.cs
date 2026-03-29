using System;
using MI.Data.UIRes;
using MI.Domain.Inventory;
using MI.Presentation.UI.Interface;
using UnityEngine;

namespace MI.Presentation.UI.Popup.Inventory
{
    public class MIInventoryItemViewer : MonoBehaviour, IMIItemViewer
    {
        // Start i
        public event Action<GameObject> OnHideAction;
        public void SetIconDataTable(MIItemIconDataTable dataTable)
        {
        }

        public void UpdateItemViewer(EItemType itemType, int amount)
        {
        }
    }
}
