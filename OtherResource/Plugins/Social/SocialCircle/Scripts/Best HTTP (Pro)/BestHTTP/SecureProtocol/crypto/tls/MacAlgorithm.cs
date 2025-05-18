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
    /// <summary>RFC 2246</summary>
    /// <remarks>
    /// Note that the values here are implementation-specific and arbitrary. It is recommended not to
    /// depend on the particular values (e.g. serialization).
    /// </remarks>
    public abstract class MacAlgorithm
    {
        public const int cls_null = 0;
        public const int md5 = 1;
        public const int sha = 2;

        /*
         * RFC 5246
         */
        public const int hmac_md5 = md5;
        public const int hmac_sha1 = sha;
        public const int hmac_sha256 = 3;
        public const int hmac_sha384 = 4;
        public const int hmac_sha512 = 5;
    }
}

#endif
