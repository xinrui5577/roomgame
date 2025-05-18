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

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto.Generators
{
    /**
     * a ElGamal key pair generator.
     * <p>
     * This Generates keys consistent for use with ElGamal as described in
     * page 164 of "Handbook of Applied Cryptography".</p>
     */
    public class ElGamalKeyPairGenerator
		: IAsymmetricCipherKeyPairGenerator
    {
        private ElGamalKeyGenerationParameters param;

        public void Init(
			KeyGenerationParameters parameters)
        {
            this.param = (ElGamalKeyGenerationParameters) parameters;
        }

        public AsymmetricCipherKeyPair GenerateKeyPair()
        {
			DHKeyGeneratorHelper helper = DHKeyGeneratorHelper.Instance;
			ElGamalParameters egp = param.Parameters;
			DHParameters dhp = new DHParameters(egp.P, egp.G, null, 0, egp.L);

			BigInteger x = helper.CalculatePrivate(dhp, param.Random);
			BigInteger y = helper.CalculatePublic(dhp, x);

			return new AsymmetricCipherKeyPair(
                new ElGamalPublicKeyParameters(y, egp),
                new ElGamalPrivateKeyParameters(x, egp));
        }
    }

}

#endif
