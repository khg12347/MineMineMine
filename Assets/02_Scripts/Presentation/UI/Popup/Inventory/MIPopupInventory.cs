using MI.Data.UIRes;
using MI.Data.User.Inventory;
using MI.Domain.UserState.Inventory;
using System.Collections.Generic;
using UnityEngine;

namespace MI.Presentation.UI.Popup.Inventory
{
    public class MIPopupInventory : MIPopupBase
    {
        private MIUINumberResources _numberResources;
        private MIItemIconDataTable _itemIconDataTable;

        [SerializeField] private List<MIInventoryItemViewer> _itemViewerList;
        private Dictionary<EItemType, MIInventoryItemViewer> _itemViewers = new();
        private MIUserInventory _inventory;
        private bool _isBindingAction = false;
        
        /// <summary> 인벤토리 자리 미리 지정, 한 번 정하면 바뀔 일 없으므로 하드코딩 </summary>
        private Dictionary<EItemType, int> _currentInventoryState = new()
        {
            { EItemType.Soil, 0 },
            { EItemType.Wood, 1 },
            { EItemType.Stone, 2 },
            //{None(미정) , 3}
            //------------------------
            { EItemType.Iron, 4 },
            { EItemType.Copper, 5 },
            { EItemType.Silver, 6 },
            { EItemType.Gold, 7}
            //------------------------
            //{Item, 8}
            //{Item, 9}
            //{Item, 10}
            //{Item, 11}
            //------------------------
            //{Item, 12}
            //{Item, 13}
            //{Item, 14}
            //{Item, 15}
            //------------------------
            //{Item, 16}
            //{Item, 17}
            //{Item, 18}
            //{Item, 19}
            //총 20개 슬롯이 존재하고, 각 슬롯에 아이템 타입은 고정
            //아직 아이템 타입이 정해지지 않은 슬롯은 비워놓음
            };

        #region Unity Events
        private void Awake()
        {
            //Component초기화만 수행
            InitializeInventoryViewer();
        }
        private void OnEnable()
        {
            //Popup이 활성화될 때마다 최신 인벤토리 상태로 UI 업데이트
            if (_inventory != null)
            {
                UpdateInventoryViewer(_inventory.GetAll());
                if (!_isBindingAction)
                {
                    _inventory.OnInventoryUpdated += UpdateInventoryViewer;
                    _isBindingAction = true;
                }
            }
        }
        private void OnDisable()
        {
            _inventory.OnInventoryUpdated -= UpdateInventoryViewer;
            _isBindingAction = false;
        }
        #endregion Unity Events

        // Initializer - 각 슬롯에 미리 지정된 자리에 아이템 뷰어 매핑
        private void InitializeInventoryViewer()
        {
            foreach (var state in _currentInventoryState)
            {
                var itemType = state.Key;
                var slotIndex = state.Value;
                if (slotIndex < _itemViewerList.Count)
                {
                    _itemViewerList[slotIndex].InitializeData(_numberResources, _itemIconDataTable);
                    _itemViewers.Add(itemType, _itemViewerList[slotIndex]);
                }
                else
                {
                    Debug.LogWarning($"[MIPopupInventory] Slot index {slotIndex} for item type {itemType} exceeds the viewer list count.");
                }
            }
        }

        /// <summary>
        /// SceneContext에서 MIUINumberResources, MIItemIconDataTable 인스턴스를 주입받음
        /// </summary>
        /// <param name="numberResources"></param>
        /// <param name="itemIconDataTable"></param>
        public void InjectResources(MIUINumberResources numberResources, MIItemIconDataTable itemIconDataTable)
        {
            _numberResources = numberResources;
            _itemIconDataTable = itemIconDataTable;
        }

        /// <summary>
        /// SceneContext에서 MIUserInventory 인스턴스를 주입받음
        /// </summary>
        /// <param name="inventory"></param>
        public void InjectInventory(MIUserInventory inventory)
        {
            _inventory = inventory;
            _inventory.OnInventoryUpdated += UpdateInventoryViewer;
        }

        /// <summary>
        /// Dictionary로 전달받은 인벤토리 상태를 기반으로 각 아이템 뷰어를 업데이트
        /// </summary>
        /// <param name="inventoryDic"></param>
        private void UpdateInventoryViewer(IReadOnlyDictionary<EItemType, int> inventoryDic)
        {
            foreach (var item in inventoryDic)
            {
                if (_itemViewers.TryGetValue(item.Key, out var viewer))
                {
                    viewer.UpdateItemViewer(item.Key, item.Value);
                }
                else
                {
                    Debug.LogWarning($"[MIPopupInventory] No viewer found for item type {item.Key}.");
                }
            }
        }
    }
}
