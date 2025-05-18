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
using System.Text;

namespace Org.BouncyCastle.Utilities
{
    /// <summary> General string utilities.</summary>
    public abstract class Strings
    {
        internal static bool IsOneOf(string s, params string[] candidates)
        {
            foreach (string candidate in candidates)
            {
                if (s == candidate)
                    return true;
            }
            return false;
        }

        public static string FromByteArray(
            byte[] bs)
        {
            char[] cs = new char[bs.Length];
            for (int i = 0; i < cs.Length; ++i)
            {
                cs[i] = Convert.ToChar(bs[i]);
            }
            return new string(cs);
        }

        public static byte[] ToByteArray(
            char[] cs)
        {
            byte[] bs = new byte[cs.Length];
            for (int i = 0; i < bs.Length; ++i)
            {
                bs[i] = Convert.ToByte(cs[i]);
            }
            return bs;
        }

        public static byte[] ToByteArray(
            string s)
        {
            byte[] bs = new byte[s.Length];
            for (int i = 0; i < bs.Length; ++i)
            {
                bs[i] = Convert.ToByte(s[i]);
            }
            return bs;
        }

        public static string FromAsciiByteArray(
            byte[] bytes)
        {
#if SILVERLIGHT || NETFX_CORE || UNITY_WP8
            // TODO Check for non-ASCII bytes in input?
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
#else
            return Encoding.ASCII.GetString(bytes, 0, bytes.Length);
#endif
        }

        public static byte[] ToAsciiByteArray(
            char[] cs)
        {
#if SILVERLIGHT || NETFX_CORE || UNITY_WP8
            // TODO Check for non-ASCII characters in input?
            return Encoding.UTF8.GetBytes(cs);
#else
            return Encoding.ASCII.GetBytes(cs);
#endif
        }

        public static byte[] ToAsciiByteArray(
            string s)
        {
#if SILVERLIGHT || NETFX_CORE || UNITY_WP8
            // TODO Check for non-ASCII characters in input?
            return Encoding.UTF8.GetBytes(s);
#else
            return Encoding.ASCII.GetBytes(s);
#endif
        }

        public static string FromUtf8ByteArray(
            byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public static byte[] ToUtf8ByteArray(
            char[] cs)
        {
            return Encoding.UTF8.GetBytes(cs);
        }

        public static byte[] ToUtf8ByteArray(
            string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }
    }
}

#endif
