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

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Asn1.Oiw
{
    public class ElGamalParameter
        : Asn1Encodable
    {
        internal DerInteger p, g;

		public ElGamalParameter(
            BigInteger	p,
            BigInteger	g)
        {
            this.p = new DerInteger(p);
            this.g = new DerInteger(g);
        }

		public ElGamalParameter(
            Asn1Sequence seq)
        {
			if (seq.Count != 2)
				throw new ArgumentException("Wrong number of elements in sequence", "seq");

			p = DerInteger.GetInstance(seq[0]);
			g = DerInteger.GetInstance(seq[1]);
        }

		public BigInteger P
		{
			get { return p.PositiveValue; }
		}

		public BigInteger G
		{
			get { return g.PositiveValue; }
		}

		public override Asn1Object ToAsn1Object()
        {
			return new DerSequence(p, g);
        }
    }
}

#endif
