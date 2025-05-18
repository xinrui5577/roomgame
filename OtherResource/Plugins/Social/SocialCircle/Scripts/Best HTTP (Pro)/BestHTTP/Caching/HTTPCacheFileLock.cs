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
using System.Collections.Generic;

namespace BestHTTP.Caching
{
    sealed class HTTPCacheFileLock
    {
        private static Dictionary<Uri, object> FileLocks = new Dictionary<Uri, object>();
        private static object SyncRoot = new object();

        internal static object Acquire(Uri uri)
        {
            lock (SyncRoot)
            {
                object fileLock;
                if (!FileLocks.TryGetValue(uri, out fileLock))
                    FileLocks.Add(uri, fileLock = new object());

                return fileLock;
            }
        }

        internal static void Remove(Uri uri)
        {
            lock (SyncRoot)
            {
                if (FileLocks.ContainsKey(uri))
                    FileLocks.Remove(uri);
            }
        }

        internal static void Clear()
        {
            lock (SyncRoot)
            {
                FileLocks.Clear();
            }
        }
    }
}

#endif
