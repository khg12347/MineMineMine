using System;

namespace MI.Domain.Pickaxe.Enhance
{
    /// <summary>
    /// System.Random 기반 기본 랜덤 프로바이더.
    /// 프로덕션 환경에서 사용한다.
    /// </summary>
    public class MIDefaultRandomProvider : IMIRandomProvider
    {
        private readonly Random _random = new();

        /// <inheritdoc/>
        public float NextFloat() => (float)_random.NextDouble();
    }
}
