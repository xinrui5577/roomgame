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
    public class ElGamalPrivateKeyParameters
		: ElGamalKeyParameters
    {
        private readonly BigInteger x;

		public ElGamalPrivateKeyParameters(
            BigInteger			x,
            ElGamalParameters	parameters)
			: base(true, parameters)
        {
			if (x == null)
				throw new ArgumentNullException("x");

			this.x = x;
        }

		public BigInteger X
        {
            get { return x; }
        }

		public override bool Equals(
			object obj)
		{
			if (obj == this)
				return true;

			ElGamalPrivateKeyParameters other = obj as ElGamalPrivateKeyParameters;

			if (other == null)
				return false;

			return Equals(other);
		}

		protected bool Equals(
			ElGamalPrivateKeyParameters other)
		{
			return other.x.Equals(x) && base.Equals(other);
		}

		public override int GetHashCode()
		{
			return x.GetHashCode() ^ base.GetHashCode();
		}
    }
}

#endif
