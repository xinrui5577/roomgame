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
	/// <summary>
	/// A temporary class to use LegacyTlsAuthentication 
	/// </summary>
	public sealed class LegacyTlsClient : DefaultTlsClient
	{
        private readonly Uri TargetUri;
        private readonly ICertificateVerifyer verifyer;
        private readonly IClientCredentialsProvider credProvider;

        public LegacyTlsClient(Uri targetUri, ICertificateVerifyer verifyer, IClientCredentialsProvider prov, System.Collections.Generic.List<string> hostNames)
		{
            this.TargetUri = targetUri;
			this.verifyer = verifyer;
            this.credProvider = prov;
            this.HostNames = hostNames;
		}

		public override TlsAuthentication GetAuthentication()
		{
			return new LegacyTlsAuthentication(this.TargetUri, verifyer, credProvider);
		}
	}
}

#endif
