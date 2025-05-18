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
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class HeartbeatExtension
    {
        protected readonly byte mMode;

        public HeartbeatExtension(byte mode)
        {
            if (!HeartbeatMode.IsValid(mode))
                throw new ArgumentException("not a valid HeartbeatMode value", "mode");

            this.mMode = mode;
        }

        public virtual byte Mode
        {
            get { return mMode; }
        }

        /**
         * Encode this {@link HeartbeatExtension} to a {@link Stream}.
         * 
         * @param output
         *            the {@link Stream} to encode to.
         * @throws IOException
         */
        public virtual void Encode(Stream output)
        {
            TlsUtilities.WriteUint8(mMode, output);
        }

        /**
         * Parse a {@link HeartbeatExtension} from a {@link Stream}.
         * 
         * @param input
         *            the {@link Stream} to parse from.
         * @return a {@link HeartbeatExtension} object.
         * @throws IOException
         */
        public static HeartbeatExtension Parse(Stream input)
        {
            byte mode = TlsUtilities.ReadUint8(input);
            if (!HeartbeatMode.IsValid(mode))
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);

            return new HeartbeatExtension(mode);
        }
    }
}

#endif
