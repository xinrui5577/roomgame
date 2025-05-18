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
    public abstract class ClientCertificateType
    {
        /*
         *  RFC 4346 7.4.4
         */
        public const byte rsa_sign = 1;
        public const byte dss_sign = 2;
        public const byte rsa_fixed_dh = 3;
        public const byte dss_fixed_dh = 4;
        public const byte rsa_ephemeral_dh_RESERVED = 5;
        public const byte dss_ephemeral_dh_RESERVED = 6;
        public const byte fortezza_dms_RESERVED = 20;

        /*
        * RFC 4492 5.5
        */
        public const byte ecdsa_sign = 64;
        public const byte rsa_fixed_ecdh = 65;
        public const byte ecdsa_fixed_ecdh = 66;
    }
}

#endif
