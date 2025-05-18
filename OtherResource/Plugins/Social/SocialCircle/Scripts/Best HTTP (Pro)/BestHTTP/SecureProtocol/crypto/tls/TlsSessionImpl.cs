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
    internal class TlsSessionImpl
        :   TlsSession
    {
        internal readonly byte[] mSessionID;
        internal SessionParameters mSessionParameters;

        internal TlsSessionImpl(byte[] sessionID, SessionParameters sessionParameters)
        {
            if (sessionID == null)
                throw new ArgumentNullException("sessionID");
            if (sessionID.Length < 1 || sessionID.Length > 32)
                throw new ArgumentException("must have length between 1 and 32 bytes, inclusive", "sessionID");

            this.mSessionID = Arrays.Clone(sessionID);
            this.mSessionParameters = sessionParameters;
        }

        public virtual SessionParameters ExportSessionParameters()
        {
            lock (this)
            {
                return this.mSessionParameters == null ? null : this.mSessionParameters.Copy();
            }
        }

        public virtual byte[] SessionID
        {
            get { lock (this) return mSessionID; }
        }

        public virtual void Invalidate()
        {
            lock (this)
            {
                if (this.mSessionParameters != null)
                {
                    this.mSessionParameters.Clear();
                    this.mSessionParameters = null;
                }
            }
        }

        public virtual bool IsResumable
        {
            get { lock (this) return this.mSessionParameters != null; }
        }
    }
}

#endif
