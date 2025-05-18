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
     * interface that a public/private key pair generator should conform to.
     */
    public interface IAsymmetricCipherKeyPairGenerator
    {
        /**
         * intialise the key pair generator.
         *
         * @param the parameters the key pair is to be initialised with.
         */
        void Init(KeyGenerationParameters parameters);

        /**
         * return an AsymmetricCipherKeyPair containing the Generated keys.
         *
         * @return an AsymmetricCipherKeyPair containing the Generated keys.
         */
        AsymmetricCipherKeyPair GenerateKeyPair();
    }
}

#endif
