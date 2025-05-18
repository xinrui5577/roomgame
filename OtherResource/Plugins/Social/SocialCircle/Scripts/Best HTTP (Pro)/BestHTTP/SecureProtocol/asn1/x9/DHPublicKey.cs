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

namespace Org.BouncyCastle.Asn1.X9
{
	public class DHPublicKey
		: Asn1Encodable
	{
		private readonly DerInteger y;

		public static DHPublicKey GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return GetInstance(DerInteger.GetInstance(obj, isExplicit));
		}

		public static DHPublicKey GetInstance(object obj)
		{
			if (obj == null || obj is DHPublicKey)
				return (DHPublicKey)obj;

			if (obj is DerInteger)
				return new DHPublicKey((DerInteger)obj);

			throw new ArgumentException("Invalid DHPublicKey: " + obj.GetType().FullName, "obj");
		}

		public DHPublicKey(DerInteger y)
		{
			if (y == null)
				throw new ArgumentNullException("y");

			this.y = y;
		}

		public DerInteger Y
		{
			get { return this.y; }
		}

		public override Asn1Object ToAsn1Object()
		{
			return this.y;
		}
	}
}

#endif
