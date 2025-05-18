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

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Modes.Gcm
{
    public class Tables1kGcmExponentiator
        : IGcmExponentiator
    {
        // A lookup table of the power-of-two powers of 'x'
        // - lookupPowX2[i] = x^(2^i)
        private IList lookupPowX2;

        public void Init(byte[] x)
        {
            uint[] y = GcmUtilities.AsUints(x);
            if (lookupPowX2 != null && Arrays.AreEqual(y, (uint[])lookupPowX2[0]))
                return;

            lookupPowX2 = Platform.CreateArrayList(8);
            lookupPowX2.Add(y);
        }

        public void ExponentiateX(long pow, byte[] output)
        {
            uint[] y = GcmUtilities.OneAsUints();
            int bit = 0;
            while (pow > 0)
            {
                if ((pow & 1L) != 0)
                {
                    EnsureAvailable(bit);
                    GcmUtilities.Multiply(y, (uint[])lookupPowX2[bit]);
                }
                ++bit;
                pow >>= 1;
            }

            GcmUtilities.AsBytes(y, output);
        }

        private void EnsureAvailable(int bit)
        {
            int count = lookupPowX2.Count;
            if (count <= bit)
            {
                uint[] tmp = (uint[])lookupPowX2[count - 1];
                do
                {
                    tmp = Arrays.Clone(tmp);
                    GcmUtilities.Multiply(tmp, tmp);
                    lookupPowX2.Add(tmp);
                }
                while (++count <= bit);
            }
        }
    }
}

#endif
