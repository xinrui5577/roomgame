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

namespace Org.BouncyCastle.Utilities.IO.Pem
{
	public class PemHeader
	{
		private string name;
		private string val;

		public PemHeader(string name, string val)
		{
			this.name = name;
			this.val = val;
		}

		public virtual string Name
		{
			get { return name; }
		}

		public virtual string Value
		{
			get { return val; }
		}

		public override int GetHashCode()
		{
			return GetHashCode(this.name) + 31 * GetHashCode(this.val);
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (!(obj is PemHeader))
				return false;

			PemHeader other = (PemHeader)obj;

			return Platform.Equals(this.name, other.name)
				&& Platform.Equals(this.val, other.val);
		}

		private int GetHashCode(string s)
		{
			if (s == null)
			{
				return 1;
			}

			return s.GetHashCode();
		}
	}
}

#endif
