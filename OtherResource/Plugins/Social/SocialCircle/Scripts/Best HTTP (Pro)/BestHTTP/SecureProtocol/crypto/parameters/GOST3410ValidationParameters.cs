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

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class Gost3410ValidationParameters
	{
		private int x0;
		private int c;
		private long x0L;
		private long cL;

		public Gost3410ValidationParameters(
			int x0,
			int c)
		{
			this.x0 = x0;
			this.c = c;
		}

		public Gost3410ValidationParameters(
			long x0L,
			long cL)
		{
			this.x0L = x0L;
			this.cL = cL;
		}

		public int C { get { return c; } }
		public int X0 { get { return x0; } }
		public long CL { get { return cL; } }
		public long X0L { get { return x0L; } }

		public override bool Equals(
			object obj)
		{
			Gost3410ValidationParameters other = obj as Gost3410ValidationParameters;

			return other != null
				&& other.c == this.c
				&& other.x0 == this.x0
				&& other.cL == this.cL
				&& other.x0L == this.x0L;
		}

		public override int GetHashCode()
		{
			return c.GetHashCode() ^ x0.GetHashCode() ^ cL.GetHashCode() ^ x0L.GetHashCode();
		}

	}
}

#endif
