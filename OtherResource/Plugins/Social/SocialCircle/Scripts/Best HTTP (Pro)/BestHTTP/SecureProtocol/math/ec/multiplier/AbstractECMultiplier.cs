/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

namespace Org.BouncyCastle.Math.EC.Multiplier
{
    public abstract class AbstractECMultiplier
        : ECMultiplier
    {
        public virtual ECPoint Multiply(ECPoint p, BigInteger k)
        {
            int sign = k.SignValue;
            if (sign == 0 || p.IsInfinity)
                return p.Curve.Infinity;

            ECPoint positive = MultiplyPositive(p, k.Abs());
            ECPoint result = sign > 0 ? positive : positive.Negate();

            /*
             * Although the various multipliers ought not to produce invalid output under normal
             * circumstances, a final check here is advised to guard against fault attacks.
             */
            return ECAlgorithms.ValidatePoint(result);
        }

        protected abstract ECPoint MultiplyPositive(ECPoint p, BigInteger k);
    }
}

#endif
