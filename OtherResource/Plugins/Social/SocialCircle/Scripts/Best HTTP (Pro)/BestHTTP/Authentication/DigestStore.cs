/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System;
using System.Collections.Generic;

namespace BestHTTP.Authentication
{
    /// <summary>
    /// Stores and manages already received digest infos.
    /// </summary>
    internal static class DigestStore
    {
        private static Dictionary<string, Digest> Digests = new Dictionary<string, Digest>();
        
        private static object Locker = new object();

        /// <summary>
        /// Array of algorithms that the plugin supports. It's in the order of priority(first has the highest priority).
        /// </summary>
        private static string[] SupportedAlgorithms = new string[] { "digest", "basic" };

        public static Digest Get(Uri uri)
        {
            lock (Locker)
            {
                Digest digest = null;
                if (Digests.TryGetValue(uri.Host, out digest))
                    if (!digest.IsUriProtected(uri))
                        return null;
                return digest;
            }
        }

        /// <summary>
        /// It will retrive or create a new Digest for the given Uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static Digest GetOrCreate(Uri uri)
        {
            lock (Locker)
            {
                Digest digest = null;
                if (!Digests.TryGetValue(uri.Host, out digest))
                    Digests.Add(uri.Host, digest = new Digest(uri));
                return digest;
            }
        }

        public static void Remove(Uri uri)
        {
            lock(Locker)
                Digests.Remove(uri.Host);
        }

        public static string FindBest(List<string> authHeaders)
        {
            if (authHeaders == null || authHeaders.Count == 0)
                return string.Empty;

            List<string> headers = new List<string>(authHeaders.Count);
            for (int i = 0; i < authHeaders.Count; ++i)
                headers.Add(authHeaders[i].ToLower());

            for (int i = 0; i < SupportedAlgorithms.Length; ++i)
            {
                int idx = headers.FindIndex((header) => header.StartsWith(SupportedAlgorithms[i]));
                if (idx != -1)
                    return authHeaders[idx];
            }

            return string.Empty;
        }
    }
}
