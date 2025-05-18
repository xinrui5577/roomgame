/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
namespace Org.BouncyCastle.Asn1
{
	/**
	 * DER TaggedObject - in ASN.1 notation this is any object preceded by
	 * a [n] where n is some number - these are assumed to follow the construction
	 * rules (as with sequences).
	 */
	public class DerTaggedObject
		: Asn1TaggedObject
	{
		/**
		 * @param tagNo the tag number for this object.
		 * @param obj the tagged object.
		 */
		public DerTaggedObject(
			int				tagNo,
			Asn1Encodable	obj)
			: base(tagNo, obj)
		{
		}

		/**
		 * @param explicitly true if an explicitly tagged object.
		 * @param tagNo the tag number for this object.
		 * @param obj the tagged object.
		 */
		public DerTaggedObject(
			bool			explicitly,
			int				tagNo,
			Asn1Encodable	obj)
			: base(explicitly, tagNo, obj)
		{
		}

		/**
		 * create an implicitly tagged object that contains a zero
		 * length sequence.
		 */
		public DerTaggedObject(
			int tagNo)
			: base(false, tagNo, DerSequence.Empty)
		{
		}

		internal override void Encode(
			DerOutputStream derOut)
		{
			if (!IsEmpty())
			{
				byte[] bytes = obj.GetDerEncoded();

				if (explicitly)
				{
					derOut.WriteEncoded(Asn1Tags.Constructed | Asn1Tags.Tagged, tagNo, bytes);
				}
				else
				{
					//
					// need to mark constructed types... (preserve Constructed tag)
					//
					int flags = (bytes[0] & Asn1Tags.Constructed) | Asn1Tags.Tagged;
					derOut.WriteTag(flags, tagNo);
					derOut.Write(bytes, 1, bytes.Length - 1);
				}
			}
			else
			{
				derOut.WriteEncoded(Asn1Tags.Constructed | Asn1Tags.Tagged, tagNo, new byte[0]);
			}
		}
	}
}

#endif
