/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
using System.IO;

using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Asn1
{
	internal class ConstructedOctetStream
		: BaseInputStream
	{
		private readonly Asn1StreamParser _parser;

		private bool _first = true;
		private Stream _currentStream;

		internal ConstructedOctetStream(
			Asn1StreamParser parser)
		{
			_parser = parser;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (_currentStream == null)
			{
				if (!_first)
					return 0;

				Asn1OctetStringParser s = (Asn1OctetStringParser)_parser.ReadObject();

				if (s == null)
					return 0;

				_first = false;
				_currentStream = s.GetOctetStream();
			}

			int totalRead = 0;

			for (;;)
			{
				int numRead = _currentStream.Read(buffer, offset + totalRead, count - totalRead);

				if (numRead > 0)
				{
					totalRead += numRead;

					if (totalRead == count)
						return totalRead;
				}
				else
				{
					Asn1OctetStringParser aos = (Asn1OctetStringParser)_parser.ReadObject();

					if (aos == null)
					{
						_currentStream = null;
						return totalRead;
					}

					_currentStream = aos.GetOctetStream();
				}
			}
		}

		public override int ReadByte()
		{
			if (_currentStream == null)
			{
				if (!_first)
					return 0;

				Asn1OctetStringParser s = (Asn1OctetStringParser)_parser.ReadObject();

				if (s == null)
					return 0;

				_first = false;
				_currentStream = s.GetOctetStream();
			}

			for (;;)
			{
				int b = _currentStream.ReadByte();

				if (b >= 0)
				{
					return b;
				}

				Asn1OctetStringParser aos = (Asn1OctetStringParser)_parser.ReadObject();

				if (aos == null)
				{
					_currentStream = null;
					return -1;
				}

				_currentStream = aos.GetOctetStream();
			}
		}
	}
}

#endif
