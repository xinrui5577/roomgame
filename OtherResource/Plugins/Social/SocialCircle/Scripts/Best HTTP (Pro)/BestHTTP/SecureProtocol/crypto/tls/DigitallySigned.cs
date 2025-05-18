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

namespace Org.BouncyCastle.Crypto.Tls
{
    public class DigitallySigned
    {
        protected readonly SignatureAndHashAlgorithm mAlgorithm;
        protected readonly byte[] mSignature;

        public DigitallySigned(SignatureAndHashAlgorithm algorithm, byte[] signature)
        {
            if (signature == null)
                throw new ArgumentNullException("signature");

            this.mAlgorithm = algorithm;
            this.mSignature = signature;
        }

        /**
         * @return a {@link SignatureAndHashAlgorithm} (or null before TLS 1.2).
         */
        public virtual SignatureAndHashAlgorithm Algorithm
        {
            get { return mAlgorithm; }
        }

        public virtual byte[] Signature
        {
            get { return mSignature; }
        }

        /**
         * Encode this {@link DigitallySigned} to a {@link Stream}.
         * 
         * @param output
         *            the {@link Stream} to encode to.
         * @throws IOException
         */
        public virtual void Encode(Stream output)
        {
            if (mAlgorithm != null)
            {
                mAlgorithm.Encode(output);
            }
            TlsUtilities.WriteOpaque16(mSignature, output);
        }

        /**
         * Parse a {@link DigitallySigned} from a {@link Stream}.
         * 
         * @param context
         *            the {@link TlsContext} of the current connection.
         * @param input
         *            the {@link Stream} to parse from.
         * @return a {@link DigitallySigned} object.
         * @throws IOException
         */
        public static DigitallySigned Parse(TlsContext context, Stream input)
        {
            SignatureAndHashAlgorithm algorithm = null;
            if (TlsUtilities.IsTlsV12(context))
            {
                algorithm = SignatureAndHashAlgorithm.Parse(input);
            }
            byte[] signature = TlsUtilities.ReadOpaque16(input);
            return new DigitallySigned(algorithm, signature);
        }
    }
}

#endif
