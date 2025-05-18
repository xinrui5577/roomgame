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
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto
{
    /**
     * The base class for parameters to key generators.
     */
    public class KeyGenerationParameters
    {
        private SecureRandom	random;
        private int				strength;

        /**
         * initialise the generator with a source of randomness
         * and a strength (in bits).
         *
         * @param random the random byte source.
         * @param strength the size, in bits, of the keys we want to produce.
         */
        public KeyGenerationParameters(
            SecureRandom	random,
            int				strength)
        {
			if (random == null)
				throw new ArgumentNullException("random");
			if (strength < 1)
				throw new ArgumentException("strength must be a positive value", "strength");

			this.random = random;
            this.strength = strength;
        }

		/**
         * return the random source associated with this
         * generator.
         *
         * @return the generators random source.
         */
        public SecureRandom Random
        {
            get { return random; }
        }

		/**
         * return the bit strength for keys produced by this generator,
         *
         * @return the strength of the keys this generator produces (in bits).
         */
        public int Strength
        {
            get { return strength; }
        }
    }
}

#endif
