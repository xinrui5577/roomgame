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

using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Signers
{
    /**
     * A deterministic K calculator based on the algorithm in section 3.2 of RFC 6979.
     */
    public class HMacDsaKCalculator
        :   IDsaKCalculator
    {
        private readonly HMac hMac;
        private readonly byte[] K;
        private readonly byte[] V;

        private BigInteger n;

        /**
         * Base constructor.
         *
         * @param digest digest to build the HMAC on.
         */
        public HMacDsaKCalculator(IDigest digest)
        {
            this.hMac = new HMac(digest);
            this.V = new byte[hMac.GetMacSize()];
            this.K = new byte[hMac.GetMacSize()];
        }

        public virtual bool IsDeterministic
        {
            get { return true; }
        }

        public virtual void Init(BigInteger n, SecureRandom random)
        {
            throw new InvalidOperationException("Operation not supported");
        }

        public void Init(BigInteger n, BigInteger d, byte[] message)
        {
            this.n = n;

            Arrays.Fill(V, (byte)0x01);
            Arrays.Fill(K, (byte)0);

            byte[] x = new byte[(n.BitLength + 7) / 8];
            byte[] dVal = BigIntegers.AsUnsignedByteArray(d);

            Array.Copy(dVal, 0, x, x.Length - dVal.Length, dVal.Length);

            byte[] m = new byte[(n.BitLength + 7) / 8];

            BigInteger mInt = BitsToInt(message);

            if (mInt.CompareTo(n) >= 0)
            {
                mInt = mInt.Subtract(n);
            }

            byte[] mVal = BigIntegers.AsUnsignedByteArray(mInt);

            Array.Copy(mVal, 0, m, m.Length - mVal.Length, mVal.Length);

            hMac.Init(new KeyParameter(K));

            hMac.BlockUpdate(V, 0, V.Length);
            hMac.Update((byte)0x00);
            hMac.BlockUpdate(x, 0, x.Length);
            hMac.BlockUpdate(m, 0, m.Length);

            hMac.DoFinal(K, 0);

            hMac.Init(new KeyParameter(K));

            hMac.BlockUpdate(V, 0, V.Length);

            hMac.DoFinal(V, 0);

            hMac.BlockUpdate(V, 0, V.Length);
            hMac.Update((byte)0x01);
            hMac.BlockUpdate(x, 0, x.Length);
            hMac.BlockUpdate(m, 0, m.Length);

            hMac.DoFinal(K, 0);

            hMac.Init(new KeyParameter(K));

            hMac.BlockUpdate(V, 0, V.Length);

            hMac.DoFinal(V, 0);
        }

        public virtual BigInteger NextK()
        {
            byte[] t = new byte[((n.BitLength + 7) / 8)];

            for (;;)
            {
                int tOff = 0;

                while (tOff < t.Length)
                {
                    hMac.BlockUpdate(V, 0, V.Length);

                    hMac.DoFinal(V, 0);

                    int len = System.Math.Min(t.Length - tOff, V.Length);
                    Array.Copy(V, 0, t, tOff, len);
                    tOff += len;
                }

                BigInteger k = BitsToInt(t);

                if (k.SignValue > 0 && k.CompareTo(n) < 0)
                {
                    return k;
                }

                hMac.BlockUpdate(V, 0, V.Length);
                hMac.Update((byte)0x00);

                hMac.DoFinal(K, 0);

                hMac.Init(new KeyParameter(K));

                hMac.BlockUpdate(V, 0, V.Length);

                hMac.DoFinal(V, 0);
            }
        }

        private BigInteger BitsToInt(byte[] t)
        {
            BigInteger v = new BigInteger(1, t);

            if (t.Length * 8 > n.BitLength)
            {
                v = v.ShiftRight(t.Length * 8 - n.BitLength);
            }

            return v;
        }
    }
}

#endif
