/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

namespace Org.BouncyCastle.Crypto.Tls
{
    public abstract class ExtensionType
    {
        /*
         * RFC 2546 2.3.
         */
        public const int server_name = 0;
        public const int max_fragment_length = 1;
        public const int client_certificate_url = 2;
        public const int trusted_ca_keys = 3;
        public const int truncated_hmac = 4;
        public const int status_request = 5;

        /*
         * RFC 4681
         */
        public const int user_mapping = 6;

        /*
         * RFC 4492 5.1.
         */
        public const int elliptic_curves = 10;
        public const int ec_point_formats = 11;

        /*
         * RFC 5054 2.8.1.
         */
        public const int srp = 12;

        /*
         * RFC 5246 7.4.1.4.
         */
        public const int signature_algorithms = 13;

        /*
         * RFC 5764 9.
         */
        public const int use_srtp = 14;

        /*
         * RFC 6520 6.
         */
        public const int heartbeat = 15;

        /*
         * RFC 7366
         */
        public const int encrypt_then_mac = 22;

        /*
         * draft-ietf-tls-session-hash-04
         * 
         * NOTE: Early code-point assignment
         */
        public const int extended_master_secret = 23;

        /*
         * RFC 5077 7.
         */
        public const int session_ticket = 35;

        /*
         * draft-ietf-tls-negotiated-ff-dhe-01
         * 
         * WARNING: Placeholder value; the real value is TBA
         */
        public static readonly int negotiated_ff_dhe_groups = 101;

        /*
         * RFC 5746 3.2.
         */
        public const int renegotiation_info = 0xff01;
    }
}

#endif
