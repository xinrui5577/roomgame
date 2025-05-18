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

namespace Org.BouncyCastle.Crypto.Parameters
{
    public class ElGamalParameters
		: ICipherParameters
    {
        private readonly BigInteger p, g;
		private readonly int l;

		public ElGamalParameters(
            BigInteger	p,
            BigInteger	g)
			: this(p, g, 0)
        {
        }

		public ElGamalParameters(
			BigInteger	p,
			BigInteger	g,
			int			l)
		{
			if (p == null)
				throw new ArgumentNullException("p");
			if (g == null)
				throw new ArgumentNullException("g");

			this.p = p;
			this.g = g;
			this.l = l;
		}

		public BigInteger P
        {
            get { return p; }
        }

		/**
        * return the generator - g
        */
        public BigInteger G
        {
            get { return g; }
        }

		/**
		 * return private value limit - l
		 */
		public int L
		{
			get { return l; }
		}

		public override bool Equals(
            object obj)
        {
			if (obj == this)
				return true;

			ElGamalParameters other = obj as ElGamalParameters;

			if (other == null)
				return false;

			return Equals(other);
        }

		protected bool Equals(
			ElGamalParameters other)
		{
			return p.Equals(other.p) && g.Equals(other.g) && l == other.l;
		}

		public override int GetHashCode()
        {
            return p.GetHashCode() ^ g.GetHashCode() ^ l;
        }
    }
}

#endif
