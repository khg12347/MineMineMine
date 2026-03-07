using UnityEngine;

namespace MI.Core
{
    public class MIUIRoot : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _hudProvider;

        public IMIHUD HUD => _hudProvider as IMIHUD;
    }
}