/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_WEBSOCKET && (!UNITY_WEBGL || UNITY_EDITOR)

using BestHTTP.WebSocket.Frames;

namespace BestHTTP.WebSocket.Extensions
{
    public interface IExtension
    {
        /// <summary>
        /// This is the first pass: here we can add headers to the request to initiate an extension negotiation.
        /// </summary>
        /// <param name="request"></param>
        void AddNegotiation(HTTPRequest request);

        /// <summary>
        /// If the websocket upgrade succeded it will call this function to be able to parse the server's negotiation
        /// response. Inside this function the IsEnabled should be set.
        /// </summary>
        bool ParseNegotiation(WebSocketResponse resp);

        /// <summary>
        /// This function should return a new header flag based on the inFlag parameter. The extension should set only the
        /// Rsv1-3 bits in the header.
        /// </summary>
        byte GetFrameHeader(WebSocketFrame writer, byte inFlag);

        /// <summary>
        /// This function will be called to be able to transform the data that will be sent to the server.
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        byte[] Encode(WebSocketFrame writer);

        /// <summary>
        /// This function can be used the decode the server-sent data.
        /// </summary>
        byte[] Decode(byte header, byte[] data);
    }
}

#endif
