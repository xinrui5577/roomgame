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
using System.Diagnostics;

namespace Org.BouncyCastle.Asn1
{
	internal class LazyDerSet
		: DerSet
	{
		private byte[] encoded;

        internal LazyDerSet(
			byte[] encoded)
		{
			this.encoded = encoded;
		}

		private void Parse()
		{
			lock (this)
			{
				if (encoded != null)
				{
					Asn1InputStream e = new LazyAsn1InputStream(encoded);

					Asn1Object o;
					while ((o = e.ReadObject()) != null)
					{
						AddObject(o);
					}

					encoded = null;
				}
			}
		}

		public override Asn1Encodable this[int index]
		{
			get
			{
				Parse();

				return base[index];
			}
		}

		public override IEnumerator GetEnumerator()
		{
			Parse();

			return base.GetEnumerator();
		}

		public override int Count
		{
			get
			{
				Parse();

				return base.Count;
			}
		}

		internal override void Encode(
			DerOutputStream derOut)
		{
			lock (this)
			{
				if (encoded == null)
				{
					base.Encode(derOut);
				}
				else
				{
					derOut.WriteEncoded(Asn1Tags.Set | Asn1Tags.Constructed, encoded);
				}
			}
		}
	}
}

#endif
