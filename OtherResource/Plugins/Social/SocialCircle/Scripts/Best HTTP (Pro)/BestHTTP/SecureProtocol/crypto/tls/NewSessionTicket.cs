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
    public class NewSessionTicket
    {
        protected readonly long mTicketLifetimeHint;
        protected readonly byte[] mTicket;

        public NewSessionTicket(long ticketLifetimeHint, byte[] ticket)
        {
            this.mTicketLifetimeHint = ticketLifetimeHint;
            this.mTicket = ticket;
        }

        public virtual long TicketLifetimeHint
        {
            get { return mTicketLifetimeHint; }
        }

        public virtual byte[] Ticket
        {
            get { return mTicket; }
        }

        /**
         * Encode this {@link NewSessionTicket} to a {@link Stream}.
         *
         * @param output the {@link Stream} to encode to.
         * @throws IOException
         */
        public virtual void Encode(Stream output)
        {
            TlsUtilities.WriteUint32(mTicketLifetimeHint, output);
            TlsUtilities.WriteOpaque16(mTicket, output);
        }

        /**
         * Parse a {@link NewSessionTicket} from a {@link Stream}.
         *
         * @param input the {@link Stream} to parse from.
         * @return a {@link NewSessionTicket} object.
         * @throws IOException
         */
        public static NewSessionTicket Parse(Stream input)
        {
            long ticketLifetimeHint = TlsUtilities.ReadUint32(input);
            byte[] ticket = TlsUtilities.ReadOpaque16(input);
            return new NewSessionTicket(ticketLifetimeHint, ticket);
        }
    }
}

#endif
