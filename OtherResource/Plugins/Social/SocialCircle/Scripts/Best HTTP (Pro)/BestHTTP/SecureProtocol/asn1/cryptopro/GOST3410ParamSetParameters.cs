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

namespace Org.BouncyCastle.Asn1.CryptoPro
{
    public class Gost3410ParamSetParameters
        : Asn1Encodable
    {
        private readonly int keySize;
        private readonly DerInteger	p, q, a;

		public static Gost3410ParamSetParameters GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
        }

		public static Gost3410ParamSetParameters GetInstance(
            object obj)
        {
            if (obj == null || obj is Gost3410ParamSetParameters)
            {
                return (Gost3410ParamSetParameters) obj;
            }

			if (obj is Asn1Sequence)
            {
                return new Gost3410ParamSetParameters((Asn1Sequence) obj);
            }

			throw new ArgumentException("Invalid GOST3410Parameter: " + obj.GetType().Name);
        }

		public Gost3410ParamSetParameters(
            int			keySize,
            BigInteger	p,
            BigInteger	q,
            BigInteger	a)
        {
            this.keySize = keySize;
            this.p = new DerInteger(p);
            this.q = new DerInteger(q);
            this.a = new DerInteger(a);
        }

		private Gost3410ParamSetParameters(
            Asn1Sequence seq)
        {
			if (seq.Count != 4)
				throw new ArgumentException("Wrong number of elements in sequence", "seq");

			this.keySize = DerInteger.GetInstance(seq[0]).Value.IntValue;
			this.p = DerInteger.GetInstance(seq[1]);
            this.q = DerInteger.GetInstance(seq[2]);
			this.a = DerInteger.GetInstance(seq[3]);
        }

		public int KeySize
		{
			get { return keySize; }
		}

		public BigInteger P
		{
			get { return p.PositiveValue; }
		}

		public BigInteger Q
		{
			get { return q.PositiveValue; }
		}

		public BigInteger A
		{
			get { return a.PositiveValue; }
		}

		public override Asn1Object ToAsn1Object()
        {
			return new DerSequence(new DerInteger(keySize), p, q, a);
        }
    }
}

#endif
