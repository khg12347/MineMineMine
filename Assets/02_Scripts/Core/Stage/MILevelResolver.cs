using MI.Data.Config;
using UnityEngine;

namespace MI.Core.Stage
{
    /// <summary>
    /// 절대 행 인덱스를 레벨 정보로 변환하는 정적 유틸 클래스.
    /// 마지막 레벨 이후는 마지막 레벨을 반복 적용합니다.
    /// </summary>
    public static class MILevelResolver
    {
        /// <summary>
        /// 지정된 행 번호에 해당하는 레벨과 블렌딩 정보를 반환합니다.
        /// </summary>
        /// <param name="row">절대 행 인덱스</param>
        /// <param name="config">스테이지 설정</param>
        /// <returns>
        /// Primary: 해당 행의 주 레벨<br/>
        /// Secondary: 블렌딩 대상 다음 레벨 (없으면 null)<br/>
        /// BlendT: 블렌딩 비율 [0, 1] — 0이면 블렌딩 없음
        /// </returns>
        public static (MILevelData Primary, MILevelData Secondary, float BlendT)
            Resolve(int row, MIStageConfig config)
        {
            var levels = config?.Levels;
            if (levels == null || levels.Count == 0)
                return (null, null, 0f);

            int accumulated = 0;
            for (int i = 0; i < levels.Count; i++)
            {
                var level   = levels[i];
                int levelEnd = accumulated + level.RowCount;

                if (row < levelEnd)
                {
                    int remaining = levelEnd - row;

                    // 마지막 BlendRows 구간에서 다음 레벨과 블렌딩
                    if (remaining <= level.BlendRows && i + 1 < levels.Count)
                    {
                        float blendT = 1f - (float)remaining / Mathf.Max(level.BlendRows, 1);
                        return (level, levels[i + 1], blendT);
                    }

                    return (level, null, 0f);
                }

                accumulated = levelEnd;
            }

            // 마지막 레벨 반복
            return (levels[levels.Count - 1], null, 0f);
        }
    }
}
