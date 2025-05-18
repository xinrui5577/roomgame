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

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Parameters
{
    public abstract class DsaKeyParameters
		: AsymmetricKeyParameter
    {
		private readonly DsaParameters parameters;

		protected DsaKeyParameters(
            bool			isPrivate,
            DsaParameters	parameters)
			: base(isPrivate)
        {
			// Note: parameters may be null
            this.parameters = parameters;
        }

		public DsaParameters Parameters
        {
            get { return parameters; }
        }

		public override bool Equals(
			object obj)
		{
			if (obj == this)
				return true;

			DsaKeyParameters other = obj as DsaKeyParameters;

			if (other == null)
				return false;

			return Equals(other);
		}

		protected bool Equals(
			DsaKeyParameters other)
		{
			return Platform.Equals(parameters, other.parameters)
				&& base.Equals(other);
		}

		public override int GetHashCode()
		{
			int hc = base.GetHashCode();

			if (parameters != null)
			{
				hc ^= parameters.GetHashCode();
			}

			return hc;
		}
    }
}

#endif
