using MI.Data.Tile;
using MI.Domain.Tile;
using MI.Untility;
using MI.Utility;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace MI.Data.Config
{
    // 광물 전역 설정 ScriptableObject
    // 밀도별 드랍 수량 범위 + EMineralType×EMineralDensity → Sprite 매핑
    [CreateAssetMenu(fileName = "MineralConfig", menuName = "MI/Config/MineralConfig")]
    public class MIMineralConfig : SerializedScriptableObject
    {
        #region Density Drop Range

        [Title("밀도별 드랍 수량 범위")]
        [InfoBox("각 EMineralDensity 단계에서 실제 드랍될 광물 수량의 min~max를 설정합니다.\n" +
                 "기본값 예시: Low=3~5, Medium=6~10, High=11~15, Star=16~20")]
        [TableList(ShowIndexLabels = true)]
        [SerializeField]
        private List<FMineralDensityRange> _densityRanges = new List<FMineralDensityRange>
        {
            new FMineralDensityRange { Density = EMineralDensity.Low,    DropRange = new MIIntRange(3,  5)  },
            new FMineralDensityRange { Density = EMineralDensity.Medium, DropRange = new MIIntRange(6,  10) },
            new FMineralDensityRange { Density = EMineralDensity.High,   DropRange = new MIIntRange(11, 15) },
            new FMineralDensityRange { Density = EMineralDensity.Star,   DropRange = new MIIntRange(16, 20) },
        };

        #endregion Density Drop Range

        #region Sprite Mapping

        [Title("광물 스프라이트 매핑")]
        [InfoBox("Key1: EMineralType (광물 종류)\nKey2: EMineralDensity (밀도)\nValue: 해당 조합에 사용할 스프라이트")]
        [OdinSerialize]
        [DictionaryDrawerSettings(KeyLabel = "광물 타입", ValueLabel = "밀도별 스프라이트")]
        private Dictionary<EMineralType, Dictionary<EMineralDensity, Sprite>> _mineralSprites
            = new Dictionary<EMineralType, Dictionary<EMineralDensity, Sprite>>();

        #endregion Sprite Mapping

        #region Public API

        public MIIntRange GetDropRange(EMineralDensity density)
        {
            foreach (var entry in _densityRanges)
            {
                if (entry.Density == density)
                    return entry.DropRange;
            }

            MILog.LogWarning($"[MIMineralConfig] 밀도 {density}에 대한 드랍 범위 설정이 없습니다.");
            return null;
        }

        public Sprite GetMineralSprite(EMineralType mineralType, EMineralDensity density)
        {
            if (!_mineralSprites.TryGetValue(mineralType, out var densityMap))
            {
                MILog.LogWarning($"[MIMineralConfig] 광물 타입 {mineralType}에 대한 스프라이트 설정이 없습니다.");
                return null;
            }

            if (!densityMap.TryGetValue(density, out Sprite sprite))
            {
                MILog.LogWarning($"[MIMineralConfig] 광물 {mineralType}, 밀도 {density}에 대한 스프라이트 설정이 없습니다.");
                return null;
            }

            return sprite;
        }

        #endregion Public API
    }
}
