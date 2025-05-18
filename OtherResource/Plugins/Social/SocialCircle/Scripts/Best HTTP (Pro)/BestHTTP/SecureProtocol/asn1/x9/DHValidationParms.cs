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

namespace Org.BouncyCastle.Asn1.X9
{
	public class DHValidationParms
		: Asn1Encodable
	{
		private readonly DerBitString seed;
		private readonly DerInteger pgenCounter;

		public static DHValidationParms GetInstance(Asn1TaggedObject obj, bool isExplicit)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, isExplicit));
		}

		public static DHValidationParms GetInstance(object obj)
		{
			if (obj == null || obj is DHDomainParameters)
				return (DHValidationParms)obj;

			if (obj is Asn1Sequence)
				return new DHValidationParms((Asn1Sequence)obj);

			throw new ArgumentException("Invalid DHValidationParms: " + obj.GetType().FullName, "obj");
		}
		
		public DHValidationParms(DerBitString seed, DerInteger pgenCounter)
		{
			if (seed == null)
				throw new ArgumentNullException("seed");
			if (pgenCounter == null)
				throw new ArgumentNullException("pgenCounter");

			this.seed = seed;
			this.pgenCounter = pgenCounter;
		}

		private DHValidationParms(Asn1Sequence seq)
		{
			if (seq.Count != 2)
				throw new ArgumentException("Bad sequence size: " + seq.Count, "seq");

			this.seed = DerBitString.GetInstance(seq[0]);
			this.pgenCounter = DerInteger.GetInstance(seq[1]);
		}

		public DerBitString Seed
		{
			get { return this.seed; }
		}

		public DerInteger PgenCounter
		{
			get { return this.pgenCounter; }
		}

		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(seed, pgenCounter);
		}
	}
}

#endif
