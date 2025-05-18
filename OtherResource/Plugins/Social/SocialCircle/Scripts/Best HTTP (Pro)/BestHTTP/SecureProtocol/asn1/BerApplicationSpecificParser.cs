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
	public class BerApplicationSpecificParser
		: IAsn1ApplicationSpecificParser
	{
		private readonly int tag;
		private readonly Asn1StreamParser parser;

		internal BerApplicationSpecificParser(
			int					tag,
			Asn1StreamParser	parser)
		{
			this.tag = tag;
			this.parser = parser;
		}

		public IAsn1Convertible ReadObject()
		{
			return parser.ReadObject();
		}

		public Asn1Object ToAsn1Object()
		{
			return new BerApplicationSpecific(tag, parser.ReadVector());
		}
	}
}

#endif
