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

using Org.BouncyCastle.Crypto.Digests;

namespace Org.BouncyCastle.Crypto.Prng
{
	/**
	 * Random generation based on the digest with counter. Calling AddSeedMaterial will
	 * always increase the entropy of the hash.
	 * <p>
	 * Internal access to the digest is synchronized so a single one of these can be shared.
	 * </p>
	 */
	public class DigestRandomGenerator
		: IRandomGenerator
	{
		private const long CYCLE_COUNT = 10;

		private long	stateCounter;
		private long	seedCounter;
		private IDigest	digest;
		private byte[]	state;
		private byte[]	seed;

		public DigestRandomGenerator(
			IDigest digest)
		{
			this.digest = digest;

			this.seed = new byte[digest.GetDigestSize()];
			this.seedCounter = 1;

			this.state = new byte[digest.GetDigestSize()];
			this.stateCounter = 1;
		}

		public void AddSeedMaterial(
			byte[] inSeed)
		{
			lock (this)
			{
				DigestUpdate(inSeed);
				DigestUpdate(seed);
				DigestDoFinal(seed);
			}
		}

		public void AddSeedMaterial(
			long rSeed)
		{
			lock (this)
			{
				DigestAddCounter(rSeed);
				DigestUpdate(seed);
				DigestDoFinal(seed);
			}
		}

		public void NextBytes(
			byte[] bytes)
		{
			NextBytes(bytes, 0, bytes.Length);
		}

		public void NextBytes(
			byte[]	bytes,
			int		start,
			int		len)
		{
			lock (this)
			{
				int stateOff = 0;

				GenerateState();

				int end = start + len;
				for (int i = start; i < end; ++i)
				{
					if (stateOff == state.Length)
					{
						GenerateState();
						stateOff = 0;
					}
					bytes[i] = state[stateOff++];
				}
			}
		}

		private void CycleSeed()
		{
			DigestUpdate(seed);
			DigestAddCounter(seedCounter++);
			DigestDoFinal(seed);
		}

		private void GenerateState()
		{
			DigestAddCounter(stateCounter++);
			DigestUpdate(state);
			DigestUpdate(seed);
			DigestDoFinal(state);

			if ((stateCounter % CYCLE_COUNT) == 0)
			{
				CycleSeed();
			}
		}

		private void DigestAddCounter(long seedVal)
		{
			ulong seed = (ulong)seedVal;
			for (int i = 0; i != 8; i++)
			{
				digest.Update((byte)seed);
				seed >>= 8;
			}
		}

		private void DigestUpdate(byte[] inSeed)
		{
			digest.BlockUpdate(inSeed, 0, inSeed.Length);
		}

		private void DigestDoFinal(byte[] result)
		{
			digest.DoFinal(result, 0);
		}
	}
}

#endif
