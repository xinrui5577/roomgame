/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

namespace Org.BouncyCastle.Crypto.Engines
{
	/// <remarks>
	/// An implementation of the Camellia key wrapper based on RFC 3657/RFC 3394.
	/// <p/>
	/// For further details see: <a href="http://www.ietf.org/rfc/rfc3657.txt">http://www.ietf.org/rfc/rfc3657.txt</a>.
	/// </remarks>
	public class CamelliaWrapEngine
		: Rfc3394WrapEngine
	{
		public CamelliaWrapEngine()
			: base(new CamelliaEngine())
		{
		}
	}
}

#endif
