using MI.Utility;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace MI.Data.UIRes
{
    [CreateAssetMenu(fileName = "StatusConfig", menuName = "MI/Data/UIRes/Numbers")]
    public class MIUINumberResources : SerializedScriptableObject
    {
        [LabelText("ū ����")]
        [SerializeField] private Sprite[] _bigNums = new Sprite[10];

        [LabelText("�߰� ����")]
        [SerializeField] private Sprite[] middleNums = new Sprite[10];
    
        public Sprite GetBigNum(int num)
        {
            if (num < 0 || num > 9)
            {
                MILog.LogError("Invalid number: " + num);
                return null;
            }
            return _bigNums[num];
        }

        public Sprite GetMiddleNum(int num)
        {
            if (num < 0 || num > 9)
            {
                MILog.LogError("Invalid number: " + num);
                return null;
            }
            return middleNums[num];
        }
    }
}
