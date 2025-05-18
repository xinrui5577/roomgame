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

namespace Org.BouncyCastle.Asn1
{
    /**
     * Der UTF8String object.
     */
    public class DerUtf8String
        : DerStringBase
    {
        private readonly string str;

		/**
         * return an UTF8 string from the passed in object.
         *
         * @exception ArgumentException if the object cannot be converted.
         */
        public static DerUtf8String GetInstance(
            object obj)
        {
            if (obj == null || obj is DerUtf8String)
            {
                return (DerUtf8String)obj;
            }

			throw new ArgumentException("illegal object in GetInstance: " + obj.GetType().Name);
        }

        /**
         * return an UTF8 string from a tagged object.
         *
         * @param obj the tagged object holding the object we want
         * @param explicitly true if the object is meant to be explicitly
         *              tagged false otherwise.
         * @exception ArgumentException if the tagged object cannot
         *               be converted.
         */
        public static DerUtf8String GetInstance(
            Asn1TaggedObject	obj,
            bool				isExplicit)
        {
			Asn1Object o = obj.GetObject();

			if (isExplicit || o is DerUtf8String)
			{
				return GetInstance(o);
			}

			return new DerUtf8String(Asn1OctetString.GetInstance(o).GetOctets());
        }

        /**
         * basic constructor - byte encoded string.
         */
        public DerUtf8String(
            byte[] str)
			: this(Encoding.UTF8.GetString(str, 0, str.Length))
        {
        }

		/**
         * basic constructor
         */
        public DerUtf8String(
            string str)
        {
			if (str == null)
				throw new ArgumentNullException("str");

			this.str = str;
        }

		public override string GetString()
        {
            return str;
        }

		protected override bool Asn1Equals(
			Asn1Object asn1Object)
		{
			DerUtf8String other = asn1Object as DerUtf8String;

			if (other == null)
				return false;

			return this.str.Equals(other.str);
        }

		internal override void Encode(
			DerOutputStream derOut)
        {
            derOut.WriteEncoded(Asn1Tags.Utf8String, Encoding.UTF8.GetBytes(str));
        }
    }
}

#endif
