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
    /// <summary>
    /// RFC 4492 5.4
    /// </summary>
    public abstract class ECCurveType
    {
        /**
         * Indicates the elliptic curve domain parameters are conveyed verbosely, and the
         * underlying finite field is a prime field.
         */
        public const byte explicit_prime = 1;

        /**
         * Indicates the elliptic curve domain parameters are conveyed verbosely, and the
         * underlying finite field is a characteristic-2 field.
         */
        public const byte explicit_char2 = 2;

        /**
         * Indicates that a named curve is used. This option SHOULD be used when applicable.
         */
        public const byte named_curve = 3;

        /*
         * Values 248 through 255 are reserved for private use.
         */
    }
}

#endif
