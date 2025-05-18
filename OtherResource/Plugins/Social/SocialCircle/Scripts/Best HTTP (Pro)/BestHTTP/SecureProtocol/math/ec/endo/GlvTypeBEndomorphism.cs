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

namespace Org.BouncyCastle.Math.EC.Endo
{
    public class GlvTypeBEndomorphism
        :   GlvEndomorphism
    {
        protected readonly ECCurve m_curve;
        protected readonly GlvTypeBParameters m_parameters;
        protected readonly ECPointMap m_pointMap;

        public GlvTypeBEndomorphism(ECCurve curve, GlvTypeBParameters parameters)
        {
            this.m_curve = curve;
            this.m_parameters = parameters;
            this.m_pointMap = new ScaleXPointMap(curve.FromBigInteger(parameters.Beta));
        }

        public virtual BigInteger[] DecomposeScalar(BigInteger k)
        {
            int bits = m_parameters.Bits;
            BigInteger b1 = CalculateB(k, m_parameters.G1, bits);
            BigInteger b2 = CalculateB(k, m_parameters.G2, bits);

            BigInteger[] v1 = m_parameters.V1, v2 = m_parameters.V2;
            BigInteger a = k.Subtract((b1.Multiply(v1[0])).Add(b2.Multiply(v2[0])));
            BigInteger b = (b1.Multiply(v1[1])).Add(b2.Multiply(v2[1])).Negate();

            return new BigInteger[]{ a, b };
        }

        public virtual ECPointMap PointMap
        {
            get { return m_pointMap; }
        }

        public virtual bool HasEfficientPointMap
        {
            get { return true; }
        }

        protected virtual BigInteger CalculateB(BigInteger k, BigInteger g, int t)
        {
            bool negative = (g.SignValue < 0);
            BigInteger b = k.Multiply(g.Abs());
            bool extra = b.TestBit(t - 1);
            b = b.ShiftRight(t);
            if (extra)
            {
                b = b.Add(BigInteger.One);
            }
            return negative ? b.Negate() : b;
        }
    }
}

#endif
