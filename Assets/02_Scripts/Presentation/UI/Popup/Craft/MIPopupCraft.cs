using MI.Data.Config;
using MI.Data.UIRes;
using MI.Domain.Pickaxe;
using MI.Presentation.UI.Popup;
using UnityEngine;

namespace MI.Presentation.UI.Popup.Craft
{
    /// <summary>
    /// 곡괭이 제작 팝업. MIPopupBase 상속.
    /// 15개 제작 슬롯 + 상단 장착 슬롯 3개 + 하단 상세 패널.
    /// </summary>
    public class MIPopupCraft : MIPopupBase
    {
        // 15개 제작 슬롯 (0~9: 제작 가능 10종, 10~14: 미해금 ???)
        [SerializeField] private MICraftSlotViewer[] _craftSlots;

        // 상단 장착 슬롯 3개 (Main, Sub1, Sub2)
        [SerializeField] private MIEquipSlotViewer[] _equipSlots;

        [SerializeField] private MICraftDetailPanel _detailPanel;

        private IMIPickaxeCraftService _craftService;
        private IMIPickaxeInventory _pickaxeInventory;
        private IMIPickaxeEquipment _pickaxeEquipment;
        private MIPickaxeCraftConfig _craftConfig;
        private MIPickaxeUIDataTable _pickaxeIconTable;
        private MIItemIconDataTable _itemIconTable;
        private MIUINumberResources _uiNumberResources;

        private EPickaxeType _selectedType = EPickaxeType.None;

        #region Initialization

        public void InjectResources(
            MIUINumberResources uiNumberResources,
            MIPickaxeUIDataTable pickaxeIconDataTable,
            MIItemIconDataTable itemIconDataTable)
        {
            _uiNumberResources = uiNumberResources;
            _pickaxeIconTable = pickaxeIconDataTable;
            _itemIconTable = itemIconDataTable;
        }


        /// <summary>
        /// 의존성 주입. MISceneContext.InitializeSceneContext()에서 호출.
        /// </summary>
        public void Initialize(
            IMIPickaxeCraftService craftService,
            IMIPickaxeInventory pickaxeInventory,
            IMIPickaxeEquipment pickaxeEquipment,
            MIPickaxeCraftConfig craftConfig)
        {
            _craftService = craftService;
            _pickaxeInventory = pickaxeInventory;
            _pickaxeEquipment = pickaxeEquipment;
            _craftConfig = craftConfig;

            InitCraftSlots();
            InitEquipSlots();
            _detailPanel.InitDataTable(_pickaxeIconTable, _itemIconTable, _uiNumberResources);

            _pickaxeInventory.OnPickaxeAdded += HandlePickaxeAdded;
            _pickaxeInventory.OnEquipChanged += HandleEquipChanged;
        }

        private void OnDestroy()
        {
            if (_pickaxeInventory == null) return;

            _pickaxeInventory.OnPickaxeAdded -= HandlePickaxeAdded;
            _pickaxeInventory.OnEquipChanged -= HandleEquipChanged;
        }

        #endregion Initialization

        #region Slot Setup

        private void InitCraftSlots()
        {
            for (int i = 0; i < _craftSlots.Length; i++)
            {
                if (i < 1)
                {
                    // Pickaxe01~10 (index 0은 사용하지 않음, 1부터 시작)
                    var type = (EPickaxeType)(i + 1);
                    bool isOwned = _pickaxeInventory.IsOwned(type);
                    _craftSlots[i].Setup(_pickaxeIconTable, type, isOwned, OnSlotClicked, OnEnhanceClicked);
                }
                else
                {
                    _craftSlots[i].SetupLocked();
                }
            }
        }

        private void InitEquipSlots()
        {
            _equipSlots[0].Setup(_pickaxeIconTable, EEquipSlot.Main, _pickaxeInventory.GetEquipped(EEquipSlot.Main));
            _equipSlots[1].Setup(_pickaxeIconTable, EEquipSlot.Sub1, _pickaxeInventory.GetEquipped(EEquipSlot.Sub1));
            _equipSlots[2].Setup(_pickaxeIconTable, EEquipSlot.Sub2, _pickaxeInventory.GetEquipped(EEquipSlot.Sub2));
        }

        #endregion Slot Setup

        #region Slot Click Callback

        /// <summary>곡괭이 슬롯 클릭 → 하단 상세 패널 갱신</summary>
        private void OnSlotClicked(EPickaxeType type)
        {
            _selectedType = type;
            bool isOwned = _pickaxeInventory.IsOwned(type);

            if (isOwned)
            {
                _detailPanel.ShowEquipMode(type, OnEquipClicked, OnInfoClicked, OnEnhanceClicked);
            }
            else
            {
                var cost = _craftConfig.GetCost(type);
                bool canCraft = _craftService.CanCraft(type);
                _detailPanel.ShowCraftMode(type, cost, canCraft, _craftService, OnCraftClicked);
            }
        }

        #endregion Slot Click Callback

        #region Button Callbacks

        /// <summary>제작 버튼 클릭</summary>
        private void OnCraftClicked()
        {
            if (_selectedType == EPickaxeType.None) return;
            _craftService.TryCraft(_selectedType);
            // UI 갱신은 OnPickaxeAdded 이벤트에서 처리
        }

        /// <summary>
        /// 장착 버튼 클릭 → Main/Sub1/Sub2 선택 UI 표시.
        /// 슬롯 선택 완료 시 OnEquipSlotSelected(slot) 호출.
        /// </summary>
        private void OnEquipClicked()
        {
            // TODO: Main/Sub1/Sub2 선택 UI 표시
            // 선택 완료 시 → OnEquipSlotSelected(slot) 호출
        }

        /// <summary>
        /// 장착 슬롯 선택 완료 시 호출.
        /// UI에서 Main/Sub1/Sub2 중 하나를 선택하면 이 메서드를 호출한다.
        /// </summary>
        public void OnEquipSlotSelected(EEquipSlot slot)
        {
            if (_selectedType == EPickaxeType.None) return;
            _pickaxeEquipment.Equip(_selectedType, slot);
        }

        /// <summary>
        /// 강화 버튼 클릭 → 강화 팝업으로 전환.
        /// 향후 강화 시스템 구현 시 이 메서드에서 강화 팝업을 열거나
        /// IMIPickaxeEnhanceService를 호출한다.
        /// </summary>
        private void OnEnhanceClicked(EPickaxeType type = EPickaxeType.None)
        {
            var targetType = type != EPickaxeType.None ? type : _selectedType;
            if (targetType == EPickaxeType.None) return;

            // TODO: 강화 시스템 — IMIPickaxeEnhanceService 또는 강화 팝업 연동
        }

        /// <summary>정보 버튼 클릭 — 현재 미구현</summary>
        private void OnInfoClicked()
        {
            // TODO: 정보 팝업 — 현재 구현 범위 밖
        }

        #endregion Button Callbacks

        #region Event Handlers

        /// <summary>곡괭이 제작 완료 → 해당 슬롯 + 상세 패널 갱신</summary>
        private void HandlePickaxeAdded(EPickaxeType type)
        {
            int index = (int)type - 1; // EPickaxeType은 1부터 시작
            if (index >= 0 && index < _craftSlots.Length)
            {
                _craftSlots[index].SetOwned(true);
            }

            if (_selectedType == type)
            {
                OnSlotClicked(type); // 상세 패널을 장착 모드로 전환
            }
        }

        /// <summary>장착 슬롯 변경 → 상단 장착 슬롯 UI 갱신</summary>
        private void HandleEquipChanged(EEquipSlot slot, EPickaxeType prev, EPickaxeType next)
        {
            int slotIndex = (int)slot;
            if (slotIndex >= 0 && slotIndex < _equipSlots.Length)
            {
                _equipSlots[slotIndex].Refresh(next);
            }
        }

        #endregion Event Handlers
    }
}
