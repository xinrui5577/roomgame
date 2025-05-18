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
    /// <summary>RFC 5705</summary>
    public abstract class ExporterLabel
    {
        /*
         * RFC 5246
         */
        public const string client_finished = "client finished";
        public const string server_finished = "server finished";
        public const string master_secret = "master secret";
        public const string key_expansion = "key expansion";

        /*
         * RFC 5216
         */
        public const string client_EAP_encryption = "client EAP encryption";

        /*
         * RFC 5281
         */
        public const string ttls_keying_material = "ttls keying material";
        public const string ttls_challenge = "ttls challenge";

        /*
         * RFC 5764
         */
        public const string dtls_srtp = "EXTRACTOR-dtls_srtp";

        /*
         * draft-ietf-tls-session-hash-04
         */
        public static readonly string extended_master_secret = "extended master secret";
    }
}

#endif
