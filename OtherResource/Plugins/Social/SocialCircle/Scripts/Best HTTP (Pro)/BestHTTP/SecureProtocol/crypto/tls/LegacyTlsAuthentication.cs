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

namespace Org.BouncyCastle.Crypto.Tls
{
    public interface IClientCredentialsProvider
    {
        TlsCredentials GetClientCredentials(TlsContext context, CertificateRequest certificateRequest);
    }

	/// <summary>
	/// A temporary class to wrap old CertificateVerifyer stuff for new TlsAuthentication.
	/// </summary>
	public class LegacyTlsAuthentication : TlsAuthentication
	{
		protected ICertificateVerifyer verifyer;
        protected IClientCredentialsProvider credProvider;
        protected Uri TargetUri;

		public LegacyTlsAuthentication(Uri targetUri, ICertificateVerifyer verifyer, IClientCredentialsProvider prov)
		{
            this.TargetUri = targetUri;
			this.verifyer = verifyer;
            this.credProvider = prov;
		}

		public virtual void NotifyServerCertificate(Certificate serverCertificate)
		{
			if (!this.verifyer.IsValid(this.TargetUri, serverCertificate.GetCertificateList()))
				throw new TlsFatalAlert(AlertDescription.user_canceled);
		}

		public virtual TlsCredentials GetClientCredentials(TlsContext context, CertificateRequest certificateRequest)
		{
			return credProvider == null ? null : credProvider.GetClientCredentials(context, certificateRequest);
		}
	}
}

#endif
