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

namespace Org.BouncyCastle.Crypto.Engines
{
	public class VmpcKsa3Engine
		: VmpcEngine
	{
		public override string AlgorithmName
		{
			get { return "VMPC-KSA3"; }
		}

		protected override void InitKey(
			byte[]	keyBytes,
			byte[]	ivBytes)
		{
			s = 0;
			P = new byte[256];
			for (int i = 0; i < 256; i++)
			{
				P[i] = (byte) i;
			}

			for (int m = 0; m < 768; m++)
			{
				s = P[(s + P[m & 0xff] + keyBytes[m % keyBytes.Length]) & 0xff];
				byte temp = P[m & 0xff];
				P[m & 0xff] = P[s & 0xff];
				P[s & 0xff] = temp;
			}

			for (int m = 0; m < 768; m++)
			{
				s = P[(s + P[m & 0xff] + ivBytes[m % ivBytes.Length]) & 0xff];
				byte temp = P[m & 0xff];
				P[m & 0xff] = P[s & 0xff];
				P[s & 0xff] = temp;
			}

			for (int m = 0; m < 768; m++)
			{
				s = P[(s + P[m & 0xff] + keyBytes[m % keyBytes.Length]) & 0xff];
				byte temp = P[m & 0xff];
				P[m & 0xff] = P[s & 0xff];
				P[s & 0xff] = temp;
			}

			n = 0;
		}
	}
}

#endif
