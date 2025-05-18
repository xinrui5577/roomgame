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

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class ServerDHParams
    {
        protected readonly DHPublicKeyParameters mPublicKey;

        public ServerDHParams(DHPublicKeyParameters publicKey)
        {
            if (publicKey == null)
                throw new ArgumentNullException("publicKey");

            this.mPublicKey = publicKey;
        }

        public virtual DHPublicKeyParameters PublicKey
        {
            get { return mPublicKey; }
        }

        /**
         * Encode this {@link ServerDHParams} to a {@link Stream}.
         * 
         * @param output
         *            the {@link Stream} to encode to.
         * @throws IOException
         */
        public virtual void Encode(Stream output)
        {
            DHParameters dhParameters = mPublicKey.Parameters;
            BigInteger Ys = mPublicKey.Y;

            TlsDHUtilities.WriteDHParameter(dhParameters.P, output);
            TlsDHUtilities.WriteDHParameter(dhParameters.G, output);
            TlsDHUtilities.WriteDHParameter(Ys, output);
        }

        /**
         * Parse a {@link ServerDHParams} from a {@link Stream}.
         * 
         * @param input
         *            the {@link Stream} to parse from.
         * @return a {@link ServerDHParams} object.
         * @throws IOException
         */
        public static ServerDHParams Parse(Stream input)
        {
            BigInteger p = TlsDHUtilities.ReadDHParameter(input);
            BigInteger g = TlsDHUtilities.ReadDHParameter(input);
            BigInteger Ys = TlsDHUtilities.ReadDHParameter(input);

            return new ServerDHParams(
                TlsDHUtilities.ValidateDHPublicKey(new DHPublicKeyParameters(Ys, new DHParameters(p, g))));
        }
    }
}

#endif
