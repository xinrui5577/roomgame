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

using Org.BouncyCastle.Asn1.Utilities;

namespace Org.BouncyCastle.Asn1
{
	public class DerOutputStream
		: FilterStream
	{
		public DerOutputStream(Stream os)
			: base(os)
		{
		}

		private void WriteLength(
			int length)
		{
			if (length > 127)
			{
				int size = 1;
				uint val = (uint) length;

				while ((val >>= 8) != 0)
				{
					size++;
				}

				WriteByte((byte)(size | 0x80));

				for (int i = (size - 1) * 8; i >= 0; i -= 8)
				{
					WriteByte((byte)(length >> i));
				}
			}
			else
			{
				WriteByte((byte)length);
			}
		}

		internal void WriteEncoded(
			int		tag,
			byte[]	bytes)
		{
			WriteByte((byte) tag);
			WriteLength(bytes.Length);
			Write(bytes, 0, bytes.Length);
		}

		internal void WriteEncoded(
			int		tag,
			byte[]	bytes,
			int		offset,
			int		length)
		{
			WriteByte((byte) tag);
			WriteLength(length);
			Write(bytes, offset, length);
		}

		internal void WriteTag(
			int	flags,
			int	tagNo)
		{
			if (tagNo < 31)
			{
				WriteByte((byte)(flags | tagNo));
			}
			else
			{
				WriteByte((byte)(flags | 0x1f));
				if (tagNo < 128)
				{
					WriteByte((byte)tagNo);
				}
				else
				{
					byte[] stack = new byte[5];
					int pos = stack.Length;

					stack[--pos] = (byte)(tagNo & 0x7F);

					do
					{
						tagNo >>= 7;
						stack[--pos] = (byte)(tagNo & 0x7F | 0x80);
					}
					while (tagNo > 127);

					Write(stack, pos, stack.Length - pos);
				}
			}
		}

		internal void WriteEncoded(
			int		flags,
			int		tagNo,
			byte[]	bytes)
		{
			WriteTag(flags, tagNo);
			WriteLength(bytes.Length);
			Write(bytes, 0, bytes.Length);
		}

		protected void WriteNull()
		{
			WriteByte(Asn1Tags.Null);
			WriteByte(0x00);
		}

		[Obsolete("Use version taking an Asn1Encodable arg instead")]
		public virtual void WriteObject(
			object obj)
		{
			if (obj == null)
			{
				WriteNull();
			}
			else if (obj is Asn1Object)
			{
				((Asn1Object)obj).Encode(this);
			}
			else if (obj is Asn1Encodable)
			{
				((Asn1Encodable)obj).ToAsn1Object().Encode(this);
			}
			else
			{
				throw new IOException("object not Asn1Object");
			}
		}

		public virtual void WriteObject(
			Asn1Encodable obj)
		{
			if (obj == null)
			{
				WriteNull();
			}
			else
			{
				obj.ToAsn1Object().Encode(this);
			}
		}

		public virtual void WriteObject(
			Asn1Object obj)
		{
			if (obj == null)
			{
				WriteNull();
			}
			else
			{
				obj.Encode(this);
			}
		}
	}
}

#endif
