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
     * RFC 6520
     */
    public abstract class HeartbeatMode
    {
        public const byte peer_allowed_to_send = 1;
        public const byte peer_not_allowed_to_send = 2;

        public static bool IsValid(byte heartbeatMode)
        {
            return heartbeatMode >= peer_allowed_to_send && heartbeatMode <= peer_not_allowed_to_send;
        }
    }
}

#endif
