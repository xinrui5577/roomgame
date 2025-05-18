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

namespace Org.BouncyCastle.Asn1.X509
{
    /**
     * The default converter for X509 DN entries when going from their
     * string value to ASN.1 strings.
     */
    public class X509DefaultEntryConverter
        : X509NameEntryConverter
    {
        /**
         * Apply default conversion for the given value depending on the oid
         * and the character range of the value.
         *
         * @param oid the object identifier for the DN entry
         * @param value the value associated with it
         * @return the ASN.1 equivalent for the string value.
         */
        public override Asn1Object GetConvertedValue(
            DerObjectIdentifier	oid,
            string				value)
        {
            if (value.Length != 0 && value[0] == '#')
            {
				try
				{
					return ConvertHexEncoded(value, 1);
				}
				catch (IOException)
				{
					throw new Exception("can't recode value for oid " + oid.Id);
				}
			}

			if (value.Length != 0 && value[0] == '\\')
			{
				value = value.Substring(1);
			}

			if (oid.Equals(X509Name.EmailAddress) || oid.Equals(X509Name.DC))
            {
                return new DerIA5String(value);
            }

			if (oid.Equals(X509Name.DateOfBirth)) // accept time string as well as # (for compatibility)
			{
				return new DerGeneralizedTime(value);
			}

			if (oid.Equals(X509Name.C)
				|| oid.Equals(X509Name.SerialNumber)
				|| oid.Equals(X509Name.DnQualifier)
				|| oid.Equals(X509Name.TelephoneNumber))
			{
				return new DerPrintableString(value);
			}

			return new DerUtf8String(value);
        }
    }
}

#endif
