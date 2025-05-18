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
    /// <summary>
    /// A generic interface for key exchange implementations in (D)TLS.
    /// </summary>
    public interface TlsKeyExchange
    {
        void Init(TlsContext context);

        /// <exception cref="IOException"/>
        void SkipServerCredentials();

        /// <exception cref="IOException"/>
        void ProcessServerCredentials(TlsCredentials serverCredentials);

        /// <exception cref="IOException"/>
        void ProcessServerCertificate(Certificate serverCertificate);

        bool RequiresServerKeyExchange { get; }

        /// <exception cref="IOException"/>
        byte[] GenerateServerKeyExchange();

        /// <exception cref="IOException"/>
        void SkipServerKeyExchange();

        /// <exception cref="IOException"/>
        void ProcessServerKeyExchange(Stream input);

        /// <exception cref="IOException"/>
        void ValidateCertificateRequest(CertificateRequest certificateRequest);

        /// <exception cref="IOException"/>
        void SkipClientCredentials();

        /// <exception cref="IOException"/>
        void ProcessClientCredentials(TlsCredentials clientCredentials);

        /// <exception cref="IOException"/>
        void ProcessClientCertificate(Certificate clientCertificate);

        /// <exception cref="IOException"/>
        void GenerateClientKeyExchange(Stream output);

        /// <exception cref="IOException"/>
        void ProcessClientKeyExchange(Stream input);

        /// <exception cref="IOException"/>
        byte[] GeneratePremasterSecret();
    }
}

#endif
