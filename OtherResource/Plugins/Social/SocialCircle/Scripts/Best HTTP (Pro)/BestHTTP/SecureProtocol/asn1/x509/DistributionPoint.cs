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
using System.Text;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.X509
{
    /**
     * The DistributionPoint object.
     * <pre>
     * DistributionPoint ::= Sequence {
     *      distributionPoint [0] DistributionPointName OPTIONAL,
     *      reasons           [1] ReasonFlags OPTIONAL,
     *      cRLIssuer         [2] GeneralNames OPTIONAL
     * }
     * </pre>
     */
    public class DistributionPoint
        : Asn1Encodable
    {
        internal readonly DistributionPointName	distributionPoint;
        internal readonly ReasonFlags			reasons;
        internal readonly GeneralNames			cRLIssuer;

		public static DistributionPoint GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
        }

		public static DistributionPoint GetInstance(
            object obj)
        {
            if(obj == null || obj is DistributionPoint)
            {
                return (DistributionPoint) obj;
            }

			if(obj is Asn1Sequence)
            {
                return new DistributionPoint((Asn1Sequence) obj);
            }

			throw new ArgumentException("Invalid DistributionPoint: " + obj.GetType().Name);
        }

		private DistributionPoint(
            Asn1Sequence seq)
        {
            for (int i = 0; i != seq.Count; i++)
            {
				Asn1TaggedObject t = Asn1TaggedObject.GetInstance(seq[i]);

				switch (t.TagNo)
                {
                case 0:
                    distributionPoint = DistributionPointName.GetInstance(t, true);
                    break;
                case 1:
                    reasons = new ReasonFlags(DerBitString.GetInstance(t, false));
                    break;
                case 2:
                    cRLIssuer = GeneralNames.GetInstance(t, false);
                    break;
                }
            }
        }

		public DistributionPoint(
            DistributionPointName	distributionPointName,
            ReasonFlags				reasons,
            GeneralNames			crlIssuer)
        {
            this.distributionPoint = distributionPointName;
            this.reasons = reasons;
            this.cRLIssuer = crlIssuer;
        }

		public DistributionPointName DistributionPointName
        {
			get { return distributionPoint; }
        }

		public ReasonFlags Reasons
        {
			get { return reasons; }
        }

		public GeneralNames CrlIssuer
        {
			get { return cRLIssuer; }
        }

		public override Asn1Object ToAsn1Object()
        {
            Asn1EncodableVector v = new Asn1EncodableVector();

			if (distributionPoint != null)
            {
                //
                // as this is a CHOICE it must be explicitly tagged
                //
                v.Add(new DerTaggedObject(0, distributionPoint));
            }

			if (reasons != null)
            {
                v.Add(new DerTaggedObject(false, 1, reasons));
            }

			if (cRLIssuer != null)
            {
                v.Add(new DerTaggedObject(false, 2, cRLIssuer));
            }

			return new DerSequence(v);
        }

		public override string ToString()
		{
			string sep = Platform.NewLine;
			StringBuilder buf = new StringBuilder();
			buf.Append("DistributionPoint: [");
			buf.Append(sep);
			if (distributionPoint != null)
			{
				appendObject(buf, sep, "distributionPoint", distributionPoint.ToString());
			}
			if (reasons != null)
			{
				appendObject(buf, sep, "reasons", reasons.ToString());
			}
			if (cRLIssuer != null)
			{
				appendObject(buf, sep, "cRLIssuer", cRLIssuer.ToString());
			}
			buf.Append("]");
			buf.Append(sep);
			return buf.ToString();
		}

		private void appendObject(
			StringBuilder	buf,
			string			sep,
			string			name,
			string			val)
		{
			string indent = "    ";

			buf.Append(indent);
			buf.Append(name);
			buf.Append(":");
			buf.Append(sep);
			buf.Append(indent);
			buf.Append(indent);
			buf.Append(val);
			buf.Append(sep);
		}
	}
}

#endif
