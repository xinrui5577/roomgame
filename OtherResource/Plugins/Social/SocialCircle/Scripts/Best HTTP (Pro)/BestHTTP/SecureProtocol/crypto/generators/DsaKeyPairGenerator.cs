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

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Generators
{
    /**
     * a DSA key pair generator.
     *
     * This Generates DSA keys in line with the method described
     * in <i>FIPS 186-3 B.1 FFC Key Pair Generation</i>.
     */
    public class DsaKeyPairGenerator
        : IAsymmetricCipherKeyPairGenerator
    {
        private static readonly BigInteger One = BigInteger.One;

        private DsaKeyGenerationParameters param;

        public void Init(
            KeyGenerationParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            // Note: If we start accepting instances of KeyGenerationParameters,
            // must apply constraint checking on strength (see DsaParametersGenerator.Init)

            this.param = (DsaKeyGenerationParameters) parameters;
        }

        public AsymmetricCipherKeyPair GenerateKeyPair()
        {
            DsaParameters dsaParams = param.Parameters;

            BigInteger x = GeneratePrivateKey(dsaParams.Q, param.Random);
            BigInteger y = CalculatePublicKey(dsaParams.P, dsaParams.G, x);

            return new AsymmetricCipherKeyPair(
                new DsaPublicKeyParameters(y, dsaParams),
                new DsaPrivateKeyParameters(x, dsaParams));
        }

        private static BigInteger GeneratePrivateKey(BigInteger q, SecureRandom random)
        {
            // B.1.2 Key Pair Generation by Testing Candidates
            int minWeight = q.BitLength >> 2;
            for (;;)
            {
                // TODO Prefer this method? (change test cases that used fixed random)
                // B.1.1 Key Pair Generation Using Extra Random Bits
                //BigInteger x = new BigInteger(q.BitLength + 64, random).Mod(q.Subtract(One)).Add(One);

                BigInteger x = BigIntegers.CreateRandomInRange(One, q.Subtract(One), random);
                if (WNafUtilities.GetNafWeight(x) >= minWeight)
                {
                    return x;
                }
            }
        }

        private static BigInteger CalculatePublicKey(BigInteger p, BigInteger g, BigInteger x)
        {
            return g.ModPow(x, p);
        }
    }
}

#endif
