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

using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Utilities.IO.Pem
{
	public class PemObject
		: PemObjectGenerator
	{
		private string		type;
		private IList		headers;
		private byte[]		content;

		public PemObject(string type, byte[] content)
			: this(type, Platform.CreateArrayList(), content)
		{
		}

		public PemObject(String type, IList headers, byte[] content)
		{
			this.type = type;
            this.headers = Platform.CreateArrayList(headers);
			this.content = content;
		}

		public string Type
		{
			get { return type; }
		}

		public IList Headers
		{
			get { return headers; }
		}

		public byte[] Content
		{
			get { return content; }
		}

		public PemObject Generate()
		{
			return this;
		}
	}
}

#endif
