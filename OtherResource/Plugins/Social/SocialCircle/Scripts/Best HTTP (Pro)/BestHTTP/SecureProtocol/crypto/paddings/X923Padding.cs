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
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Paddings
{
    /**
    * A padder that adds X9.23 padding to a block - if a SecureRandom is
    * passed in random padding is assumed, otherwise padding with zeros is used.
    */
    public class X923Padding
		: IBlockCipherPadding
    {
        private SecureRandom random;

		/**
        * Initialise the padder.
        *
        * @param random a SecureRandom if one is available.
        */
        public void Init(
			SecureRandom random)
        {
            this.random = random;
        }

		/**
        * Return the name of the algorithm the cipher implements.
        *
        * @return the name of the algorithm the cipher implements.
        */
        public string PaddingName
        {
            get { return "X9.23"; }
        }

		/**
        * add the pad bytes to the passed in block, returning the
        * number of bytes added.
        */
        public int AddPadding(
            byte[]  input,
            int     inOff)
        {
            byte code = (byte)(input.Length - inOff);

            while (inOff < input.Length - 1)
            {
                if (random == null)
                {
                    input[inOff] = 0;
                }
                else
                {
                    input[inOff] = (byte)random.NextInt();
                }
                inOff++;
            }

            input[inOff] = code;

            return code;
        }

        /**
        * return the number of pad bytes present in the block.
        */
        public int PadCount(
			byte[] input)
        {
            int count = input[input.Length - 1] & 0xff;

            if (count > input.Length)
            {
                throw new InvalidCipherTextException("pad block corrupted");
            }

            return count;
        }
    }
}

#endif
