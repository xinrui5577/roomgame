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
    public class DsaPublicKeyParameters
		: DsaKeyParameters
    {
        private readonly BigInteger y;

		public DsaPublicKeyParameters(
            BigInteger		y,
            DsaParameters	parameters)
			: base(false, parameters)
        {
			if (y == null)
				throw new ArgumentNullException("y");

			this.y = y;
        }

		public BigInteger Y
        {
            get { return y; }
        }

		public override bool Equals(object obj)
        {
			if (obj == this)
				return true;

			DsaPublicKeyParameters other = obj as DsaPublicKeyParameters;

			if (other == null)
				return false;

			return Equals(other);
        }

		protected bool Equals(
			DsaPublicKeyParameters other)
		{
			return y.Equals(other.y) && base.Equals(other);
		}

		public override int GetHashCode()
        {
			return y.GetHashCode() ^ base.GetHashCode();
        }
    }
}

#endif
