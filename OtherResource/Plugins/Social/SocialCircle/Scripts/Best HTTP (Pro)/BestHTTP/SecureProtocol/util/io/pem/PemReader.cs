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
using System.IO;
using System.Text;

using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Utilities.IO.Pem
{
	public class PemReader
	{
		private const string BeginString = "-----BEGIN ";
		private const string EndString = "-----END ";

		private readonly TextReader reader;

		public PemReader(TextReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			this.reader = reader;
		}

		public TextReader Reader
		{
			get { return reader; }
		}

		/// <returns>
		/// A <see cref="PemObject"/>
		/// </returns>
		/// <exception cref="IOException"></exception>
		public PemObject ReadPemObject()
		{
			string line = reader.ReadLine();

			if (line != null && line.StartsWith(BeginString))
			{
				line = line.Substring(BeginString.Length);
				int index = line.IndexOf('-');
				string type = line.Substring(0, index);

				if (index > 0)
					return LoadObject(type);
			}

			return null;
		}

		private PemObject LoadObject(string type)
		{
			string endMarker = EndString + type;
			IList headers = Platform.CreateArrayList();
			StringBuilder buf = new StringBuilder();

			string line;
			while ((line = reader.ReadLine()) != null
				&& line.IndexOf(endMarker) == -1)
			{
				int colonPos = line.IndexOf(':');

				if (colonPos == -1)
				{
					buf.Append(line.Trim());
				}
				else
				{
					// Process field
					string fieldName = line.Substring(0, colonPos).Trim();

					if (fieldName.StartsWith("X-"))
						fieldName = fieldName.Substring(2);

					string fieldValue = line.Substring(colonPos + 1).Trim();

					headers.Add(new PemHeader(fieldName, fieldValue));
				}
			}

			if (line == null)
			{
				throw new IOException(endMarker + " not found");
			}

			if (buf.Length % 4 != 0)
			{
				throw new IOException("base64 data appears to be truncated");
			}

			return new PemObject(type, headers, Base64.Decode(buf.ToString()));
		}
	}
}

#endif
