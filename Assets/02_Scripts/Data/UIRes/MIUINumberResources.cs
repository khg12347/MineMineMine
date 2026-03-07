using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace MI.Data.UIRes
{
    [CreateAssetMenu(fileName = "StatusConfig", menuName = "MI/Data/UIRes/Numbers")]
    public class MIUINumberResources : SerializedScriptableObject
    {
        [LabelText("큰 숫자")]
        [SerializeField] private Sprite[] _bigNums = new Sprite[10];

        [LabelText("중간 숫자")]
        [SerializeField] private Sprite[] middleNums = new Sprite[10];
    
        public Sprite GetBigNum(int num)
        {
            if (num < 0 || num > 9)
            {
                Debug.LogError("Invalid number: " + num);
                return null;
            }
            return _bigNums[num];
        }

        public Sprite GetMiddleNum(int num)
        {
            if (num < 0 || num > 9)
            {
                Debug.LogError("Invalid number: " + num);
                return null;
            }
            return middleNums[num];
        }
    }
}
