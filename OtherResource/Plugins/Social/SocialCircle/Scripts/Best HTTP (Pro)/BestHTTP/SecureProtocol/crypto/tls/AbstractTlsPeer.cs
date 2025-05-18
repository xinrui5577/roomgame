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
    public abstract class AbstractTlsPeer
        :   TlsPeer
    {
        public virtual bool ShouldUseGmtUnixTime()
        {
            /*
             * draft-mathewson-no-gmtunixtime-00 2. For the reasons we discuss above, we recommend that
             * TLS implementors MUST by default set the entire value the ClientHello.Random and
             * ServerHello.Random fields, including gmt_unix_time, to a cryptographically random
             * sequence.
             */
            return false;
        }

        public virtual void NotifySecureRenegotiation(bool secureRenegotiation)
        {
            if (!secureRenegotiation)
            {
                /*
                 * RFC 5746 3.4/3.6. In this case, some clients/servers may want to terminate the handshake instead
                 * of continuing; see Section 4.1/4.3 for discussion.
                 */
                throw new TlsFatalAlert(AlertDescription.handshake_failure);
            }
        }

        public abstract TlsCompression GetCompression();

        public abstract TlsCipher GetCipher();

        public virtual void NotifyAlertRaised(byte alertLevel, byte alertDescription, string message, Exception cause)
        {
        }

        public virtual void NotifyAlertReceived(byte alertLevel, byte alertDescription)
        {
        }

        public virtual void NotifyHandshakeComplete()
        {
        }
    }
}

#endif
