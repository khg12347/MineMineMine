using MI.Domain.Tile;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MI.Data.Config
{
    [CreateAssetMenu(fileName = "MineralConfig", menuName = "MI/Config/MineralConfig")]
    public class MIMineralConfig : SerializedScriptableObject
    {
        [Title("±§π∞ º≥¡§")]
        [LabelText("±§π∞ ≈∏¿‘")]
        [SerializeField] private EMineralType _mineralType;
        
    }
}
