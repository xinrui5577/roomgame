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
	/// This should be implemented by any class which can find out, if a given
	/// certificate chain is being accepted by an client.
	/// </remarks>
	//[Obsolete("Perform certificate verification in TlsAuthentication implementation")]
	public interface ICertificateVerifyer
	{
		/// <param name="certs">The certs, which are part of the chain.</param>
        /// <param name="targetUri"></param>
		/// <returns>True, if the chain is accepted, false otherwise</returns>
		bool IsValid(Uri targetUri, X509CertificateStructure[] certs);
	}
}

#endif
