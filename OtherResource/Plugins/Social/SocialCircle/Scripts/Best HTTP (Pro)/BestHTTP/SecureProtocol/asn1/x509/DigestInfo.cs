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

namespace Org.BouncyCastle.Asn1.X509
{
    /**
     * The DigestInfo object.
     * <pre>
     * DigestInfo::=Sequence{
     *          digestAlgorithm  AlgorithmIdentifier,
     *          digest OCTET STRING }
     * </pre>
     */
    public class DigestInfo
        : Asn1Encodable
    {
        private readonly byte[] digest;
        private readonly AlgorithmIdentifier algID;

		public static DigestInfo GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
        }

		public static DigestInfo GetInstance(
            object obj)
        {
            if (obj is DigestInfo)
            {
                return (DigestInfo) obj;
            }

			if (obj is Asn1Sequence)
            {
                return new DigestInfo((Asn1Sequence) obj);
            }

			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public DigestInfo(
            AlgorithmIdentifier	algID,
            byte[]				digest)
        {
            this.digest = digest;
            this.algID = algID;
        }

		private DigestInfo(
            Asn1Sequence seq)
        {
			if (seq.Count != 2)
				throw new ArgumentException("Wrong number of elements in sequence", "seq");

            algID = AlgorithmIdentifier.GetInstance(seq[0]);
			digest = Asn1OctetString.GetInstance(seq[1]).GetOctets();
		}

		public AlgorithmIdentifier AlgorithmID
		{
			get { return algID; }
		}

		public byte[] GetDigest()
        {
            return digest;
        }

		public override Asn1Object ToAsn1Object()
        {
			return new DerSequence(algID, new DerOctetString(digest));
        }
    }
}

#endif
