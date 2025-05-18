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
    class DefiniteLengthInputStream
        : LimitedInputStream
    {
		private static readonly byte[] EmptyBytes = new byte[0];

		private readonly int _originalLength;
		private int _remaining;

        internal DefiniteLengthInputStream(
            Stream	inStream,
            int		length)
            : base(inStream, length)
        {
			if (length < 0)
				throw new ArgumentException("negative lengths not allowed", "length");

			this._originalLength = length;
			this._remaining = length;

			if (length == 0)
			{
				SetParentEofDetect(true);
			}
        }

		internal int Remaining
		{
			get { return _remaining; }
		}

		public override int ReadByte()
        {
			if (_remaining == 0)
				return -1;

			int b = _in.ReadByte();

			if (b < 0)
				throw new EndOfStreamException("DEF length " + _originalLength + " object truncated by " + _remaining);

			if (--_remaining == 0)
			{
				SetParentEofDetect(true);
			}

			return b;
        }

		public override int Read(
			byte[]	buf,
			int		off,
			int		len)
		{
			if (_remaining == 0)
				return 0;

			int toRead = System.Math.Min(len, _remaining);
			int numRead = _in.Read(buf, off, toRead);

			if (numRead < 1)
				throw new EndOfStreamException("DEF length " + _originalLength + " object truncated by " + _remaining);

			if ((_remaining -= numRead) == 0)
			{
				SetParentEofDetect(true);
			}

			return numRead;
		}

        internal void ReadAllIntoByteArray(byte[] buf)
        {
            if (_remaining != buf.Length)
                throw new ArgumentException("buffer length not right for data");

            if ((_remaining -= Streams.ReadFully(_in, buf)) != 0)
                throw new EndOfStreamException("DEF length " + _originalLength + " object truncated by " + _remaining);
            SetParentEofDetect(true);
        }

        internal byte[] ToArray()
		{
			if (_remaining == 0)
				return EmptyBytes;

			byte[] bytes = new byte[_remaining];
			if ((_remaining -= Streams.ReadFully(_in, bytes)) != 0)
				throw new EndOfStreamException("DEF length " + _originalLength + " object truncated by " + _remaining);
			SetParentEofDetect(true);
			return bytes;
		}
    }
}

#endif
