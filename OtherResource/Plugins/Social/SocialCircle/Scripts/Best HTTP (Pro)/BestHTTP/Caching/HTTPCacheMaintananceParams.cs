/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_CACHING && (!UNITY_WEBGL || UNITY_EDITOR)

using System;

namespace BestHTTP.Caching
{
    public sealed class HTTPCacheMaintananceParams
    {
        /// <summary>
        /// Delete cache entries that accessed older then this value. If TimeSpan.FromSeconds(0) is used then all cache entries will be deleted. With TimeSpan.FromDays(2) entries that older then two days will be deleted.
        /// </summary>
        public TimeSpan DeleteOlder { get; private set; }

        /// <summary>
        /// If the cache is larger then the MaxCacheSize after the first maintanance step, then the maintanance job will forcedelete cache entries starting with the oldest last accessed one.
        /// </summary>
        public ulong MaxCacheSize { get; private set; }

        public HTTPCacheMaintananceParams(TimeSpan deleteOlder, ulong maxCacheSize)
        {
            this.DeleteOlder = deleteOlder;
            this.MaxCacheSize = maxCacheSize;
        }
    }
}

#endif
