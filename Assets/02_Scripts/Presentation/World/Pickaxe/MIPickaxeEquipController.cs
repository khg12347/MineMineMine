using System.Collections.Generic;

using MI.Data.Config;
using MI.Domain.Pickaxe;
using MI.Domain.Pickaxe.Equipment;

using UnityEngine;

namespace MI.Presentation.World.Pickaxe
{
    using Camera = UnityEngine.Camera;

    /// <summary>
    /// 곡괭이 3슬롯(Main/Sub1/Sub2) 인스턴스를 프리팹 기반으로 스폰하고 관리한다.
    /// 장착 변경 이벤트에 따라 Config 교체 + ActiveSelf 토글을 수행한다.
    /// </summary>
    public class MIPickaxeManager : MonoBehaviour
    {
        #region Fields

        private readonly Dictionary<EEquipSlot, MIPickaxeController> _controllers = new();

        private MIPickaxeSpecDataTable _specDataTable;
        private IMIPickaxeInventory _inventory;
        private Camera _mainCamera;
        private GameObject _pickaxePrefab;

        #endregion Fields

        #region Properties

        /// <summary>메인 슬롯 곡괭이 컨트롤러. StageOrchestrator가 참조한다.</summary>
        public MIPickaxeController MainPickaxe =>
            _controllers.TryGetValue(EEquipSlot.Main, out var c) ? c : null;

        #endregion Properties

        #region Public API

        /// <summary>
        /// 매니저를 초기화한다. 프리팹에서 3개 인스턴스를 스폰하고 이벤트를 구독한다.
        /// </summary>
        public void Initialize(
            MIPickaxeSpecDataTable specDataTable,
            IMIPickaxeInventory inventory,
            Camera mainCamera,
            GameObject pickaxePrefab)
        {
            _specDataTable = specDataTable;
            _inventory = inventory;
            _mainCamera = mainCamera;
            _pickaxePrefab = pickaxePrefab;

            SpawnControllers();

            _inventory.OnEquipChanged += HandleEquipChanged;

            SyncAllSlots();
        }

        #endregion Public API

        #region Unity Lifecycle

        private void OnDestroy()
        {
            if (_inventory != null)
                _inventory.OnEquipChanged -= HandleEquipChanged;
        }

        #endregion Unity Lifecycle

        #region Helper

        private void SpawnControllers()
        {
            SpawnOne(EEquipSlot.Main);
            SpawnOne(EEquipSlot.Sub1);
            SpawnOne(EEquipSlot.Sub2);
        }

        private void SpawnOne(EEquipSlot slot)
        {
            var go = Instantiate(_pickaxePrefab, transform);
            go.name = $"Pickaxe_{slot}";
            go.SetActive(false);

            var controller = go.GetComponent<MIPickaxeController>();
            controller.SetCamera(_mainCamera);
            _controllers[slot] = controller;
        }

        /// <summary>현재 장착 상태를 기반으로 3슬롯을 일괄 동기화한다.</summary>
        private void SyncAllSlots()
        {
            ApplySlot(EEquipSlot.Main, _inventory.GetEquipped(EEquipSlot.Main));
            ApplySlot(EEquipSlot.Sub1, _inventory.GetEquipped(EEquipSlot.Sub1));
            ApplySlot(EEquipSlot.Sub2, _inventory.GetEquipped(EEquipSlot.Sub2));
        }

        private void HandleEquipChanged(EEquipSlot slot, EPickaxeType prev, EPickaxeType next)
        {
            ApplySlot(slot, next);
        }

        private void ApplySlot(EEquipSlot slot, EPickaxeType pickaxeType)
        {
            if (!_controllers.TryGetValue(slot, out var controller)) return;

            if (pickaxeType == EPickaxeType.None)
            {
                controller.gameObject.SetActive(false);
                return;
            }

            var config = _specDataTable.GetConfig(pickaxeType);
            if (config == null) return;

            controller.ApplyConfig(config);

            if (!controller.gameObject.activeSelf)
                controller.gameObject.SetActive(true);
        }

        #endregion Helper
    }
}
