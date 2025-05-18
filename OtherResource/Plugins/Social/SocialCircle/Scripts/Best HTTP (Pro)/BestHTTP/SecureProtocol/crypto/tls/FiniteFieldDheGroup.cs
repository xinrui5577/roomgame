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
    /*
     * draft-ietf-tls-negotiated-ff-dhe-01
     */
    public abstract class FiniteFieldDheGroup
    {
        public const byte ffdhe2432 = 0;
        public const byte ffdhe3072 = 1;
        public const byte ffdhe4096 = 2;
        public const byte ffdhe6144 = 3;
        public const byte ffdhe8192 = 4;

        public static bool IsValid(byte group)
        {
            return group >= ffdhe2432 && group <= ffdhe8192;
        }
    }
}

#endif
