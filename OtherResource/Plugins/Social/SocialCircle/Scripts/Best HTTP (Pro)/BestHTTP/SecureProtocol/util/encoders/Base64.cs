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
using System.IO;
using System.Text;

namespace Org.BouncyCastle.Utilities.Encoders
{
    public sealed class Base64
    {
        private Base64()
        {
        }

        public static string ToBase64String(
            byte[] data)
        {
            return Convert.ToBase64String(data, 0, data.Length);
        }

        public static string ToBase64String(
            byte[] data,
            int off,
            int length)
        {
            return Convert.ToBase64String(data, off, length);
        }

        /**
         * encode the input data producing a base 64 encoded byte array.
         *
         * @return a byte array containing the base 64 encoded data.
         */
        public static byte[] Encode(
            byte[] data)
        {
            return Encode(data, 0, data.Length);
        }

        /**
         * encode the input data producing a base 64 encoded byte array.
         *
         * @return a byte array containing the base 64 encoded data.
         */
        public static byte[] Encode(
            byte[] data,
            int off,
            int length)
        {
            string s = Convert.ToBase64String(data, off, length);
            return Strings.ToAsciiByteArray(s);
        }

        /**
         * Encode the byte data to base 64 writing it to the given output stream.
         *
         * @return the number of bytes produced.
         */
        public static int Encode(
            byte[]	data,
            Stream	outStream)
        {
            byte[] encoded = Encode(data);
            outStream.Write(encoded, 0, encoded.Length);
            return encoded.Length;
        }

        /**
         * Encode the byte data to base 64 writing it to the given output stream.
         *
         * @return the number of bytes produced.
         */
        public static int Encode(
            byte[]	data,
            int		off,
            int		length,
            Stream	outStream)
        {
            byte[] encoded = Encode(data, off, length);
            outStream.Write(encoded, 0, encoded.Length);
            return encoded.Length;
        }

        /**
         * decode the base 64 encoded input data. It is assumed the input data is valid.
         *
         * @return a byte array representing the decoded data.
         */
        public static byte[] Decode(
            byte[] data)
        {
            string s = Strings.FromAsciiByteArray(data);
            return Convert.FromBase64String(s);
        }

        /**
         * decode the base 64 encoded string data - whitespace will be ignored.
         *
         * @return a byte array representing the decoded data.
         */
        public static byte[] Decode(
            string data)
        {
            return Convert.FromBase64String(data);
        }

        /**
         * decode the base 64 encoded string data writing it to the given output stream,
         * whitespace characters will be ignored.
         *
         * @return the number of bytes produced.
         */
        public static int Decode(
            string	data,
            Stream	outStream)
        {
            byte[] decoded = Decode(data);
            outStream.Write(decoded, 0, decoded.Length);
            return decoded.Length;
        }
    }
}

#endif
