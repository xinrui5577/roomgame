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

namespace Org.BouncyCastle.Asn1
{
	/**
	 * A Null object.
	 */
	public class DerNull
		: Asn1Null
	{
		public static readonly DerNull Instance = new DerNull(0);

		byte[] zeroBytes = new byte[0];

		[Obsolete("Use static Instance object")]
		public DerNull()
		{
		}

		protected internal DerNull(int dummy)
		{
		}

		internal override void Encode(
			DerOutputStream  derOut)
		{
			derOut.WriteEncoded(Asn1Tags.Null, zeroBytes);
		}

		protected override bool Asn1Equals(
			Asn1Object asn1Object)
		{
			return asn1Object is DerNull;
		}

		protected override int Asn1GetHashCode()
		{
			return -1;
		}
	}
}

#endif
