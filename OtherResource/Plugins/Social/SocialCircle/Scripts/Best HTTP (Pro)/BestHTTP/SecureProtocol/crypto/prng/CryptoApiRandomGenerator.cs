/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

#if !NETCF_1_0

using System;

#if NETFX_CORE
    using Windows.Security.Cryptography;
#else
    using System.Security.Cryptography;
#endif

namespace Org.BouncyCastle.Crypto.Prng
{
    /// <summary>
    /// Uses Microsoft's RNGCryptoServiceProvider
    /// </summary>
    public class CryptoApiRandomGenerator
        : IRandomGenerator
    {
#if !NETFX_CORE
        private readonly RandomNumberGenerator rndProv;
#endif

        public CryptoApiRandomGenerator()
#if !NETFX_CORE
            : this(new RNGCryptoServiceProvider())
#endif
        {
        }

#if !NETFX_CORE
        public CryptoApiRandomGenerator(RandomNumberGenerator rng)
        {
            this.rndProv = rng;
        }
#endif

        #region IRandomGenerator Members

        public virtual void AddSeedMaterial(byte[] seed)
        {
            // We don't care about the seed
        }

        public virtual void AddSeedMaterial(long seed)
        {
            // We don't care about the seed
        }

        public virtual void NextBytes(byte[] bytes)
        {
#if NETFX_CORE
            var buffer = CryptographicBuffer.GenerateRandom((uint)bytes.Length);
            byte[] finalBytes = null;
            CryptographicBuffer.CopyToByteArray(buffer, out finalBytes);
            finalBytes.CopyTo(bytes, 0);
#else
            rndProv.GetBytes(bytes);
#endif
        }

        public virtual void NextBytes(byte[] bytes, int start, int len)
        {
            if (start < 0)
                throw new ArgumentException("Start offset cannot be negative", "start");
            if (bytes.Length < (start + len))
                throw new ArgumentException("Byte array too small for requested offset and length");

            if (bytes.Length == len && start == 0) 
            {
                NextBytes(bytes);
            }
            else 
            {
#if NETFX_CORE
                byte[] tmpBuf = null;
                var buffer = CryptographicBuffer.GenerateRandom((uint)bytes.Length);
                CryptographicBuffer.CopyToByteArray(buffer, out tmpBuf);
#else
                byte[] tmpBuf = new byte[len];
                NextBytes(tmpBuf);
#endif
                Array.Copy(tmpBuf, 0, bytes, start, len);
            }
        }

        #endregion
    }
}

#endif

#endif
