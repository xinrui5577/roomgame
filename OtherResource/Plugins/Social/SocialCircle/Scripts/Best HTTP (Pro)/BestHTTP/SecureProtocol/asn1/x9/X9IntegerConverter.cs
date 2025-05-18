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

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;

namespace Org.BouncyCastle.Asn1.X9
{
    public abstract class X9IntegerConverter
    {
        public static int GetByteLength(ECFieldElement fe)
        {
            return (fe.FieldSize + 7) / 8;
        }

        public static int GetByteLength(ECCurve c)
        {
            return (c.FieldSize + 7) / 8;
        }

        public static byte[] IntegerToBytes(BigInteger s, int qLength)
        {
            byte[] bytes = s.ToByteArrayUnsigned();

            if (qLength < bytes.Length)
            {
                byte[] tmp = new byte[qLength];
                Array.Copy(bytes, bytes.Length - tmp.Length, tmp, 0, tmp.Length);
                return tmp;
            }
            else if (qLength > bytes.Length)
            {
                byte[] tmp = new byte[qLength];
                Array.Copy(bytes, 0, tmp, tmp.Length - bytes.Length, bytes.Length);
                return tmp;
            }

            return bytes;
        }
    }
}

#endif
