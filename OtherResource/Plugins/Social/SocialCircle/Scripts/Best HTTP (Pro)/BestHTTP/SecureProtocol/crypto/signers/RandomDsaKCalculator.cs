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

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Signers
{
    public class RandomDsaKCalculator
        :   IDsaKCalculator
    {
        private BigInteger q;
        private SecureRandom random;

        public virtual bool IsDeterministic
        {
            get { return false; }
        }

        public virtual void Init(BigInteger n, SecureRandom random)
        {
            this.q = n;
            this.random = random;
        }

        public virtual void Init(BigInteger n, BigInteger d, byte[] message)
        {
            throw new InvalidOperationException("Operation not supported");
        }

        public virtual BigInteger NextK()
        {
            int qBitLength = q.BitLength;

            BigInteger k;
            do
            {
                k = new BigInteger(qBitLength, random);
            }
            while (k.SignValue < 1 || k.CompareTo(q) >= 0);

            return k;
        }
    }
}

#endif
