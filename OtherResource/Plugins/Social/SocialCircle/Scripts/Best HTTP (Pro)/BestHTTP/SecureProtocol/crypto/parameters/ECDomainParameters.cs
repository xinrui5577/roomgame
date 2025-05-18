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

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Parameters
{
    public class ECDomainParameters
    {
        internal ECCurve     curve;
        internal byte[]      seed;
        internal ECPoint     g;
        internal BigInteger  n;
        internal BigInteger  h;

        public ECDomainParameters(
            ECCurve     curve,
            ECPoint     g,
            BigInteger  n)
            : this(curve, g, n, BigInteger.One)
        {
        }

        public ECDomainParameters(
            ECCurve     curve,
            ECPoint     g,
            BigInteger  n,
            BigInteger  h)
            : this(curve, g, n, h, null)
        {
        }

        public ECDomainParameters(
            ECCurve     curve,
            ECPoint     g,
            BigInteger  n,
            BigInteger  h,
            byte[]      seed)
        {
            if (curve == null)
                throw new ArgumentNullException("curve");
            if (g == null)
                throw new ArgumentNullException("g");
            if (n == null)
                throw new ArgumentNullException("n");
            if (h == null)
                throw new ArgumentNullException("h");

            this.curve = curve;
            this.g = g.Normalize();
            this.n = n;
            this.h = h;
            this.seed = Arrays.Clone(seed);
        }

        public ECCurve Curve
        {
            get { return curve; }
        }

        public ECPoint G
        {
            get { return g; }
        }

        public BigInteger N
        {
            get { return n; }
        }

        public BigInteger H
        {
            get { return h; }
        }

        public byte[] GetSeed()
        {
            return Arrays.Clone(seed);
        }

        public override bool Equals(
            object obj)
        {
            if (obj == this)
                return true;

            ECDomainParameters other = obj as ECDomainParameters;

            if (other == null)
                return false;

            return Equals(other);
        }

        protected bool Equals(
            ECDomainParameters other)
        {
            return curve.Equals(other.curve)
                &&	g.Equals(other.g)
                &&	n.Equals(other.n)
                &&	h.Equals(other.h)
                &&	Arrays.AreEqual(seed, other.seed);
        }

        public override int GetHashCode()
        {
            return curve.GetHashCode()
                ^	g.GetHashCode()
                ^	n.GetHashCode()
                ^	h.GetHashCode()
                ^	Arrays.GetHashCode(seed);
        }
    }

}

#endif
