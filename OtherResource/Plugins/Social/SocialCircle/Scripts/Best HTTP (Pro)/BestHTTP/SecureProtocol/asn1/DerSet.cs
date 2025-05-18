/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	/**
	 * A Der encoded set object
	 */
	public class DerSet
		: Asn1Set
	{
		public static readonly DerSet Empty = new DerSet();

		public static DerSet FromVector(
			Asn1EncodableVector v)
		{
			return v.Count < 1 ? Empty : new DerSet(v);
		}

		internal static DerSet FromVector(
			Asn1EncodableVector	v,
			bool				needsSorting)
		{
			return v.Count < 1 ? Empty : new DerSet(v, needsSorting);
		}

		/**
		 * create an empty set
		 */
		public DerSet()
			: base(0)
		{
		}

		/**
		 * @param obj - a single object that makes up the set.
		 */
		public DerSet(
			Asn1Encodable obj)
			: base(1)
		{
			AddObject(obj);
		}

		public DerSet(
			params Asn1Encodable[] v)
			: base(v.Length)
		{
			foreach (Asn1Encodable o in v)
			{
				AddObject(o);
			}

			Sort();
		}

		/**
		 * @param v - a vector of objects making up the set.
		 */
		public DerSet(
			Asn1EncodableVector v)
			: this(v, true)
		{
		}

		internal DerSet(
			Asn1EncodableVector	v,
			bool				needsSorting)
			: base(v.Count)
		{
			foreach (Asn1Encodable o in v)
			{
				AddObject(o);
			}

			if (needsSorting)
			{
				Sort();
			}
		}

		/*
		 * A note on the implementation:
		 * <p>
		 * As Der requires the constructed, definite-length model to
		 * be used for structured types, this varies slightly from the
		 * ASN.1 descriptions given. Rather than just outputing Set,
		 * we also have to specify Constructed, and the objects length.
		 */
		internal override void Encode(
			DerOutputStream derOut)
		{
			// TODO Intermediate buffer could be avoided if we could calculate expected length
			MemoryStream bOut = new MemoryStream();
			DerOutputStream dOut = new DerOutputStream(bOut);

			foreach (Asn1Encodable obj in this)
			{
				dOut.WriteObject(obj);
			}

			dOut.Dispose();

			byte[] bytes = bOut.ToArray();

			derOut.WriteEncoded(Asn1Tags.Set | Asn1Tags.Constructed, bytes);
		}
	}
}

#endif
