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

namespace Org.BouncyCastle.Crypto
{
	/// <summary>
	/// This exception is thrown whenever a cipher requires a change of key, iv
	/// or similar after x amount of bytes enciphered
    /// </summary>
#if !(NETCF_1_0 || NETCF_2_0 || SILVERLIGHT || NETFX_CORE)
    [Serializable]
#endif
    public class MaxBytesExceededException
		: CryptoException
	{
		public MaxBytesExceededException()
		{
		}

		public MaxBytesExceededException(
			string message)
			: base(message)
		{
		}

		public MaxBytesExceededException(
			string		message,
			Exception	e)
			: base(message, e)
		{
		}
	}
}

#endif
