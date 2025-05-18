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

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1
{
	public class BerTaggedObjectParser
		: Asn1TaggedObjectParser
	{
		private bool				_constructed;
		private int					_tagNumber;
		private Asn1StreamParser	_parser;

		[Obsolete]
		internal BerTaggedObjectParser(
			int		baseTag,
			int		tagNumber,
			Stream	contentStream)
			: this((baseTag & Asn1Tags.Constructed) != 0, tagNumber, new Asn1StreamParser(contentStream))
		{
		}

		internal BerTaggedObjectParser(
			bool				constructed,
			int					tagNumber,
			Asn1StreamParser	parser)
		{
			_constructed = constructed;
			_tagNumber = tagNumber;
			_parser = parser;
		}

		public bool IsConstructed
		{
			get { return _constructed; }
		}

		public int TagNo
		{
			get { return _tagNumber; }
		}

		public IAsn1Convertible GetObjectParser(
			int		tag,
			bool	isExplicit)
		{
			if (isExplicit)
			{
				if (!_constructed)
					throw new IOException("Explicit tags must be constructed (see X.690 8.14.2)");

				return _parser.ReadObject();
			}

			return _parser.ReadImplicit(_constructed, tag);
		}

		public Asn1Object ToAsn1Object()
		{
			try
			{
				return _parser.ReadTaggedObject(_constructed, _tagNumber);
			}
			catch (IOException e)
			{
				throw new Asn1ParsingException(e.Message);
			}
		}
	}
}

#endif
