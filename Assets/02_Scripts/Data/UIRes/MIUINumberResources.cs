using MI.Utility;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace MI.Data.UIRes
{
    [CreateAssetMenu(fileName = "StatusConfig", menuName = "MI/Data/UIRes/Numbers")]
    public class MIUINumberResources : SerializedScriptableObject
    {
        [LabelText("큰 크기(깊이UI)")]
        [SerializeField] private Sprite[] _bigNums = new Sprite[10];

        [LabelText("중간 크기(레벨UI)")]
        [SerializeField] private Sprite[] middleNums = new Sprite[10];

        [LabelText("작은 크기")]
        [SerializeField] private Sprite[] _smallNums = new Sprite[10];

        [DictionaryDrawerSettings(KeyLabel = "character", ValueLabel = "Sprite")]
        [SerializeField] private Dictionary<char, Sprite> _charToSpriteMap = new Dictionary<char, Sprite>();

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
        public Sprite GetSmallNum(int num)
        {
            if (num < 0 || num > 9)
            {
                MILog.LogError("Invalid number: " + num);
                return null;
            }
            return _smallNums[num];
        }

        public Sprite GetCharSprite(char c)
        {
            if (_charToSpriteMap.TryGetValue(c, out Sprite sprite))
            {
                return sprite;
            }
            MILog.LogError("Character not found in map: " + c);
            return null;
        }
    }
}
