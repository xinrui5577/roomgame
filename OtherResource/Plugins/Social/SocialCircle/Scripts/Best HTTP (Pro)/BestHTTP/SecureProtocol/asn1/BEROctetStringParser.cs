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
using System.IO;

using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Asn1
{
	public class BerOctetStringParser
		: Asn1OctetStringParser
	{
		private readonly Asn1StreamParser _parser;

		internal BerOctetStringParser(
			Asn1StreamParser parser)
		{
			_parser = parser;
		}

		public Stream GetOctetStream()
		{
			return new ConstructedOctetStream(_parser);
		}

		public Asn1Object ToAsn1Object()
		{
			try
			{
				return new BerOctetString(Streams.ReadAll(GetOctetStream()));
			}
			catch (IOException e)
			{
				throw new Asn1ParsingException("IOException converting stream to byte array: " + e.Message, e);
			}
		}
	}
}

#endif
