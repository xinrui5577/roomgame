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
using System.Text;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Signers
{
	public class DsaDigestSigner
		: ISigner
	{
		private readonly IDigest digest;
		private readonly IDsa dsaSigner;
		private bool forSigning;

		public DsaDigestSigner(
			IDsa	signer,
			IDigest	digest)
		{
			this.digest = digest;
			this.dsaSigner = signer;
		}

		public virtual string AlgorithmName
		{
			get { return digest.AlgorithmName + "with" + dsaSigner.AlgorithmName; }
		}

        public virtual void Init(
			bool							forSigning,
			ICipherParameters	parameters)
		{
			this.forSigning = forSigning;

			AsymmetricKeyParameter k;

			if (parameters is ParametersWithRandom)
			{
				k = (AsymmetricKeyParameter)((ParametersWithRandom)parameters).Parameters;
			}
			else
			{
				k = (AsymmetricKeyParameter)parameters;
			}

			if (forSigning && !k.IsPrivate)
				throw new InvalidKeyException("Signing Requires Private Key.");

			if (!forSigning && k.IsPrivate)
				throw new InvalidKeyException("Verification Requires Public Key.");

			Reset();

			dsaSigner.Init(forSigning, parameters);
		}

		/**
		 * update the internal digest with the byte b
		 */
        public virtual void Update(
			byte input)
		{
			digest.Update(input);
		}

		/**
		 * update the internal digest with the byte array in
		 */
        public virtual void BlockUpdate(
			byte[]	input,
			int			inOff,
			int			length)
		{
			digest.BlockUpdate(input, inOff, length);
		}

		/**
		 * Generate a signature for the message we've been loaded with using
		 * the key we were initialised with.
     */
        public virtual byte[] GenerateSignature()
		{
			if (!forSigning)
				throw new InvalidOperationException("DSADigestSigner not initialised for signature generation.");

			byte[] hash = new byte[digest.GetDigestSize()];
			digest.DoFinal(hash, 0);

			BigInteger[] sig = dsaSigner.GenerateSignature(hash);

			return DerEncode(sig[0], sig[1]);
		}

		/// <returns>true if the internal state represents the signature described in the passed in array.</returns>
        public virtual bool VerifySignature(
			byte[] signature)
		{
			if (forSigning)
				throw new InvalidOperationException("DSADigestSigner not initialised for verification");

			byte[] hash = new byte[digest.GetDigestSize()];
			digest.DoFinal(hash, 0);

			try
			{
				BigInteger[] sig = DerDecode(signature);
				return dsaSigner.VerifySignature(hash, sig[0], sig[1]);
			}
			catch (IOException)
			{
				return false;
			}
		}

		/// <summary>Reset the internal state</summary>
        public virtual void Reset()
		{
			digest.Reset();
		}

		private byte[] DerEncode(
			BigInteger	r,
			BigInteger	s)
		{
			return new DerSequence(new DerInteger(r), new DerInteger(s)).GetDerEncoded();
		}

		private BigInteger[] DerDecode(
			byte[] encoding)
		{
			Asn1Sequence s = (Asn1Sequence) Asn1Object.FromByteArray(encoding);

			return new BigInteger[]
			{
				((DerInteger) s[0]).Value,
				((DerInteger) s[1]).Value
			};
		}
	}
}

#endif
