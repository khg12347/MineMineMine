using MI.Domain.User;
using UnityEngine;

namespace MI.Core
{
    /// <summary>
    /// 게임 루트 진입점. Non-MB 객체들을 직접 생성하고 관리합니다.
    /// BootStrap 씬에 배치하여 사용합니다.
    /// </summary>
    public class MIGameRoot : MonoBehaviour
    {
        public static MIUserState UserState { get; private set; }

        #region Lifecycle

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            UserState = new MIUserState();
        }

        private void OnDestroy()
        {
            UserState?.Dispose();
            UserState = null;
        }

        #endregion Lifecycle
    }
}
