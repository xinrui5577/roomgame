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

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto.Parameters
{
	public class Gost3410Parameters
		: ICipherParameters
	{
		private readonly BigInteger p, q, a;
		private readonly Gost3410ValidationParameters validation;

		public Gost3410Parameters(
			BigInteger	p,
			BigInteger	q,
			BigInteger	a)
			: this(p, q, a, null)
		{
		}

		public Gost3410Parameters(
			BigInteger						p,
			BigInteger						q,
			BigInteger						a,
			Gost3410ValidationParameters	validation)
		{
			if (p == null)
				throw new ArgumentNullException("p");
			if (q == null)
				throw new ArgumentNullException("q");
			if (a == null)
				throw new ArgumentNullException("a");

			this.p = p;
			this.q = q;
			this.a = a;
			this.validation = validation;
		}

		public BigInteger P
		{
			get { return p; }
		}

		public BigInteger Q
		{
			get { return q; }
		}

		public BigInteger A
		{
			get { return a; }
		}

		public Gost3410ValidationParameters ValidationParameters
		{
			get { return validation; }
		}

		public override bool Equals(
			object obj)
		{
			if (obj == this)
				return true;

			Gost3410Parameters other = obj as Gost3410Parameters;

			if (other == null)
				return false;

			return Equals(other);
		}

		protected bool Equals(
			Gost3410Parameters other)
		{
			return p.Equals(other.p) && q.Equals(other.q) && a.Equals(other.a);
		}

		public override int GetHashCode()
		{
			return p.GetHashCode() ^ q.GetHashCode() ^ a.GetHashCode();
		}
	}
}

#endif
