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

using Org.BouncyCastle.Math.EC.Endo;

namespace Org.BouncyCastle.Math.EC.Multiplier
{
    public class GlvMultiplier
        :   AbstractECMultiplier
    {
        protected readonly ECCurve curve;
        protected readonly GlvEndomorphism glvEndomorphism;

        public GlvMultiplier(ECCurve curve, GlvEndomorphism glvEndomorphism)
        {
            if (curve == null || curve.Order == null)
                throw new ArgumentException("Need curve with known group order", "curve");

            this.curve = curve;
            this.glvEndomorphism = glvEndomorphism;
        }

        protected override ECPoint MultiplyPositive(ECPoint p, BigInteger k)
        {
            if (!curve.Equals(p.Curve))
                throw new InvalidOperationException();

            BigInteger n = p.Curve.Order;
            BigInteger[] ab = glvEndomorphism.DecomposeScalar(k.Mod(n));
            BigInteger a = ab[0], b = ab[1];

            ECPointMap pointMap = glvEndomorphism.PointMap;
            if (glvEndomorphism.HasEfficientPointMap)
            {
                return ECAlgorithms.ImplShamirsTrickWNaf(p, a, pointMap, b);
            }

            return ECAlgorithms.ImplShamirsTrickWNaf(p, a, pointMap.Map(p), b);
        }
    }
}

#endif
