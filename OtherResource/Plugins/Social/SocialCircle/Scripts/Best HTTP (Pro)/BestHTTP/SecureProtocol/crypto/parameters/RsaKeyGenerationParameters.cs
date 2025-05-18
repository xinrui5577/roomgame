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

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Parameters
{
    public class RsaKeyGenerationParameters
		: KeyGenerationParameters
    {
        private readonly BigInteger publicExponent;
        private readonly int certainty;

		public RsaKeyGenerationParameters(
            BigInteger		publicExponent,
            SecureRandom	random,
            int				strength,
            int				certainty)
			: base(random, strength)
        {
            this.publicExponent = publicExponent;
            this.certainty = certainty;
        }

		public BigInteger PublicExponent
        {
			get { return publicExponent; }
        }

		public int Certainty
        {
			get { return certainty; }
        }

		public override bool Equals(
			object obj)
		{
			RsaKeyGenerationParameters other = obj as RsaKeyGenerationParameters;

			if (other == null)
			{
				return false;
			}

			return certainty == other.certainty
				&& publicExponent.Equals(other.publicExponent);
		}

		public override int GetHashCode()
		{
			return certainty.GetHashCode() ^ publicExponent.GetHashCode();
		}
    }
}

#endif
