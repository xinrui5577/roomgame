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
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.X509.Extension
{
	public class X509ExtensionUtilities
	{
		public static Asn1Object FromExtensionValue(
			Asn1OctetString extensionValue)
		{
			return Asn1Object.FromByteArray(extensionValue.GetOctets());
		}

		public static ICollection GetIssuerAlternativeNames(
			X509Certificate cert)
		{
			Asn1OctetString extVal = cert.GetExtensionValue(X509Extensions.IssuerAlternativeName);

			return GetAlternativeName(extVal);
		}

		public static ICollection GetSubjectAlternativeNames(
			X509Certificate cert)
		{
			Asn1OctetString extVal = cert.GetExtensionValue(X509Extensions.SubjectAlternativeName);

			return GetAlternativeName(extVal);
		}

		private static ICollection GetAlternativeName(
			Asn1OctetString extVal)
		{
			IList temp = Platform.CreateArrayList();

			if (extVal != null)
			{
				try
				{
					Asn1Sequence seq = DerSequence.GetInstance(FromExtensionValue(extVal));

					foreach (GeneralName genName in seq)
					{
                        IList list = Platform.CreateArrayList();
						list.Add(genName.TagNo);

						switch (genName.TagNo)
						{
							case GeneralName.EdiPartyName:
							case GeneralName.X400Address:
							case GeneralName.OtherName:
								list.Add(genName.Name.ToAsn1Object());
								break;
							case GeneralName.DirectoryName:
								list.Add(X509Name.GetInstance(genName.Name).ToString());
								break;
							case GeneralName.DnsName:
							case GeneralName.Rfc822Name:
							case GeneralName.UniformResourceIdentifier:
								list.Add(((IAsn1String)genName.Name).GetString());
								break;
							case GeneralName.RegisteredID:
								list.Add(DerObjectIdentifier.GetInstance(genName.Name).Id);
								break;
							case GeneralName.IPAddress:
								list.Add(DerOctetString.GetInstance(genName.Name).GetOctets());
								break;
							default:
								throw new IOException("Bad tag number: " + genName.TagNo);
						}

						temp.Add(list);
					}
				}
				catch (Exception e)
				{
					throw new CertificateParsingException(e.Message);
				}
			}

			return temp;
		}
	}
}

#endif
