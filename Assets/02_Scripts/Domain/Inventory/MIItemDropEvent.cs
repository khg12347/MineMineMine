using MI.Utility;
using System;
using System.Collections.Generic;

namespace MI.Domain.Inventory
{
    [Serializable]
    public struct FDropItemData
    {
        public EItemType ItemType;
        public int Amount;
    }

    public interface IMIItemDropEventListener
    {
        void OnItemDropped(FDropItemData data);
    }

    /// <summary>
    /// 아이템 드롭 이벤트 브로드캐스터 (정적 옵저버 패턴).
    /// IMIItemDropEventListener를 등록한 객체에 드롭 데이터를 전파합니다.
    /// </summary>
    public static class MIItemDropEvent
    {
        private static readonly List<IMIItemDropEventListener> _listeners = new();

        #region Public API

        public static void Register(IMIItemDropEventListener listener)
        {
            if (listener == null || _listeners.Contains(listener))
                return;

            _listeners.Add(listener);
        }

        public static void Unregister(IMIItemDropEventListener listener)
        {
            _listeners.Remove(listener);
        }

        /// <summary>
        /// 등록된 모든 리스너에게 드롭 데이터를 전파합니다.
        /// 역방향 순회로 콜백 중 Unregister 호출에도 안전합니다.
        /// </summary>
        public static void Broadcast(FDropItemData data)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
                _listeners[i].OnItemDropped(data);

            MILog.Log($"[MIItemDropEvent] Broadcast: {data.ItemType.ToString()} x{data.Amount}");
        }

        #endregion Public API
    }
}
