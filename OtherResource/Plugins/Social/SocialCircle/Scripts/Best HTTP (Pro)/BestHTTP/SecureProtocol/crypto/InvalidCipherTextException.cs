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
     * this exception is thrown whenever we find something we don't expect in a
     * message.
     */
#if !(NETCF_1_0 || NETCF_2_0 || SILVERLIGHT || NETFX_CORE)
    [Serializable]
#endif
    public class InvalidCipherTextException
		: CryptoException
    {
		/**
		* base constructor.
		*/
        public InvalidCipherTextException()
        {
        }

		/**
         * create a InvalidCipherTextException with the given message.
         *
         * @param message the message to be carried with the exception.
         */
        public InvalidCipherTextException(
            string message)
			: base(message)
        {
        }

		public InvalidCipherTextException(
            string		message,
            Exception	exception)
			: base(message, exception)
        {
        }
    }
}

#endif
