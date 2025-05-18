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

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Crypto.Modes
{
    /**
    * Implements the Segmented Integer Counter (SIC) mode on top of a simple
    * block cipher.
    */
    public class SicBlockCipher
        : IBlockCipher
    {
        private readonly IBlockCipher cipher;
        private readonly int blockSize;
        private readonly byte[] IV;
        private readonly byte[] counter;
        private readonly byte[] counterOut;

        /**
        * Basic constructor.
        *
        * @param c the block cipher to be used.
        */
        public SicBlockCipher(IBlockCipher cipher)
        {
            this.cipher = cipher;
            this.blockSize = cipher.GetBlockSize();
            this.IV = new byte[blockSize];
            this.counter = new byte[blockSize];
            this.counterOut = new byte[blockSize];
        }

        /**
        * return the underlying block cipher that we are wrapping.
        *
        * @return the underlying block cipher that we are wrapping.
        */
        public IBlockCipher GetUnderlyingCipher()
        {
            return cipher;
        }

        public void Init(
            bool				forEncryption, //ignored by this CTR mode
            ICipherParameters	parameters)
        {
            if (parameters is ParametersWithIV)
            {
                ParametersWithIV ivParam = (ParametersWithIV) parameters;
                byte[] iv = ivParam.GetIV();
                Array.Copy(iv, 0, IV, 0, IV.Length);

                Reset();

                // if null it's an IV changed only.
                if (ivParam.Parameters != null)
                {
                    cipher.Init(true, ivParam.Parameters);
                }
            }
            else
            {
                throw new ArgumentException("SIC mode requires ParametersWithIV", "parameters");
            }
        }

        public string AlgorithmName
        {
            get { return cipher.AlgorithmName + "/SIC"; }
        }

        public bool IsPartialBlockOkay
        {
            get { return true; }
        }

        public int GetBlockSize()
        {
            return cipher.GetBlockSize();
        }

        public int ProcessBlock(
            byte[]	input,
            int		inOff,
            byte[]	output,
            int		outOff)
        {
            cipher.ProcessBlock(counter, 0, counterOut, 0);

            //
            // XOR the counterOut with the plaintext producing the cipher text
            //
            for (int i = 0; i < counterOut.Length; i++)
            {
                output[outOff + i] = (byte)(counterOut[i] ^ input[inOff + i]);
            }

            // Increment the counter
            int j = counter.Length;
            while (--j >= 0 && ++counter[j] == 0)
            {
            }

            return counter.Length;
        }

        public void Reset()
        {
            Array.Copy(IV, 0, counter, 0, counter.Length);
            cipher.Reset();
        }
    }
}

#endif
