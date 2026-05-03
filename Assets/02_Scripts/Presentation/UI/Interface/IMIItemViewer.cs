using System;
using MI.Data.UIRes;
using MI.Data.User.Inventory;
using UnityEngine;

namespace MI.Presentation.UI.Interface
{
    public interface IMIItemViewer
    {
        event Action<GameObject> OnHideAction;
        void SetIconDataTable(MIItemIconDataTable dataTable);
        void UpdateItemViewer(EItemType itemType, int amount);
    }
}
