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

namespace Org.BouncyCastle.Crypto.Tls
{
    public interface TlsSigner
    {
        void Init(TlsContext context);

        byte[] GenerateRawSignature(AsymmetricKeyParameter privateKey, byte[] md5AndSha1);

        byte[] GenerateRawSignature(SignatureAndHashAlgorithm algorithm,
            AsymmetricKeyParameter privateKey, byte[] hash);

        bool VerifyRawSignature(byte[] sigBytes, AsymmetricKeyParameter publicKey, byte[] md5AndSha1);

        bool VerifyRawSignature(SignatureAndHashAlgorithm algorithm, byte[] sigBytes,
            AsymmetricKeyParameter publicKey, byte[] hash);

        ISigner CreateSigner(AsymmetricKeyParameter privateKey);

        ISigner CreateSigner(SignatureAndHashAlgorithm algorithm, AsymmetricKeyParameter privateKey);

        ISigner CreateVerifyer(AsymmetricKeyParameter publicKey);

        ISigner CreateVerifyer(SignatureAndHashAlgorithm algorithm, AsymmetricKeyParameter publicKey);

        bool IsValidPublicKey(AsymmetricKeyParameter publicKey);
    }
}

#endif
