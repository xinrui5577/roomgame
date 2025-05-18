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

namespace Org.BouncyCastle.Crypto
{
    /**
     * interface that a message digest conforms to.
     */
    public interface IDigest
    {
        /**
         * return the algorithm name
         *
         * @return the algorithm name
         */
        string AlgorithmName { get; }

		/**
         * return the size, in bytes, of the digest produced by this message digest.
         *
         * @return the size, in bytes, of the digest produced by this message digest.
         */
		int GetDigestSize();

		/**
         * return the size, in bytes, of the internal buffer used by this digest.
         *
         * @return the size, in bytes, of the internal buffer used by this digest.
         */
		int GetByteLength();

		/**
         * update the message digest with a single byte.
         *
         * @param inByte the input byte to be entered.
         */
        void Update(byte input);

        /**
         * update the message digest with a block of bytes.
         *
         * @param input the byte array containing the data.
         * @param inOff the offset into the byte array where the data starts.
         * @param len the length of the data.
         */
        void BlockUpdate(byte[] input, int inOff, int length);

        /**
         * Close the digest, producing the final digest value. The doFinal
         * call leaves the digest reset.
         *
         * @param output the array the digest is to be copied into.
         * @param outOff the offset into the out array the digest is to start at.
         */
        int DoFinal(byte[] output, int outOff);

        /**
         * reset the digest back to it's initial state.
         */
        void Reset();
    }
}

#endif
