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

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class MqvPrivateParameters
		: ICipherParameters
	{
		private readonly ECPrivateKeyParameters staticPrivateKey;
		private readonly ECPrivateKeyParameters ephemeralPrivateKey;
		private readonly ECPublicKeyParameters ephemeralPublicKey;
		
		public MqvPrivateParameters(
			ECPrivateKeyParameters	staticPrivateKey,
			ECPrivateKeyParameters	ephemeralPrivateKey)
			: this(staticPrivateKey, ephemeralPrivateKey, null)
		{
		}

		public MqvPrivateParameters(
			ECPrivateKeyParameters	staticPrivateKey,
			ECPrivateKeyParameters	ephemeralPrivateKey,
			ECPublicKeyParameters	ephemeralPublicKey)
		{
			this.staticPrivateKey = staticPrivateKey;
			this.ephemeralPrivateKey = ephemeralPrivateKey;
			this.ephemeralPublicKey = ephemeralPublicKey;
		}

		public ECPrivateKeyParameters StaticPrivateKey
		{
			get { return staticPrivateKey; }
		}

		public ECPrivateKeyParameters EphemeralPrivateKey
		{
			get { return ephemeralPrivateKey; }
		}

		public ECPublicKeyParameters EphemeralPublicKey
		{
			get { return ephemeralPublicKey; }
		}
	}
}

#endif
