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

namespace Org.BouncyCastle.Crypto.Digests
{
	public class NullDigest : IDigest
	{
		private readonly MemoryStream bOut = new MemoryStream();

		public string AlgorithmName
		{
			get { return "NULL"; }
		}

		public int GetByteLength()
		{
			// TODO Is this okay?
			return 0;
		}

		public int GetDigestSize()
		{
			return (int) bOut.Length;
		}

		public void Update(byte b)
		{
			bOut.WriteByte(b);
		}

		public void BlockUpdate(byte[] inBytes, int inOff, int len)
		{
			bOut.Write(inBytes, inOff, len);
		}

		public int DoFinal(byte[] outBytes, int outOff)
		{
			byte[] res = bOut.ToArray();
			res.CopyTo(outBytes, outOff);
			Reset();
			return res.Length;
		}

		public void Reset()
		{
			bOut.SetLength(0);
		}
	}
}

#endif
