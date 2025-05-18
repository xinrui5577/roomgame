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
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto
{
    /**
     * interface for classes implementing the Digital Signature Algorithm
     */
    public interface IDsa
    {
		string AlgorithmName { get; }

		/**
         * initialise the signer for signature generation or signature
         * verification.
         *
         * @param forSigning true if we are generating a signature, false
         * otherwise.
         * @param param key parameters for signature generation.
         */
        void Init(bool forSigning, ICipherParameters parameters);

        /**
         * sign the passed in message (usually the output of a hash function).
         *
         * @param message the message to be signed.
         * @return two big integers representing the r and s values respectively.
         */
        BigInteger[] GenerateSignature(byte[] message);

        /**
         * verify the message message against the signature values r and s.
         *
         * @param message the message that was supposed to have been signed.
         * @param r the r signature value.
         * @param s the s signature value.
         */
        bool VerifySignature(byte[] message, BigInteger  r, BigInteger s);
    }
}

#endif
