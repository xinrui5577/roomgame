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

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1
{
    public class DerGeneralString
        : DerStringBase
    {
        private readonly string str;

        public static DerGeneralString GetInstance(
            object obj)
        {
            if (obj == null || obj is DerGeneralString)
            {
                return (DerGeneralString) obj;
            }

			throw new ArgumentException("illegal object in GetInstance: "
                    + obj.GetType().Name);
        }

        public static DerGeneralString GetInstance(
            Asn1TaggedObject	obj,
            bool				isExplicit)
        {
			Asn1Object o = obj.GetObject();

			if (isExplicit || o is DerGeneralString)
			{
				return GetInstance(o);
			}

			return new DerGeneralString(((Asn1OctetString)o).GetOctets());
        }

        public DerGeneralString(
			byte[] str)
			: this(Strings.FromAsciiByteArray(str))
        {
        }

		public DerGeneralString(
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

		public byte[] GetOctets()
        {
            return Strings.ToAsciiByteArray(str);
        }

		internal override void Encode(
			DerOutputStream derOut)
        {
            derOut.WriteEncoded(Asn1Tags.GeneralString, GetOctets());
        }

		protected override bool Asn1Equals(
			Asn1Object asn1Object)
        {
			DerGeneralString other = asn1Object as DerGeneralString;

			if (other == null)
				return false;

			return this.str.Equals(other.str);
        }
    }
}

#endif
