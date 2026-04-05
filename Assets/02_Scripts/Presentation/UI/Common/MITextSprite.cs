using System.Collections.Generic;
using MI.Data.UIRes;
using UnityEngine.UI;

namespace MI.Presentation.UI.Common
{
    /// <summary>
    /// 문자열을 분해하여 Image 리스트에 스프라이트를 할당하는 정적 유틸리티.
    /// 숫자(0~9)는 GetSmallNum, 그 외 문자는 GetCharSprite로 스프라이트를 조회한다.
    /// </summary>
    public static class MITextSprite
    {
        /// <summary>
        /// text를 character 단위로 분해하여 images 각 요소에 스프라이트를 할당한다.
        /// text보다 images가 길면 초과 Image는 비활성화한다.
        /// text가 images보다 길면 초과 문자는 무시한다.
        /// </summary>
        public static void SetTextSprite(string text, IReadOnlyList<Image> images, MIUINumberResources resources)
        {
            if (images == null || resources == null) return;

            int textLen  = text?.Length ?? 0;
            int imageLen = images.Count;

            for (int i = 0; i < imageLen; i++)
            {
                if (images[i] == null) continue;

                if (i < textLen)
                {
                    char c = text[i];
                    images[i].sprite = (c >= '0' && c <= '9')
                        ? resources.GetSmallNum(c - '0')
                        : resources.GetCharSprite(c);
                    images[i].gameObject.SetActive(true);
                }
                else
                {
                    images[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
