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
using System.Diagnostics;
using System.IO;

namespace Org.BouncyCastle.Utilities.IO
{
	public class TeeInputStream
		: BaseInputStream
	{
		private readonly Stream input, tee;

		public TeeInputStream(Stream input, Stream tee)
		{
			Debug.Assert(input.CanRead);
			Debug.Assert(tee.CanWrite);

			this.input = input;
			this.tee = tee;
		}

        protected override void Dispose(bool isDisposing)
        {
            try
            {
                input.Dispose();
                tee.Dispose();
            }
            finally
            {
                base.Dispose(isDisposing);
            }
        }

		public override int Read(byte[] buf, int off, int len)
		{
			int i = input.Read(buf, off, len);

			if (i > 0)
			{
				tee.Write(buf, off, i);
			}

			return i;
		}

		public override int ReadByte()
		{
			int i = input.ReadByte();

			if (i >= 0)
			{
				tee.WriteByte((byte)i);
			}

			return i;
		}
	}
}

#endif
