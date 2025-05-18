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

using Org.BouncyCastle.Asn1.X509;

namespace Org.BouncyCastle.Crypto.Tls
{
	/// <remarks>
	/// A certificate verifyer, that will always return true.
	/// <pre>
	/// DO NOT USE THIS FILE UNLESS YOU KNOW EXACTLY WHAT YOU ARE DOING.
	/// </pre>
	/// </remarks>
	//[Obsolete("Perform certificate verification in TlsAuthentication implementation")]
	public class AlwaysValidVerifyer : ICertificateVerifyer
	{
		/// <summary>Return true.</summary>
		public bool IsValid(Uri targetUri, X509CertificateStructure[] certs)
		{
			return true;
		}
	}
}

#endif
