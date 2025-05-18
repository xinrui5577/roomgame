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

using Org.BouncyCastle.Asn1;

namespace Org.BouncyCastle.Asn1.Ocsp
{
    public class OcspResponse
        : Asn1Encodable
    {
        private readonly OcspResponseStatus	responseStatus;
        private readonly ResponseBytes		responseBytes;

		public static OcspResponse GetInstance(
			Asn1TaggedObject	obj,
			bool				explicitly)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static OcspResponse GetInstance(
			object obj)
		{
			if (obj == null || obj is OcspResponse)
			{
				return (OcspResponse)obj;
			}

			if (obj is Asn1Sequence)
			{
				return new OcspResponse((Asn1Sequence)obj);
			}

			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public OcspResponse(
            OcspResponseStatus	responseStatus,
            ResponseBytes		responseBytes)
        {
			if (responseStatus == null)
				throw new ArgumentNullException("responseStatus");

			this.responseStatus = responseStatus;
            this.responseBytes = responseBytes;
        }

		private OcspResponse(
            Asn1Sequence seq)
        {
            responseStatus = new OcspResponseStatus(
				DerEnumerated.GetInstance(seq[0]));

			if (seq.Count == 2)
            {
                responseBytes = ResponseBytes.GetInstance(
					(Asn1TaggedObject)seq[1], true);
            }
        }

		public OcspResponseStatus ResponseStatus
		{
			get { return responseStatus; }
		}

		public ResponseBytes ResponseBytes
		{
			get { return responseBytes; }
		}

		/**
         * Produce an object suitable for an Asn1OutputStream.
         * <pre>
         * OcspResponse ::= Sequence {
         *     responseStatus         OcspResponseStatus,
         *     responseBytes          [0] EXPLICIT ResponseBytes OPTIONAL }
         * </pre>
         */
        public override Asn1Object ToAsn1Object()
        {
            Asn1EncodableVector v = new Asn1EncodableVector(responseStatus);

			if (responseBytes != null)
            {
                v.Add(new DerTaggedObject(true, 0, responseBytes));
            }

			return new DerSequence(v);
        }
    }
}

#endif
