using System;
using MI.Data.Config;
using MI.Domain.Pickaxe;
using MI.Domain.Pickaxe.Equipment;
using MI.Domain.UserState.Inventory;
using MI.Domain.UserState.Wallet;
using MI.Utility;

namespace MI.Domain.Pickaxe.Craft
{
    /// <summary>
    /// 곡괭이 제작 조건 검증 + 재료/재화 소모 + 보유 등록.
    /// Presentation에서 콜백으로 호출한다.
    /// </summary>
    public class MIPickaxeCraftService : IMIPickaxeCraftService
    {
        private readonly MIPickaxeCraftConfig _craftConfig;
        private readonly MIUserInventory _inventory;
        private readonly MIUserWallet _wallet;
        private readonly MIPickaxeInventory _pickaxeInventory;

        /// <inheritdoc/>
        public event Action<EPickaxeType> OnPickaxeCrafted;

        public MIPickaxeCraftService(
            IMIPickaxeDataRegistry pickaxeData,
            MIUserInventory inventory,
            MIUserWallet wallet,
            MIPickaxeInventory pickaxeInventory)
        {
            _craftConfig = pickaxeData.CraftConfig;
            _inventory = inventory;
            _wallet = wallet;
            _pickaxeInventory = pickaxeInventory;
        }

        #region IMIPickaxeCraftService Implementation — Validation

        /// <inheritdoc/>
        public bool CanCraft(EPickaxeType type)
        {
            if (type == EPickaxeType.None) return false;

            // 이미 보유
            if (_pickaxeInventory.IsOwned(type)) return false;

            var costData = _craftConfig.GetCost(type);
            if (!costData.HasValue) return false;

            var cost = costData.Value;

            // 재료 검증
            if (cost.Materials != null)
            {
                for (int i = 0; i < cost.Materials.Length; i++)
                {
                    if (!HasEnoughMaterial(cost.Materials[i])) return false;
                }
            }

            // 재화 검증
            if (cost.Currencies != null)
            {
                for (int i = 0; i < cost.Currencies.Length; i++)
                {
                    if (!HasEnoughCurrency(cost.Currencies[i])) return false;
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public bool HasEnoughMaterial(FMaterialCost material)
            => _inventory.HasEnough(material.ItemType, material.Amount);
        public int GetMaterialAmount(FMaterialCost material)
            => _inventory.GetAmount(material.ItemType);

        /// <inheritdoc/>
        public bool HasEnoughCurrency(FCurrencyCost currency)
            => _wallet.HasEnough(currency.CurrencyType, currency.Amount);


        #endregion IMIPickaxeCraftService Implementation — Validation

        #region IMIPickaxeCraftService Implementation — Craft

        /// <inheritdoc/>
        public bool TryCraft(EPickaxeType type)
        {
            if (!CanCraft(type)) return false;

            var cost = _craftConfig.GetCost(type).Value;

            // 재료 소모
            if (cost.Materials != null)
            {
                for (int i = 0; i < cost.Materials.Length; i++)
                {
                    var mat = cost.Materials[i];

                    //CanCraft에서 이미 검증했으므로 TryConsume은 실패하지 않음이 보장됨
                    _inventory.TryConsume(mat.ItemType, mat.Amount);
                }
            }

            // 재화 소모
            if (cost.Currencies != null)
            {
                for (int i = 0; i < cost.Currencies.Length; i++)
                {
                    var cur = cost.Currencies[i];
                    _wallet.Spend(cur.CurrencyType, cur.Amount);
                }
            }

            // 기본 스탯은 빈 값으로 생성 (타입별 MIPickaxeConfig SO와의 연결은 추후 확장)
            var instance = FPickaxeInstance.Create(type, default);
            _pickaxeInventory.AddPickaxe(instance);

            MILog.Log($"[MIPickaxeCraftService] {type} 제작 완료");
            OnPickaxeCrafted?.Invoke(type);

            return true;
        }

        #endregion IMIPickaxeCraftService Implementation — Craft
    }
}
