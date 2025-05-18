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
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Agreement
{
    /**
     * a Diffie-Hellman key agreement class.
     * <p>
     * note: This is only the basic algorithm, it doesn't take advantage of
     * long term public keys if they are available. See the DHAgreement class
     * for a "better" implementation.</p>
     */
    public class DHBasicAgreement
        : IBasicAgreement
    {
        private DHPrivateKeyParameters	key;
        private DHParameters			dhParams;

        public virtual void Init(
            ICipherParameters parameters)
        {
            if (parameters is ParametersWithRandom)
            {
                parameters = ((ParametersWithRandom) parameters).Parameters;
            }

            if (!(parameters is DHPrivateKeyParameters))
            {
                throw new ArgumentException("DHEngine expects DHPrivateKeyParameters");
            }

            this.key = (DHPrivateKeyParameters) parameters;
            this.dhParams = key.Parameters;
        }

        public virtual int GetFieldSize()
        {
            return (key.Parameters.P.BitLength + 7) / 8;
        }

        /**
         * given a short term public key from a given party calculate the next
         * message in the agreement sequence.
         */
        public virtual BigInteger CalculateAgreement(
            ICipherParameters pubKey)
        {
            if (this.key == null)
                throw new InvalidOperationException("Agreement algorithm not initialised");

            DHPublicKeyParameters pub = (DHPublicKeyParameters)pubKey;

            if (!pub.Parameters.Equals(dhParams))
            {
                throw new ArgumentException("Diffie-Hellman public key has wrong parameters.");
            }

            return pub.Y.ModPow(key.X, dhParams.P);
        }
    }
}

#endif
