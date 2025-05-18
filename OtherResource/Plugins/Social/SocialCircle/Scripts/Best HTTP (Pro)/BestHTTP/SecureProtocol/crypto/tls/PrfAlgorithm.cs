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
    /// <summary>RFC 5246</summary>
    /// <remarks>
    /// Note that the values here are implementation-specific and arbitrary. It is recommended not to
    /// depend on the particular values (e.g. serialization).
    /// </remarks>
    public abstract class PrfAlgorithm
    {
        /*
         * Placeholder to refer to the legacy TLS algorithm
         */
        public const int tls_prf_legacy = 0;

        public const int tls_prf_sha256 = 1;

        /*
         * Implied by RFC 5288
         */
        public const int tls_prf_sha384 = 2;
    }
}

#endif
