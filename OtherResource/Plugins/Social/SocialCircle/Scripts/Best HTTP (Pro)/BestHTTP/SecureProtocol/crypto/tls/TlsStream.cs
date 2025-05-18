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
    internal class TlsStream
        : Stream
    {
        private readonly TlsProtocol handler;

        internal TlsStream(TlsProtocol handler)
        {
            this.handler = handler;
        }

        public override bool CanRead
        {
            get { return !handler.IsClosed; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return !handler.IsClosed; }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                handler.Close();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override void Flush()
        {
            handler.Flush();
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override int Read(byte[]	buf, int off, int len)
        {
            return this.handler.ReadApplicationData(buf, off, len);
        }

        public override int ReadByte()
        {
            byte[] buf = new byte[1];
            if (this.Read(buf, 0, 1) <= 0)
                return -1;
            return buf[0];
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buf, int off, int len)
        {
            this.handler.WriteData(buf, off, len);
        }

        public override void WriteByte(byte b)
        {
            this.handler.WriteData(new byte[] { b }, 0, 1);
        }
    }
}

#endif
