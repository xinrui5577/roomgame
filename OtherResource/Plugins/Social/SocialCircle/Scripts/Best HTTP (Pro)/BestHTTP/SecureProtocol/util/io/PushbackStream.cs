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

using Org.BouncyCastle.Asn1.Utilities;

namespace Org.BouncyCastle.Utilities.IO
{
	public class PushbackStream
		: FilterStream
	{
		private int buf = -1;

		public PushbackStream(
			Stream s)
			: base(s)
		{
		}

		public override int ReadByte()
		{
			if (buf != -1)
			{
				int tmp = buf;
				buf = -1;
				return tmp;
			}

			return base.ReadByte();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (buf != -1 && count > 0)
			{
				// TODO Can this case be made more efficient?
				buffer[offset] = (byte) buf;
				buf = -1;
				return 1;
			}

			return base.Read(buffer, offset, count);
		}

		public virtual void Unread(int b)
		{
			if (buf != -1)
				throw new InvalidOperationException("Can only push back one byte");

			buf = b & 0xFF;
		}
	}
}

#endif
