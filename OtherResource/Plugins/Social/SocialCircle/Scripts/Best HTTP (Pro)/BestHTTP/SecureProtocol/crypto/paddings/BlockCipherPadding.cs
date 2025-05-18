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
using Org.BouncyCastle.Security;


namespace Org.BouncyCastle.Crypto.Paddings
{
    /**
     * Block cipher padders are expected to conform to this interface
     */
    public interface IBlockCipherPadding
    {
        /**
         * Initialise the padder.
         *
         * @param param parameters, if any required.
         */
        void Init(SecureRandom random);
            //throws ArgumentException;

        /**
         * Return the name of the algorithm the cipher implements.
         *
         * @return the name of the algorithm the cipher implements.
         */
        string PaddingName { get; }

		/**
         * add the pad bytes to the passed in block, returning the
         * number of bytes added.
         */
        int AddPadding(byte[] input, int inOff);

        /**
         * return the number of pad bytes present in the block.
         * @exception InvalidCipherTextException if the padding is badly formed
         * or invalid.
         */
        int PadCount(byte[] input);
        //throws InvalidCipherTextException;
    }

}

#endif
