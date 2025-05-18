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

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Parameters
{
    public class ECKeyGenerationParameters
		: KeyGenerationParameters
    {
        private readonly ECDomainParameters domainParams;
		private readonly DerObjectIdentifier publicKeyParamSet;

		public ECKeyGenerationParameters(
			ECDomainParameters	domainParameters,
			SecureRandom		random)
			: base(random, domainParameters.N.BitLength)
        {
            this.domainParams = domainParameters;
        }

		public ECKeyGenerationParameters(
			DerObjectIdentifier	publicKeyParamSet,
			SecureRandom		random)
			: this(ECKeyParameters.LookupParameters(publicKeyParamSet), random)
		{
			this.publicKeyParamSet = publicKeyParamSet;
		}

		public ECDomainParameters DomainParameters
        {
			get { return domainParams; }
        }

		public DerObjectIdentifier PublicKeyParamSet
		{
			get { return publicKeyParamSet; }
		}
    }
}

#endif
