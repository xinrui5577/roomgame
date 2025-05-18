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
    public class CertificateStatusRequest
    {
        protected readonly byte mStatusType;
        protected readonly object mRequest;

        public CertificateStatusRequest(byte statusType, Object request)
        {
            if (!IsCorrectType(statusType, request))
                throw new ArgumentException("not an instance of the correct type", "request");

            this.mStatusType = statusType;
            this.mRequest = request;
        }

        public virtual byte StatusType
        {
            get { return mStatusType; }
        }

        public virtual object Request
        {
            get { return mRequest; }
        }

        public virtual OcspStatusRequest GetOcspStatusRequest()
        {
            if (!IsCorrectType(CertificateStatusType.ocsp, mRequest))
                throw new InvalidOperationException("'request' is not an OCSPStatusRequest");

            return (OcspStatusRequest)mRequest;
        }

        /**
         * Encode this {@link CertificateStatusRequest} to a {@link Stream}.
         * 
         * @param output
         *            the {@link Stream} to encode to.
         * @throws IOException
         */
        public virtual void Encode(Stream output)
        {
            TlsUtilities.WriteUint8(mStatusType, output);

            switch (mStatusType)
            {
            case CertificateStatusType.ocsp:
                ((OcspStatusRequest)mRequest).Encode(output);
                break;
            default:
                throw new TlsFatalAlert(AlertDescription.internal_error);
            }
        }

        /**
         * Parse a {@link CertificateStatusRequest} from a {@link Stream}.
         * 
         * @param input
         *            the {@link Stream} to parse from.
         * @return a {@link CertificateStatusRequest} object.
         * @throws IOException
         */
        public static CertificateStatusRequest Parse(Stream input)
        {
            byte status_type = TlsUtilities.ReadUint8(input);
            object result;

            switch (status_type)
            {
            case CertificateStatusType.ocsp:
                result = OcspStatusRequest.Parse(input);
                break;
            default:
                throw new TlsFatalAlert(AlertDescription.decode_error);
            }

            return new CertificateStatusRequest(status_type, result);
        }

        protected static bool IsCorrectType(byte statusType, object request)
        {
            switch (statusType)
            {
            case CertificateStatusType.ocsp:
                return request is OcspStatusRequest;
            default:
                throw new ArgumentException("unsupported value", "statusType");
            }
        }
    }
}

#endif
