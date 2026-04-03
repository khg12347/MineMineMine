using System;
using System.Collections.Generic;
using MI.Utility;

namespace MI.Domain.UserState.Wallet
{
    [Serializable]
    public struct FGoldDropData
    {
        public int Amount;
    }

    public interface IMIGoldDropEventListener
    {
        void OnGoldDropped(FGoldDropData data);
    }

    /// <summary>
    /// 골드 드랍 이벤트 브로드캐스터 (정적 옵저버 패턴).
    /// IMIGoldDropEventListener를 등록한 객체에 드랍 데이터를 전파합니다.
    /// </summary>
    public static class MIGoldDropEvent
    {
        private static readonly List<IMIGoldDropEventListener> s_listeners = new();

        #region Public API

        public static void Register(IMIGoldDropEventListener listener)
        {
            if (listener == null || s_listeners.Contains(listener))
                return;

            s_listeners.Add(listener);
        }

        public static void Unregister(IMIGoldDropEventListener listener)
        {
            s_listeners.Remove(listener);
        }

        /// <summary>
        /// 등록된 모든 리스너에게 골드 드랍 데이터를 전파합니다.
        /// 역방향 순회로 콜백 중 Unregister 호출에도 안전합니다.
        /// </summary>
        public static void Broadcast(FGoldDropData data)
        {
            for (int i = s_listeners.Count - 1; i >= 0; i--)
                s_listeners[i].OnGoldDropped(data);

            MILog.Log($"[MIGoldDropEvent] Broadcast: Gold x{data.Amount}");
        }

        #endregion Public API
    }
}
