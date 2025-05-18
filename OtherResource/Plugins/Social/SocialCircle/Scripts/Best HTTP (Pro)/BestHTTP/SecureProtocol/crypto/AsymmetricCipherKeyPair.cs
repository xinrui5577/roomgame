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
     * a holding class for public/private parameter pairs.
     */
    public class AsymmetricCipherKeyPair
    {
        private readonly AsymmetricKeyParameter publicParameter;
        private readonly AsymmetricKeyParameter privateParameter;

		/**
         * basic constructor.
         *
         * @param publicParam a public key parameters object.
         * @param privateParam the corresponding private key parameters.
         */
        public AsymmetricCipherKeyPair(
            AsymmetricKeyParameter    publicParameter,
            AsymmetricKeyParameter    privateParameter)
        {
			if (publicParameter.IsPrivate)
				throw new ArgumentException("Expected a public key", "publicParameter");
			if (!privateParameter.IsPrivate)
				throw new ArgumentException("Expected a private key", "privateParameter");

			this.publicParameter = publicParameter;
            this.privateParameter = privateParameter;
        }

		/**
         * return the public key parameters.
         *
         * @return the public key parameters.
         */
        public AsymmetricKeyParameter Public
        {
            get { return publicParameter; }
        }

		/**
         * return the private key parameters.
         *
         * @return the private key parameters.
         */
        public AsymmetricKeyParameter Private
        {
            get { return privateParameter; }
        }
    }
}

#endif
