/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
using Org.BouncyCastle.Asn1;
using System;
using System.Collections;

using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Asn1.Pkcs
{
    public class DHParameter
        : Asn1Encodable
    {
        internal DerInteger p, g, l;

		public DHParameter(
            BigInteger	p,
            BigInteger	g,
            int			l)
        {
            this.p = new DerInteger(p);
            this.g = new DerInteger(g);

			if (l != 0)
            {
                this.l = new DerInteger(l);
            }
        }

		public DHParameter(
            Asn1Sequence seq)
        {
            IEnumerator e = seq.GetEnumerator();

			e.MoveNext();
            p = (DerInteger)e.Current;

			e.MoveNext();
            g = (DerInteger)e.Current;

			if (e.MoveNext())
            {
                l = (DerInteger) e.Current;
            }
        }

		public BigInteger P
		{
			get { return p.PositiveValue; }
		}

		public BigInteger G
		{
			get { return g.PositiveValue; }
		}

		public BigInteger L
		{
            get { return l == null ? null : l.PositiveValue; }
        }

		public override Asn1Object ToAsn1Object()
        {
            Asn1EncodableVector v = new Asn1EncodableVector(p, g);

			if (this.l != null)
            {
                v.Add(l);
            }

			return new DerSequence(v);
        }
    }
}

#endif
