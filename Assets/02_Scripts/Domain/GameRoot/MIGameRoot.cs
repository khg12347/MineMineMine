using MI.Core.ServiceLocator;
using MI.Domain.User;
using UnityEngine;

namespace MI.Domain.GameRoot
{
    /// <summary>
    /// 게임 루트 진입점. Non-MB 객체들을 직접 생성하고 관리합니다.
    /// BootStrap 씬에 배치하여 사용합니다.
    /// </summary>
    public class MIGameRoot : MonoBehaviour
    {
        private MIUserState _userState;

        #region Lifecycle

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _userState = new MIUserState();

            MIServiceLocator.Register(_userState);

            MISceneContext.Current.InitializeSceneContext();
        }

        private void OnDestroy()
        {
            MIServiceLocator.Unregister<MIUserState>();
            _userState?.Dispose();
            _userState = null;
        }

        #endregion Lifecycle
    }
}
