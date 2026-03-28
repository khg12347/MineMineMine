using UnityEngine;
using UnityEngine.SceneManagement;

namespace MI.Core.BootStrap
{
    public static class MIBootStrap
    {
        private const string ROOT_SCENE_NAME = "BootStrap";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            SceneManager.LoadScene(ROOT_SCENE_NAME, LoadSceneMode.Additive);
        }
    }
}
