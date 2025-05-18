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

namespace Org.BouncyCastle.Asn1
{
    public abstract class Asn1Object
		: Asn1Encodable
    {
		/// <summary>Create a base ASN.1 object from a byte array.</summary>
		/// <param name="data">The byte array to parse.</param>
		/// <returns>The base ASN.1 object represented by the byte array.</returns>
		/// <exception cref="IOException">If there is a problem parsing the data.</exception>
		public static Asn1Object FromByteArray(
			byte[] data)
		{
			try
			{
				return new Asn1InputStream(data).ReadObject();
			}
			catch (InvalidCastException)
			{
				throw new IOException("cannot recognise object in stream");    
			}
		}

		/// <summary>Read a base ASN.1 object from a stream.</summary>
		/// <param name="inStr">The stream to parse.</param>
		/// <returns>The base ASN.1 object represented by the byte array.</returns>
		/// <exception cref="IOException">If there is a problem parsing the data.</exception>
		public static Asn1Object FromStream(
			Stream inStr)
		{
			try
			{
				return new Asn1InputStream(inStr).ReadObject();
			}
			catch (InvalidCastException)
			{
				throw new IOException("cannot recognise object in stream");    
			}
		}

		public sealed override Asn1Object ToAsn1Object()
        {
            return this;
        }

		internal abstract void Encode(DerOutputStream derOut);

		protected abstract bool Asn1Equals(Asn1Object asn1Object);
		protected abstract int Asn1GetHashCode();

		internal bool CallAsn1Equals(Asn1Object obj)
		{
			return Asn1Equals(obj);
		}

		internal int CallAsn1GetHashCode()
		{
			return Asn1GetHashCode();
		}
	}
}

#endif
