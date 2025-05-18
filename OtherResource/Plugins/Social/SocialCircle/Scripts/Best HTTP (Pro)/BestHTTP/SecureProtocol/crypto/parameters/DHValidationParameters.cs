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

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Parameters
{
    public class DHValidationParameters
    {
        private readonly byte[] seed;
        private readonly int counter;

		public DHValidationParameters(
            byte[]	seed,
            int		counter)
        {
			if (seed == null)
				throw new ArgumentNullException("seed");

			this.seed = (byte[]) seed.Clone();
            this.counter = counter;
        }

		public byte[] GetSeed()
        {
            return (byte[]) seed.Clone();
        }

		public int Counter
        {
            get { return counter; }
        }

		public override bool Equals(
            object obj)
        {
			if (obj == this)
				return true;

			DHValidationParameters other = obj as DHValidationParameters;

			if (other == null)
				return false;

			return Equals(other);
		}

		protected bool Equals(
			DHValidationParameters other)
		{
			return counter == other.counter
				&& Arrays.AreEqual(this.seed, other.seed);
		}

		public override int GetHashCode()
        {
			return counter.GetHashCode() ^ Arrays.GetHashCode(seed);
		}
    }
}

#endif
