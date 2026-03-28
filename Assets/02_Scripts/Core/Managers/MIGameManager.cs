using MI.Utility;

namespace MI.Core
{
    public sealed class MIGameManager : MISingleton<MIGameManager>
    {
        private void OnApplicationQuit()
        {
            MIAppLifeTime.OnApplicationQuit();
        }
    }
}
