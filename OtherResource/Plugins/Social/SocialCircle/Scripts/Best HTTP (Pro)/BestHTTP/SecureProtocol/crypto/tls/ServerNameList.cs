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

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class ServerNameList
    {
        protected readonly IList mServerNameList;

        /**
         * @param serverNameList an {@link IList} of {@link ServerName}.
         */
        public ServerNameList(IList serverNameList)
        {
            if (serverNameList == null || serverNameList.Count < 1)
                throw new ArgumentException("must not be null or empty", "serverNameList");

            this.mServerNameList = serverNameList;
        }

        /**
         * @return an {@link IList} of {@link ServerName}.
         */
        public virtual IList ServerNames
        {
            get { return mServerNameList; }
        }

        /**
         * Encode this {@link ServerNameList} to a {@link Stream}.
         * 
         * @param output
         *            the {@link Stream} to encode to.
         * @throws IOException
         */
        public virtual void Encode(Stream output)
        {
            MemoryStream buf = new MemoryStream();

            foreach (ServerName entry in ServerNames)
            {
                entry.Encode(buf);
            }

            TlsUtilities.CheckUint16(buf.Length);
            TlsUtilities.WriteUint16((int)buf.Length, output);
            buf.WriteTo(output);
        }

        /**
         * Parse a {@link ServerNameList} from a {@link Stream}.
         * 
         * @param input
         *            the {@link Stream} to parse from.
         * @return a {@link ServerNameList} object.
         * @throws IOException
         */
        public static ServerNameList Parse(Stream input)
        {
            int length = TlsUtilities.ReadUint16(input);
            if (length < 1)
                throw new TlsFatalAlert(AlertDescription.decode_error);

            byte[] data = TlsUtilities.ReadFully(length, input);

            MemoryStream buf = new MemoryStream(data, false);

            IList server_name_list = Platform.CreateArrayList();
            while (buf.Position < buf.Length)
            {
                ServerName entry = ServerName.Parse(buf);
                server_name_list.Add(entry);
            }

            return new ServerNameList(server_name_list);
        }
    }
}

#endif
