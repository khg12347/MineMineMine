using System;
using System.Collections.Generic;
using MI.Utility;

namespace MI.Domain.UserState.Wallet
{
    /// <summary>
    /// 플레이어 재화 관리.
    /// 골드/다이아 등 재화를 누적하여 보관합니다.
    /// </summary>
    public class MIUserWallet : IMIGoldDropEventListener
    {
        // long 타입으로 관리하여 대량의 재화도 안전하게 처리
        private readonly Dictionary<ECurrencyType, long> _currencies = new();

        // 재화 변경 시 발행 (타입, 변경량, 변경 후 총량)
        public event Action<ECurrencyType, long, long> OnCurrencyUpdated;

        #region Event Listener

        public void Enable() => MIGoldDropEvent.Register(this);
        public void Disable() => MIGoldDropEvent.Unregister(this);

        public void OnGoldDropped(FGoldDropData data)
        {
            Add(ECurrencyType.Gold, data.Amount);
        }

        #endregion Event Listener

        #region Public API

        public long GetAmount(ECurrencyType type)
        {
            return _currencies.TryGetValue(type, out long amount) ? amount : 0;
        }

        public void Add(ECurrencyType type, long amount)
        {
            if (type == ECurrencyType.None || amount <= 0) return;

            if (_currencies.ContainsKey(type))
                _currencies[type] += amount;
            else
                _currencies[type] = amount;

            OnCurrencyUpdated?.Invoke(type, amount, _currencies[type]);
            MILog.Log($"[MIUserWallet] {type} +{amount} (총 {_currencies[type]})");
        }

        public bool Spend(ECurrencyType type, long amount)
        {
            if (type == ECurrencyType.None || amount <= 0) return false;

            long current = GetAmount(type);
            if (current < amount) return false;

            _currencies[type] = current - amount;
            OnCurrencyUpdated?.Invoke(type, -amount, _currencies[type]);
            MILog.Log($"[MIUserWallet] {type} -{amount} (총 {_currencies[type]})");
            return true;
        }

        public bool HasEnough(ECurrencyType type, long amount) => GetAmount(type) >= amount;

        #endregion Public API
    }
}
