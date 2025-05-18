/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

using System;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Generators
{
    class DHKeyGeneratorHelper
    {
        internal static readonly DHKeyGeneratorHelper Instance = new DHKeyGeneratorHelper();

        private DHKeyGeneratorHelper()
        {
        }

        internal BigInteger CalculatePrivate(
            DHParameters	dhParams,
            SecureRandom	random)
        {
            int limit = dhParams.L;

            if (limit != 0)
            {
                int minWeight = limit >> 2;
                for (;;)
                {
                    BigInteger x = new BigInteger(limit, random).SetBit(limit - 1);
                    if (WNafUtilities.GetNafWeight(x) >= minWeight)
                    {
                        return x;
                    }
                }
            }

            BigInteger min = BigInteger.Two;
            int m = dhParams.M;
            if (m != 0)
            {
                min = BigInteger.One.ShiftLeft(m - 1);
            }

            BigInteger q = dhParams.Q;
            if (q == null)
            {
                q = dhParams.P;
            }
            BigInteger max = q.Subtract(BigInteger.Two);

            {
                int minWeight = max.BitLength >> 2;
                for (;;)
                {
                    BigInteger x = BigIntegers.CreateRandomInRange(min, max, random);
                    if (WNafUtilities.GetNafWeight(x) >= minWeight)
                    {
                        return x;
                    }
                }
            }
        }

        internal BigInteger CalculatePublic(
            DHParameters	dhParams,
            BigInteger		x)
        {
            return dhParams.G.ModPow(x, dhParams.P);
        }
    }
}

#endif
