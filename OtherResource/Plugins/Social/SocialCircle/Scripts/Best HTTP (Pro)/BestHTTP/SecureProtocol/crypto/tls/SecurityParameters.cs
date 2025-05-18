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

namespace Org.BouncyCastle.Crypto.Tls
{
    public class SecurityParameters
    {
        internal int entity = -1;
        internal int cipherSuite = -1;
        internal byte compressionAlgorithm = CompressionMethod.cls_null;
        internal int prfAlgorithm = -1;
        internal int verifyDataLength = -1;
        internal byte[] masterSecret = null;
        internal byte[] clientRandom = null;
        internal byte[] serverRandom = null;
        internal byte[] sessionHash = null;
        internal byte[] pskIdentity = null;
        internal byte[] srpIdentity = null;

        // TODO Keep these internal, since it's maybe not the ideal place for them
        internal short maxFragmentLength = -1;
        internal bool truncatedHMac = false;
        internal bool encryptThenMac = false;
        internal bool extendedMasterSecret = false;

        internal virtual void Clear()
        {
            if (this.masterSecret != null)
            {
                Arrays.Fill(this.masterSecret, (byte)0);
                this.masterSecret = null;
            }
        }

        /**
         * @return {@link ConnectionEnd}
         */
        public virtual int Entity
        {
            get { return entity; }
        }

        /**
         * @return {@link CipherSuite}
         */
        public virtual int CipherSuite
        {
            get { return cipherSuite; }
        }

        /**
         * @return {@link CompressionMethod}
         */
        public byte CompressionAlgorithm
        {
            get { return compressionAlgorithm; }
        }

        /**
         * @return {@link PRFAlgorithm}
         */
        public virtual int PrfAlgorithm
        {
            get { return prfAlgorithm; }
        }

        public virtual int VerifyDataLength
        {
            get { return verifyDataLength; }
        }

        public virtual byte[] MasterSecret
        {
            get { return masterSecret; }
        }

        public virtual byte[] ClientRandom
        {
            get { return clientRandom; }
        }

        public virtual byte[] ServerRandom
        {
            get { return serverRandom; }
        }

        public virtual byte[] SessionHash
        {
            get { return sessionHash; }
        }

        public virtual byte[] PskIdentity
        {
            get { return pskIdentity; }
        }

        public virtual byte[] SrpIdentity
        {
            get { return srpIdentity; }
        }
    }
}

#endif
