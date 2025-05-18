/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
namespace Org.BouncyCastle.Asn1.Iana
{
	public abstract class IanaObjectIdentifiers
	{
		// id-SHA1 OBJECT IDENTIFIER ::=
		// {iso(1) identified-organization(3) dod(6) internet(1) security(5) mechanisms(5) ipsec(8) isakmpOakley(1)}
		//

		public static readonly DerObjectIdentifier IsakmpOakley = new DerObjectIdentifier("1.3.6.1.5.5.8.1");

		public static readonly DerObjectIdentifier HmacMD5 = new DerObjectIdentifier(IsakmpOakley + ".1");
		public static readonly DerObjectIdentifier HmacSha1 = new DerObjectIdentifier(IsakmpOakley + ".2");

		public static readonly DerObjectIdentifier HmacTiger = new DerObjectIdentifier(IsakmpOakley + ".3");

		public static readonly DerObjectIdentifier HmacRipeMD160 = new DerObjectIdentifier(IsakmpOakley + ".4");
	}
}

#endif
