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

namespace Org.BouncyCastle.Asn1
{
    public class DerBoolean
        : Asn1Object
    {
        private readonly byte value;

        public static readonly DerBoolean False = new DerBoolean(false);
        public static readonly DerBoolean True  = new DerBoolean(true);

        /**
         * return a bool from the passed in object.
         *
         * @exception ArgumentException if the object cannot be converted.
         */
        public static DerBoolean GetInstance(
            object obj)
        {
            if (obj == null || obj is DerBoolean)
            {
                return (DerBoolean) obj;
            }

            throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
        }

        /**
         * return a DerBoolean from the passed in bool.
         */
        public static DerBoolean GetInstance(
            bool value)
        {
            return value ? True : False;
        }

        /**
         * return a Boolean from a tagged object.
         *
         * @param obj the tagged object holding the object we want
         * @param explicitly true if the object is meant to be explicitly
         *              tagged false otherwise.
         * @exception ArgumentException if the tagged object cannot
         *               be converted.
         */
        public static DerBoolean GetInstance(
            Asn1TaggedObject	obj,
            bool				isExplicit)
        {
            Asn1Object o = obj.GetObject();

            if (isExplicit || o is DerBoolean)
            {
                return GetInstance(o);
            }

            return FromOctetString(((Asn1OctetString)o).GetOctets());
        }

        public DerBoolean(
            byte[] val)
        {
            if (val.Length != 1)
                throw new ArgumentException("byte value should have 1 byte in it", "val");

            // TODO Are there any constraints on the possible byte values?
            this.value = val[0];
        }

        private DerBoolean(
            bool value)
        {
            this.value = value ? (byte)0xff : (byte)0;
        }

        public bool IsTrue
        {
            get { return value != 0; }
        }

        internal override void Encode(
            DerOutputStream derOut)
        {
            // TODO Should we make sure the byte value is one of '0' or '0xff' here?
            derOut.WriteEncoded(Asn1Tags.Boolean, new byte[]{ value });
        }

        protected override bool Asn1Equals(
            Asn1Object asn1Object)
        {
            DerBoolean other = asn1Object as DerBoolean;

            if (other == null)
                return false;

            return IsTrue == other.IsTrue;
        }

        protected override int Asn1GetHashCode()
        {
            return IsTrue.GetHashCode();
        }

        public override string ToString()
        {
            return IsTrue ? "TRUE" : "FALSE";
        }

        internal static DerBoolean FromOctetString(byte[] value)
        {
            if (value.Length != 1)
            {
                throw new ArgumentException("BOOLEAN value should have 1 byte in it", "value");
            }

            byte b = value[0];

            return b == 0 ? False : b == 0xFF ? True : new DerBoolean(value);
        }
    }
}

#endif
