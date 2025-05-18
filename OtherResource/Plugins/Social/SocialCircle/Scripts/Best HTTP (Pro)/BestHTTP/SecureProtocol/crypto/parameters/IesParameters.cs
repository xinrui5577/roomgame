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

namespace Org.BouncyCastle.Crypto.Parameters
{
    /**
     * parameters for using an integrated cipher in stream mode.
     */
    public class IesParameters : ICipherParameters
    {
        private byte[]  derivation;
        private byte[]  encoding;
        private int     macKeySize;

        /**
         * @param derivation the derivation parameter for the KDF function.
         * @param encoding the encoding parameter for the KDF function.
         * @param macKeySize the size of the MAC key (in bits).
         */
        public IesParameters(
            byte[]  derivation,
            byte[]  encoding,
            int     macKeySize)
        {
            this.derivation = derivation;
            this.encoding = encoding;
            this.macKeySize = macKeySize;
        }

        public byte[] GetDerivationV()
        {
            return derivation;
        }

        public byte[] GetEncodingV()
        {
            return encoding;
        }

        public int MacKeySize
        {
			get
			{
				return macKeySize;
			}
        }
    }

}

#endif
