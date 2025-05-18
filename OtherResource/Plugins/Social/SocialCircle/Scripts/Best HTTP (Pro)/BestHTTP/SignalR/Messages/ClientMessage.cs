/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_SIGNALR

using System;

using BestHTTP.SignalR.Hubs;

namespace BestHTTP.SignalR.Messages
{
    /// <summary>
    /// This struct represents a message from the client.
    /// It holds every data and reference needed to construct the string represented message that will be sent to the wire.
    /// </summary>
    public struct ClientMessage
    {
        /// <summary>
        /// Reference to the source Hub. The Name and the State of the hub will be user.
        /// </summary>
        public readonly Hub Hub;

        /// <summary>
        /// Name of the method on the server to be called.
        /// </summary>
        public readonly string Method;

        /// <summary>
        /// Arguments of the method.
        /// </summary>
        public readonly object[] Args;

        /// <summary>
        /// Unique id on the client of this message
        /// </summary>
        public readonly UInt64 CallIdx;

        /// <summary>
        /// The delegate that will be called when the server will sends a result of this method call.
        /// </summary>
        public readonly OnMethodResultDelegate ResultCallback;

        /// <summary>
        /// The delegate that will be called when the server sends an error-result to this method call.
        /// </summary>
        public readonly OnMethodFailedDelegate ResultErrorCallback;

        /// <summary>
        /// The delegate that will be called when the server sends a progress message to this method call.
        /// </summary>
        public readonly OnMethodProgressDelegate ProgressCallback;

        public ClientMessage(Hub hub,
                             string method, 
                             object[] args, 
                             UInt64 callIdx, 
                             OnMethodResultDelegate resultCallback,
                             OnMethodFailedDelegate resultErrorCallback, 
                             OnMethodProgressDelegate progressCallback)
        {
            Hub = hub;
            Method = method;
            Args = args;

            CallIdx = callIdx;

            ResultCallback = resultCallback;
            ResultErrorCallback = resultErrorCallback;
            ProgressCallback = progressCallback;
        }
    }
}

#endif
