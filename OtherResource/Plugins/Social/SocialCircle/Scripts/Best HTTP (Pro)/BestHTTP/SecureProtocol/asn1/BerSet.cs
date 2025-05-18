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
    public class BerSet
        : DerSet
    {
		public static new readonly BerSet Empty = new BerSet();

		public static new BerSet FromVector(
			Asn1EncodableVector v)
		{
			return v.Count < 1 ? Empty : new BerSet(v);
		}

		internal static new BerSet FromVector(
			Asn1EncodableVector v,
			bool				needsSorting)
		{
			return v.Count < 1 ? Empty : new BerSet(v, needsSorting);
		}

		/**
         * create an empty sequence
         */
        public BerSet()
        {
        }

        /**
         * create a set containing one object
         */
        public BerSet(Asn1Encodable obj) : base(obj)
        {
        }

        /**
         * create a set containing a vector of objects.
         */
        public BerSet(Asn1EncodableVector v) : base(v, false)
        {
        }

        internal BerSet(Asn1EncodableVector v, bool needsSorting) : base(v, needsSorting)
        {
        }

        /*
         */
        internal override void Encode(
            DerOutputStream derOut)
        {
            if (derOut is Asn1OutputStream || derOut is BerOutputStream)
            {
                derOut.WriteByte(Asn1Tags.Set | Asn1Tags.Constructed);
                derOut.WriteByte(0x80);

                foreach (Asn1Encodable o in this)
				{
                    derOut.WriteObject(o);
                }

                derOut.WriteByte(0x00);
                derOut.WriteByte(0x00);
            }
            else
            {
                base.Encode(derOut);
            }
        }
    }
}

#endif
