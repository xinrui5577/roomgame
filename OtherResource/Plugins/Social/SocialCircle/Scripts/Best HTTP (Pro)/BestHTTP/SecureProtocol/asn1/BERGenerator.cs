/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
using System.IO;

using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Asn1
{
    public class BerGenerator
        : Asn1Generator
    {
        private bool      _tagged = false;
        private bool      _isExplicit;
        private int          _tagNo;

        protected BerGenerator(
            Stream outStream)
            : base(outStream)
        {
        }

        public BerGenerator(
            Stream outStream,
            int tagNo,
            bool isExplicit)
            : base(outStream)
        {
            _tagged = true;
            _isExplicit = isExplicit;
            _tagNo = tagNo;
        }

		public override void AddObject(
			Asn1Encodable obj)
		{
			new BerOutputStream(Out).WriteObject(obj);
		}

		public override Stream GetRawOutputStream()
        {
            return Out;
        }

		public override void Close()
		{
			WriteBerEnd();
		}

        private void WriteHdr(
            int tag)
        {
            Out.WriteByte((byte) tag);
            Out.WriteByte(0x80);
        }

        protected void WriteBerHeader(
            int tag)
        {
            if (_tagged)
            {
                int tagNum = _tagNo | Asn1Tags.Tagged;

                if (_isExplicit)
                {
                    WriteHdr(tagNum | Asn1Tags.Constructed);
                    WriteHdr(tag);
                }
                else
                {
                    if ((tag & Asn1Tags.Constructed) != 0)
                    {
                        WriteHdr(tagNum | Asn1Tags.Constructed);
                    }
                    else
                    {
                        WriteHdr(tagNum);
                    }
                }
            }
            else
            {
                WriteHdr(tag);
            }
        }

		protected void WriteBerBody(
            Stream contentStream)
        {
			Streams.PipeAll(contentStream, Out);
        }

		protected void WriteBerEnd()
        {
            Out.WriteByte(0x00);
            Out.WriteByte(0x00);

            if (_tagged && _isExplicit)  // write extra end for tag header
            {
                Out.WriteByte(0x00);
                Out.WriteByte(0x00);
            }
        }
    }
}

#endif
