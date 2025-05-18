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

namespace Org.BouncyCastle.Math.EC.Multiplier
{
    public class FixedPointCombMultiplier
        : AbstractECMultiplier
    {
        protected override ECPoint MultiplyPositive(ECPoint p, BigInteger k)
        {
            ECCurve c = p.Curve;
            int size = FixedPointUtilities.GetCombSize(c);

            if (k.BitLength > size)
            {
                /*
                 * TODO The comb works best when the scalars are less than the (possibly unknown) order.
                 * Still, if we want to handle larger scalars, we could allow customization of the comb
                 * size, or alternatively we could deal with the 'extra' bits either by running the comb
                 * multiple times as necessary, or by using an alternative multiplier as prelude.
                 */
                throw new InvalidOperationException("fixed-point comb doesn't support scalars larger than the curve order");
            }

            int minWidth = GetWidthForCombSize(size);

            FixedPointPreCompInfo info = FixedPointUtilities.Precompute(p, minWidth);
            ECPoint[] lookupTable = info.PreComp;
            int width = info.Width;

            int d = (size + width - 1) / width;

            ECPoint R = c.Infinity;

            int top = d * width - 1;
            for (int i = 0; i < d; ++i)
            {
                int index = 0;

                for (int j = top - i; j >= 0; j -= d)
                {
                    index <<= 1;
                    if (k.TestBit(j))
                    {
                        index |= 1;
                    }
                }

                R = R.TwicePlus(lookupTable[index]);
            }

            return R;
        }

        protected virtual int GetWidthForCombSize(int combSize)
        {
            return combSize > 257 ? 6 : 5;
        }
    }
}

#endif
