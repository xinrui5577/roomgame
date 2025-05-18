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
    /// <summary>
    /// Class to decode and encode Hex.
    /// </summary>
    public sealed class Hex
    {
        private static readonly IEncoder encoder = new HexEncoder();

        private Hex()
        {
        }

        public static string ToHexString(
            byte[] data)
        {
            return ToHexString(data, 0, data.Length);
        }

        public static string ToHexString(
            byte[]	data,
            int		off,
            int		length)
        {
            byte[] hex = Encode(data, off, length);
            return Strings.FromAsciiByteArray(hex);
        }

        /**
         * encode the input data producing a Hex encoded byte array.
         *
         * @return a byte array containing the Hex encoded data.
         */
        public static byte[] Encode(
            byte[] data)
        {
            return Encode(data, 0, data.Length);
        }

        /**
         * encode the input data producing a Hex encoded byte array.
         *
         * @return a byte array containing the Hex encoded data.
         */
        public static byte[] Encode(
            byte[]	data,
            int		off,
            int		length)
        {
            MemoryStream bOut = new MemoryStream(length * 2);

            encoder.Encode(data, off, length, bOut);

            return bOut.ToArray();
        }

        /**
         * Hex encode the byte data writing it to the given output stream.
         *
         * @return the number of bytes produced.
         */
        public static int Encode(
            byte[]	data,
            Stream	outStream)
        {
            return encoder.Encode(data, 0, data.Length, outStream);
        }

        /**
         * Hex encode the byte data writing it to the given output stream.
         *
         * @return the number of bytes produced.
         */
        public static int Encode(
            byte[]	data,
            int		off,
            int		length,
            Stream	outStream)
        {
            return encoder.Encode(data, off, length, outStream);
        }

        /**
         * decode the Hex encoded input data. It is assumed the input data is valid.
         *
         * @return a byte array representing the decoded data.
         */
        public static byte[] Decode(
            byte[] data)
        {
            MemoryStream bOut = new MemoryStream((data.Length + 1) / 2);

            encoder.Decode(data, 0, data.Length, bOut);

            return bOut.ToArray();
        }

        /**
         * decode the Hex encoded string data - whitespace will be ignored.
         *
         * @return a byte array representing the decoded data.
         */
        public static byte[] Decode(
            string data)
        {
            MemoryStream bOut = new MemoryStream((data.Length + 1) / 2);

            encoder.DecodeString(data, bOut);

            return bOut.ToArray();
        }

        /**
         * decode the Hex encoded string data writing it to the given output stream,
         * whitespace characters will be ignored.
         *
         * @return the number of bytes produced.
         */
        public static int Decode(
            string	data,
            Stream	outStream)
        {
            return encoder.DecodeString(data, outStream);
        }
    }
}

#endif
