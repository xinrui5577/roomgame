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
using System.Globalization;
using System.IO;
using System.Text;

#if SILVERLIGHT || NETFX_CORE || UNITY_WP8
using System.Collections.Generic;
#else
using System.Collections;
#endif

namespace Org.BouncyCastle.Utilities
{
    internal abstract class Platform
    {
#if NETCF_1_0 || NETCF_2_0
        private static string GetNewLine()
        {
            MemoryStream buf = new MemoryStream();
            StreamWriter w = new StreamWriter(buf, Encoding.UTF8);
            w.WriteLine();
            w.Close();
            byte[] bs = buf.ToArray();
            return Encoding.UTF8.GetString(bs, 0, bs.Length);
        }
#else
        private static string GetNewLine()
        {
            return Environment.NewLine;
        }
#endif

        internal static int CompareIgnoreCase(string a, string b)
        {
#if SILVERLIGHT || NETFX_CORE || UNITY_WP8
            return String.Compare(a, b, StringComparison.OrdinalIgnoreCase);
#else
            return String.Compare(a, b, true);
#endif
        }

#if NETCF_1_0
        internal static Exception CreateNotImplementedException(
            string message)
        {
            return new Exception("Not implemented: " + message);
        }

        internal static bool Equals(
            object	a,
            object	b)
        {
            return a == b || (a != null && b != null && a.Equals(b));
        }
#else
        internal static Exception CreateNotImplementedException(
            string message)
        {
            return new NotImplementedException(message);
        }
#endif

#if SILVERLIGHT || NETFX_CORE || UNITY_WP8
        internal static System.Collections.IList CreateArrayList()
        {
            return new List<object>();
        }
        internal static System.Collections.IList CreateArrayList(int capacity)
        {
            return new List<object>(capacity);
        }
        internal static System.Collections.IList CreateArrayList(System.Collections.ICollection collection)
        {
            System.Collections.IList result = new List<object>(collection.Count);
            foreach (object o in collection)
            {
                result.Add(o);
            }
            return result;
        }
        internal static System.Collections.IList CreateArrayList(System.Collections.IEnumerable collection)
        {
            System.Collections.IList result = new List<object>();
            foreach (object o in collection)
            {
                result.Add(o);
            }
            return result;
        }
        internal static System.Collections.IDictionary CreateHashtable()
        {
            return new Dictionary<object, object>();
        }
        internal static System.Collections.IDictionary CreateHashtable(int capacity)
        {
            return new Dictionary<object, object>(capacity);
        }
        internal static System.Collections.IDictionary CreateHashtable(System.Collections.IDictionary dictionary)
        {
            System.Collections.IDictionary result = new Dictionary<object, object>(dictionary.Count);
            foreach (System.Collections.DictionaryEntry entry in dictionary)
            {
                result.Add(entry.Key, entry.Value);
            }
            return result;
        }
#else
        internal static System.Collections.IList CreateArrayList()
        {
            return new ArrayList();
        }
        internal static System.Collections.IList CreateArrayList(int capacity)
        {
            return new ArrayList(capacity);
        }
        internal static System.Collections.IList CreateArrayList(System.Collections.ICollection collection)
        {
            return new ArrayList(collection);
        }
        internal static System.Collections.IList CreateArrayList(System.Collections.IEnumerable collection)
        {
            ArrayList result = new ArrayList();
            foreach (object o in collection)
            {
                result.Add(o);
            }
            return result;
        }
        internal static System.Collections.IDictionary CreateHashtable()
        {
            return new Hashtable();
        }
        internal static System.Collections.IDictionary CreateHashtable(int capacity)
        {
            return new Hashtable(capacity);
        }
        internal static System.Collections.IDictionary CreateHashtable(System.Collections.IDictionary dictionary)
        {
            return new Hashtable(dictionary);
        }
#endif

        internal static string ToLowerInvariant(string s)
        {
#if NETFX_CORE
            return s.ToLower();
#else
            return s.ToLower(CultureInfo.InvariantCulture);
#endif
        }

        internal static string ToUpperInvariant(string s)
        {
#if NETFX_CORE
            return s.ToUpper();
#else
            return s.ToUpper(CultureInfo.InvariantCulture);
#endif
        }

        internal static readonly string NewLine = GetNewLine();
    }
}

#endif
