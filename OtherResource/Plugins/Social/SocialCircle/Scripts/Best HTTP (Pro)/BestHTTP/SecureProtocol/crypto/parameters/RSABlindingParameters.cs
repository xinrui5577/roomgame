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

using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class RsaBlindingParameters
		: ICipherParameters
	{
		private readonly RsaKeyParameters	publicKey;
		private readonly BigInteger			blindingFactor;

		public RsaBlindingParameters(
			RsaKeyParameters	publicKey,
			BigInteger			blindingFactor)
		{
			if (publicKey.IsPrivate)
				throw new ArgumentException("RSA parameters should be for a public key");

			this.publicKey = publicKey;
			this.blindingFactor = blindingFactor;
		}

		public RsaKeyParameters PublicKey
		{
			get { return publicKey; }
		}

		public BigInteger BlindingFactor
		{
			get { return blindingFactor; }
		}
	}
}

#endif
