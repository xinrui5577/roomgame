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
using System.Collections;

using Org.BouncyCastle.Asn1;

namespace Org.BouncyCastle.Asn1.X509
{
    /**
     * PKIX RFC-2459
     *
     * The X.509 v2 CRL syntax is as follows.  For signature calculation,
     * the data that is to be signed is ASN.1 Der encoded.
     *
     * <pre>
     * CertificateList  ::=  Sequence  {
     *      tbsCertList          TbsCertList,
     *      signatureAlgorithm   AlgorithmIdentifier,
     *      signatureValue       BIT STRING  }
     * </pre>
     */
    public class CertificateList
        : Asn1Encodable
    {
        private readonly TbsCertificateList	tbsCertList;
        private readonly AlgorithmIdentifier sigAlgID;
        private readonly DerBitString sig;

		public static CertificateList GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
        }

		public static CertificateList GetInstance(
            object obj)
        {
            if (obj is CertificateList)
                return (CertificateList) obj;

			if (obj != null)
				return new CertificateList(Asn1Sequence.GetInstance(obj));

			return null;
		}

		private CertificateList(
            Asn1Sequence seq)
        {
			if (seq.Count != 3)
				throw new ArgumentException("sequence wrong size for CertificateList", "seq");

			tbsCertList = TbsCertificateList.GetInstance(seq[0]);
			sigAlgID = AlgorithmIdentifier.GetInstance(seq[1]);
			sig = DerBitString.GetInstance(seq[2]);
        }

		public TbsCertificateList TbsCertList
		{
			get { return tbsCertList; }
		}

		public CrlEntry[] GetRevokedCertificates()
        {
            return tbsCertList.GetRevokedCertificates();
        }

		public IEnumerable GetRevokedCertificateEnumeration()
		{
			return tbsCertList.GetRevokedCertificateEnumeration();
		}

		public AlgorithmIdentifier SignatureAlgorithm
		{
			get { return sigAlgID; }
		}

		public DerBitString Signature
		{
			get { return sig; }
		}

		public int Version
		{
			get { return tbsCertList.Version; }
		}

		public X509Name Issuer
		{
			get { return tbsCertList.Issuer; }
		}

		public Time ThisUpdate
		{
			get { return tbsCertList.ThisUpdate; }
		}

		public Time NextUpdate
		{
			get { return tbsCertList.NextUpdate; }
		}

		public override Asn1Object ToAsn1Object()
        {
			return new DerSequence(tbsCertList, sigAlgID, sig);
        }
    }
}

#endif
