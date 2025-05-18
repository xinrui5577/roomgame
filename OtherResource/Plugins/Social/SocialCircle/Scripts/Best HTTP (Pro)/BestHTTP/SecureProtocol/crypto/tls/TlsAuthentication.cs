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
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
	public interface TlsAuthentication
	{
		/// <summary>
		/// Called by the protocol handler to report the server certificate.
		/// </summary>
		/// <remarks>
		/// This method is responsible for certificate verification and validation
		/// </remarks>
		/// <param name="serverCertificate">The server <see cref="Certificate"/> received</param>
		/// <exception cref="IOException"></exception>
		void NotifyServerCertificate(Certificate serverCertificate);

		/// <summary>
		/// Return client credentials in response to server's certificate request
		/// </summary>
		/// <param name="certificateRequest">
		/// A <see cref="CertificateRequest"/> containing server certificate request details
		/// </param>
		/// <returns>
		/// A <see cref="TlsCredentials"/> to be used for client authentication
		/// (or <c>null</c> for no client authentication)
		/// </returns>
		/// <exception cref="IOException"></exception>
		TlsCredentials GetClientCredentials(TlsContext context, CertificateRequest certificateRequest);
	}
}

#endif
