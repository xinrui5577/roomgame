/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_SERVERSENT_EVENTS

using System;

namespace BestHTTP.ServerSentEvents
{
    public sealed class Message
    {
        /// <summary>
        /// Event Id of the message. If it's null, then it's not present.
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Name of the event, or an empty string.
        /// </summary>
        public string Event { get; internal set; }

        /// <summary>
        /// The actual payload of the message.
        /// </summary>
        public string Data { get; internal set; }

        /// <summary>
        /// A reconnection time, in milliseconds. This must initially be a user-agent-defined value, probably in the region of a few seconds.
        /// </summary>
        public TimeSpan Retry { get; internal set; }

        public override string ToString()
        {
            return string.Format("\"{0}\": \"{1}\"", Event, Data);
        }
    }
}

#endif
