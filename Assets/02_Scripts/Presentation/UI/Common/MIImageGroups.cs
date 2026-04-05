using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MI.Presentation.UI.Common
{
    public class MIImageGroups : MonoBehaviour
    {
        [SerializeField] private List<Image> _imageList = new List<Image>();

        public IReadOnlyList<Image> ImageList => _imageList;
    }
}
