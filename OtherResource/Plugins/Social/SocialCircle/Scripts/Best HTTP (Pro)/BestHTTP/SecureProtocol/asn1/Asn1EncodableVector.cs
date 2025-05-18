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

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1
{
    public class Asn1EncodableVector
		: IEnumerable
    {
        private IList v = Platform.CreateArrayList();

		public static Asn1EncodableVector FromEnumerable(
			IEnumerable e)
		{
			Asn1EncodableVector v = new Asn1EncodableVector();
			foreach (Asn1Encodable obj in e)
			{
				v.Add(obj);
			}
			return v;
		}

//		public Asn1EncodableVector()
//		{
//		}

		public Asn1EncodableVector(
			params Asn1Encodable[] v)
		{
			Add(v);
		}

//		public void Add(
//			Asn1Encodable obj)
//		{
//			v.Add(obj);
//		}

		public void Add(
			params Asn1Encodable[] objs)
		{
			foreach (Asn1Encodable obj in objs)
			{
				v.Add(obj);
			}
		}

		public void AddOptional(
			params Asn1Encodable[] objs)
		{
			if (objs != null)
			{
				foreach (Asn1Encodable obj in objs)
				{
					if (obj != null)
					{
						v.Add(obj);
					}
				}
			}
		}

		public Asn1Encodable this[
			int index]
		{
			get { return (Asn1Encodable) v[index]; }
		}

		[Obsolete("Use 'object[index]' syntax instead")]
		public Asn1Encodable Get(
            int index)
        {
            return this[index];
        }

		[Obsolete("Use 'Count' property instead")]
		public int Size
		{
			get { return v.Count; }
		}

		public int Count
		{
			get { return v.Count; }
		}

		public IEnumerator GetEnumerator()
		{
			return v.GetEnumerator();
		}
	}
}

#endif
