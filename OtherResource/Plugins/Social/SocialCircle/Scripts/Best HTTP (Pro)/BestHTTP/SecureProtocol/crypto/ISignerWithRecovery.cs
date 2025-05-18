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
    /**
     * Signer with message recovery.
     */
    public interface ISignerWithRecovery
        : ISigner
    {
        /**
         * Returns true if the signer has recovered the full message as
         * part of signature verification.
         *
         * @return true if full message recovered.
         */
        bool HasFullMessage();

        /**
         * Returns a reference to what message was recovered (if any).
         *
         * @return full/partial message, null if nothing.
         */
        byte[] GetRecoveredMessage();

		/**
		 * Perform an update with the recovered message before adding any other data. This must
		 * be the first update method called, and calling it will result in the signer assuming
		 * that further calls to update will include message content past what is recoverable.
		 *
		 * @param signature the signature that we are in the process of verifying.
		 * @throws IllegalStateException
		 */
		void UpdateWithRecoveredMessage(byte[] signature);
	}
}

#endif
