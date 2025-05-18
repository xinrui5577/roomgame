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

namespace Org.BouncyCastle.Utilities.Collections
{
	public sealed class EmptyEnumerable
		: IEnumerable
	{
		public static readonly IEnumerable Instance = new EmptyEnumerable();

		private EmptyEnumerable()
		{
		}

		public IEnumerator GetEnumerator()
		{
			return EmptyEnumerator.Instance;
		}
	}

	public sealed class EmptyEnumerator
		: IEnumerator
	{
		public static readonly IEnumerator Instance = new EmptyEnumerator();

		private EmptyEnumerator()
		{
		}

		public bool MoveNext()
		{
			return false;
		}

		public void Reset()
		{
		}

		public object Current
		{
			get { throw new InvalidOperationException("No elements"); }
		}
	}
}

#endif
