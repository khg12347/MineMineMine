using System;

using MI.Data.Config;
using MI.Data.Pickaxe;
using MI.Data.Pickaxe.Enhance;
using MI.Domain.Pickaxe.Equipment;
using MI.Utility;
using MI.Domain.UserState.Inventory;
using MI.Domain.UserState.Wallet;
using MI.Data.Pickaxe.Craft;

namespace MI.Domain.Pickaxe.Enhance
{
    /// <summary>
    /// 곡괭이 강화 조건 검증 + 재료/재화 소모 + 확률 판정 + 인스턴스 갱신.
    /// Presentation에서 콜백으로 호출한다.
    /// </summary>
    public class MIPickaxeEnhanceService : IMIPickaxeEnhanceService
    {
        private readonly MIEnhanceCostConfig _enhanceCostConfig;
        private readonly MIEnhanceConfig _enhanceConfig;
        private readonly MIUserInventory _inventory;
        private readonly MIUserWallet _wallet;
        private readonly MIPickaxeInventory _pickaxeInventory;
        private readonly IMIRandomProvider _randomProvider;

        /// <inheritdoc/>
        public event Action<FEnhanceAttemptResult> OnEnhanceAttempted;

        /// <inheritdoc/>
        public int MaxLevel => _enhanceCostConfig.MaxLevel;

        public MIPickaxeEnhanceService(
            IMIPickaxeDataRegistry pickaxeData,
            MIUserInventory inventory,
            MIUserWallet wallet,
            MIPickaxeInventory pickaxeInventory,
            IMIRandomProvider randomProvider)
        {
            _enhanceCostConfig = pickaxeData.EnhanceCostConfig;
            _enhanceConfig     = pickaxeData.EnhanceConfig;
            _inventory         = inventory;
            _wallet            = wallet;
            _pickaxeInventory  = pickaxeInventory;
            _randomProvider    = randomProvider;
        }

        #region IMIPickaxeEnhanceService — Validation

        /// <inheritdoc/>
        public bool CanEnhance(EPickaxeType type)
        {
            if (type == EPickaxeType.None)
            {
                MILog.Log($"[MIPickaxeEnhanceService] CanEnhance false — type이 None");
                return false;
            }

            if (!_pickaxeInventory.IsOwned(type))
            {
                MILog.Log($"[MIPickaxeEnhanceService] CanEnhance false — {type} 미보유");
                return false;
            }

            var instance = _pickaxeInventory.GetInstance(type);
            if (!instance.HasValue)
            {
                MILog.Log($"[MIPickaxeEnhanceService] CanEnhance false — {type} 인스턴스 조회 실패");
                return false;
            }

            if (instance.Value.Level >= MaxLevel)
            {
                MILog.Log($"[MIPickaxeEnhanceService] CanEnhance false — {type} 최대 레벨 도달 (Lv{instance.Value.Level}/{MaxLevel})");
                return false;
            }

            var entry = _enhanceCostConfig.GetEntry(instance.Value.Level);
            if (!entry.HasValue)
            {
                MILog.Log($"[MIPickaxeEnhanceService] CanEnhance false — {type} Lv{instance.Value.Level} 비용 데이터 없음");
                return false;
            }

            var e = entry.Value;

            if (e.Materials != null)
            {
                for (int i = 0; i < e.Materials.Length; i++)
                {
                    if (!HasEnoughMaterial(e.Materials[i]))
                    {
                        int owned = GetMaterialAmount(e.Materials[i]);
                        MILog.Log($"[MIPickaxeEnhanceService] CanEnhance false — {type} 재료 부족: {e.Materials[i].ItemType} ({owned}/{e.Materials[i].Amount})");
                        return false;
                    }
                }
            }

            if (e.Currencies != null)
            {
                for (int i = 0; i < e.Currencies.Length; i++)
                {
                    if (!HasEnoughCurrency(e.Currencies[i]))
                    {
                        MILog.Log($"[MIPickaxeEnhanceService] CanEnhance false — {type} 재화 부족: {e.Currencies[i].CurrencyType} (필요 {e.Currencies[i].Amount})");
                        return false;
                    }
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public bool HasEnoughMaterial(FMaterialCost material)
            => _inventory.HasEnough(material.ItemType, material.Amount);

        /// <inheritdoc/>
        public bool HasEnoughCurrency(FCurrencyCost currency)
            => _wallet.HasEnough(currency.CurrencyType, currency.Amount);

        /// <inheritdoc/>
        public int GetMaterialAmount(FMaterialCost material)
            => _inventory.GetAmount(material.ItemType);

        /// <inheritdoc/>
        public FEnhanceLevelEntry? GetCurrentLevelEntry(EPickaxeType type)
        {
            var instance = _pickaxeInventory.GetInstance(type);
            if (!instance.HasValue) return null;
            return _enhanceCostConfig.GetEntry(instance.Value.Level);
        }

        /// <inheritdoc/>
        public int GetCurrentLevel(EPickaxeType type)
        {
            var instance = _pickaxeInventory.GetInstance(type);
            return instance?.Level ?? 0;
        }

        #endregion IMIPickaxeEnhanceService — Validation

        #region IMIPickaxeEnhanceService — Enhance

        /// <inheritdoc/>
        public FEnhanceAttemptResult TryEnhance(EPickaxeType type)
        {
            // 버그 방지 검증 — UI에서 사전 차단 전제. 도달 시 설계 오류.
            if (!_pickaxeInventory.IsOwned(type))
                throw new InvalidOperationException($"[MIPickaxeEnhanceService] 미보유 곡괭이 강화 시도: {type}");

            var instance = _pickaxeInventory.GetInstance(type);
            if (!instance.HasValue)
                throw new InvalidOperationException($"[MIPickaxeEnhanceService] 인스턴스 조회 실패: {type}");

            int currentLevel = instance.Value.Level;

            if (currentLevel >= MaxLevel)
                throw new InvalidOperationException($"[MIPickaxeEnhanceService] 최대 레벨 초과 강화 시도: {type} Lv{currentLevel}");

            var entry = _enhanceCostConfig.GetEntry(currentLevel);
            if (!entry.HasValue)
                throw new InvalidOperationException($"[MIPickaxeEnhanceService] 강화 비용 데이터 누락: {type} Lv{currentLevel}");

            var e = entry.Value;

            // 재료/재화 충분 여부 검증 — 정상 결과로 반환
            if (e.Materials != null)
            {
                for (int i = 0; i < e.Materials.Length; i++)
                {
                    if (!HasEnoughMaterial(e.Materials[i]))
                        return Emit(new FEnhanceAttemptResult(EEnhanceResult.InsufficientMaterial, type, currentLevel, currentLevel));
                }
            }

            if (e.Currencies != null)
            {
                for (int i = 0; i < e.Currencies.Length; i++)
                {
                    if (!HasEnoughCurrency(e.Currencies[i]))
                        return Emit(new FEnhanceAttemptResult(EEnhanceResult.InsufficientCurrency, type, currentLevel, currentLevel));
                }
            }

            // 재료/재화 소모 (성공/실패 무관)
            if (e.Materials != null)
            {
                for (int i = 0; i < e.Materials.Length; i++)
                    _inventory.TryConsume(e.Materials[i].ItemType, e.Materials[i].Amount);
            }

            if (e.Currencies != null)
            {
                for (int i = 0; i < e.Currencies.Length; i++)
                    _wallet.Spend(e.Currencies[i].CurrencyType, e.Currencies[i].Amount);
            }

            // 확률 판정
            float roll    = _randomProvider.NextFloat();
            bool  success = roll < e.SuccessRate;

            if (!success)
            {
                MILog.Log($"[MIPickaxeEnhanceService] {type} Lv{currentLevel} 강화 실패 (확률 {e.SuccessRate:P0}, 주사위 {roll:F3})");
                return Emit(new FEnhanceAttemptResult(EEnhanceResult.Fail, type, currentLevel, currentLevel));
            }

            // 성공: Level / EnhanceCount 증가 후 ResolveStats 재계산
            var inst = instance.Value;
            inst.Level        += 1;
            inst.EnhanceCount += 1;
            inst.ResolveStats(_enhanceConfig);

            _pickaxeInventory.UpdateInstance(inst);

            // 대성공 추가 판정
            float perfectRoll = _randomProvider.NextFloat();
            bool  isPerfect   = perfectRoll < e.PerfectSuccessRate;

            if (isPerfect)
            {
                MILog.Log($"[MIPickaxeEnhanceService] {type} Lv{currentLevel}→{inst.Level} 강화 대성공 (대성공확률 {e.PerfectSuccessRate:P0}, 주사위 {perfectRoll:F3})");
                return Emit(new FEnhanceAttemptResult(EEnhanceResult.PerfectlySuccess, type, currentLevel, inst.Level));
            }

            MILog.Log($"[MIPickaxeEnhanceService] {type} Lv{currentLevel}→{inst.Level} 강화 성공");
            return Emit(new FEnhanceAttemptResult(EEnhanceResult.Success, type, currentLevel, inst.Level));
        }

        /// <inheritdoc/>
        public FEnhanceAttemptResult TryEnhanceFree(EPickaxeType type)
        {
            var instance = _pickaxeInventory.GetInstance(type);
            if (!instance.HasValue)
                throw new InvalidOperationException($"[MIPickaxeEnhanceService] TryEnhanceFree — 인스턴스 조회 실패: {type}");

            int currentLevel = instance.Value.Level;

            // MaxLevel 도달 시 예외 없이 정상 반환
            if (currentLevel >= MaxLevel)
            {
                MILog.Log($"[MIPickaxeEnhanceService] TryEnhanceFree — {type} 이미 최대 레벨 (Lv{currentLevel}), 재도전 중단");
                return Emit(new FEnhanceAttemptResult(EEnhanceResult.MaxLevel, type, currentLevel, currentLevel));
            }

            var entry = _enhanceCostConfig.GetEntry(currentLevel);
            if (!entry.HasValue)
                throw new InvalidOperationException($"[MIPickaxeEnhanceService] TryEnhanceFree — 비용 데이터 누락: {type} Lv{currentLevel}");

            var e = entry.Value;

            // 대성공 보너스 재도전 — 무조건 레벨업 (실패 없음)
            // 레벨 증가
            var inst = instance.Value;
            inst.Level        += 1;
            inst.EnhanceCount += 1;
            inst.ResolveStats(_enhanceConfig);

            _pickaxeInventory.UpdateInstance(inst);

            // 대성공 추가 판정
            float perfectRoll = _randomProvider.NextFloat();
            bool  isPerfect   = perfectRoll < e.PerfectSuccessRate;

            if (isPerfect)
            {
                MILog.Log($"[MIPickaxeEnhanceService] TryEnhanceFree — {type} Lv{currentLevel}→{inst.Level} 대성공 (주사위 {perfectRoll:F3})");
                return Emit(new FEnhanceAttemptResult(EEnhanceResult.PerfectlySuccess, type, currentLevel, inst.Level));
            }

            MILog.Log($"[MIPickaxeEnhanceService] TryEnhanceFree — {type} Lv{currentLevel}→{inst.Level} 성공");
            return Emit(new FEnhanceAttemptResult(EEnhanceResult.Success, type, currentLevel, inst.Level));
        }

        #endregion IMIPickaxeEnhanceService — Enhance

        #region Helper

        private FEnhanceAttemptResult Emit(FEnhanceAttemptResult result)
        {
            OnEnhanceAttempted?.Invoke(result);
            return result;
        }

        #endregion Helper
    }
}
