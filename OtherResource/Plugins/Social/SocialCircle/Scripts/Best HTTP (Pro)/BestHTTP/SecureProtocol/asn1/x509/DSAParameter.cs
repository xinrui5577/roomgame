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
using System.Collections;

using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Asn1.X509
{
    public class DsaParameter
        : Asn1Encodable
    {
        internal readonly DerInteger p, q, g;

		public static DsaParameter GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
        }

		public static DsaParameter GetInstance(
            object obj)
        {
            if(obj == null || obj is DsaParameter)
            {
                return (DsaParameter) obj;
            }

			if(obj is Asn1Sequence)
            {
                return new DsaParameter((Asn1Sequence) obj);
            }

			throw new ArgumentException("Invalid DsaParameter: " + obj.GetType().Name);
        }

		public DsaParameter(
            BigInteger	p,
            BigInteger	q,
            BigInteger	g)
        {
            this.p = new DerInteger(p);
            this.q = new DerInteger(q);
            this.g = new DerInteger(g);
        }

		private DsaParameter(
            Asn1Sequence seq)
        {
			if (seq.Count != 3)
				throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");

			this.p = DerInteger.GetInstance(seq[0]);
			this.q = DerInteger.GetInstance(seq[1]);
			this.g = DerInteger.GetInstance(seq[2]);
        }

		public BigInteger P
		{
			get { return p.PositiveValue; }
		}

		public BigInteger Q
		{
			get { return q.PositiveValue; }
		}

		public BigInteger G
		{
			get { return g.PositiveValue; }
		}

		public override Asn1Object ToAsn1Object()
        {
			return new DerSequence(p, q, g);
        }
    }
}

#endif
