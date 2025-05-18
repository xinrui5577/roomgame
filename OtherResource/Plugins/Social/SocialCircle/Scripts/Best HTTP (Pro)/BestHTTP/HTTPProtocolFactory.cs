/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BestHTTP
{
    public enum SupportedProtocols
    {
        Unknown,
        HTTP,

#if !BESTHTTP_DISABLE_WEBSOCKET
        WebSocket,
#endif

#if !BESTHTTP_DISABLE_SERVERSENT_EVENTS
        ServerSentEvents
#endif
    }

    internal static class HTTPProtocolFactory
    {
        public static HTTPResponse Get(SupportedProtocols protocol, HTTPRequest request, Stream stream, bool isStreamed, bool isFromCache)
        {
            switch (protocol)
            {
#if !BESTHTTP_DISABLE_WEBSOCKET && (!UNITY_WEBGL || UNITY_EDITOR)
                case SupportedProtocols.WebSocket: return new WebSocket.WebSocketResponse(request, stream, isStreamed, isFromCache);
#endif

#if !BESTHTTP_DISABLE_SERVERSENT_EVENTS && (!UNITY_WEBGL || UNITY_EDITOR)
                case SupportedProtocols.ServerSentEvents: return new ServerSentEvents.EventSourceResponse(request, stream, isStreamed, isFromCache);
#endif
                default: return new HTTPResponse(request, stream, isStreamed, isFromCache);
            }
        }

        public static SupportedProtocols GetProtocolFromUri(Uri uri)
        {
            if (uri == null || uri.Scheme == null)
                throw new Exception("Malformed URI in GetProtocolFromUri");

            string scheme = uri.Scheme.ToLowerInvariant();
            switch (scheme)
            {
#if !BESTHTTP_DISABLE_WEBSOCKET
                case "ws":
                case "wss":
                    return SupportedProtocols.WebSocket;
#endif

                default:
                    return SupportedProtocols.HTTP;
            }
        }

        public static bool IsSecureProtocol(Uri uri)
        {
            if (uri == null || uri.Scheme == null)
                throw new Exception("Malformed URI in IsSecureProtocol");

            string scheme = uri.Scheme.ToLowerInvariant();
            switch (scheme)
            {
                // http
                case "https":

#if !BESTHTTP_DISABLE_WEBSOCKET
                // WebSocket
                case "wss":
#endif
                    return true;
            }

            return false;
        }
    }
}
