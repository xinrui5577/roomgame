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
using System.Text;

namespace Org.BouncyCastle.Crypto
{
    public interface ISigner
    {
        /**
         * Return the name of the algorithm the signer implements.
         *
         * @return the name of the algorithm the signer implements.
         */
        string AlgorithmName { get; }

		/**
         * Initialise the signer for signing or verification.
         *
         * @param forSigning true if for signing, false otherwise
         * @param param necessary parameters.
         */
         void Init(bool forSigning, ICipherParameters parameters);

        /**
         * update the internal digest with the byte b
         */
        void Update(byte input);

        /**
         * update the internal digest with the byte array in
         */
        void BlockUpdate(byte[] input, int inOff, int length);

        /**
         * Generate a signature for the message we've been loaded with using
         * the key we were initialised with.
         */
        byte[] GenerateSignature();
        /**
         * return true if the internal state represents the signature described
         * in the passed in array.
         */
        bool VerifySignature(byte[] signature);

        /**
         * reset the internal state
         */
        void Reset();
    }
}

#endif
