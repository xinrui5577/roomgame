/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_SOCKETIO

using System.Collections.Generic;

namespace BestHTTP.SocketIO.Transports
{
    /// <summary>
    /// Possible states of an ITransport implementation.
    /// </summary>
    public enum TransportStates : int
    {
        /// <summary>
        /// The transport is connecting to the server.
        /// </summary>
        Connecting = 0,

        /// <summary>
        /// The transport is connected, and started the opening process.
        /// </summary>
        Opening = 1,

        /// <summary>
        /// The transport is open, can send and receive packets.
        /// </summary>
        Open = 2,

        /// <summary>
        /// The transport is closed.
        /// </summary>
        Closed = 3,

        /// <summary>
        /// The transport is paused.
        /// </summary>
        Paused = 4
    }

    /// <summary>
    /// An interface that a Socket.IO transport must implement.
    /// </summary>
    public interface ITransport
    {
        /// <summary>
        /// Current state of the transport
        /// </summary>
        TransportStates State { get; }

        /// <summary>
        /// SocketManager instance that this transport is bound to.
        /// </summary>
        SocketManager Manager { get; }

        /// <summary>
        /// True if the transport is busy with sending messages.
        /// </summary>
        bool IsRequestInProgress { get; }

        /// <summary>
        /// Start open/upgrade the transport.
        /// </summary>
        void Open();

        /// <summary>
        /// Do a poll for available messages on the server.
        /// </summary>
        void Poll();

        /// <summary>
        /// Send a single packet to the server.
        /// </summary>
        void Send(Packet packet);

        /// <summary>
        /// Send a list of packets to the server.
        /// </summary>
        void Send(List<Packet> packets);

        /// <summary>
        /// Close this transport.
        /// </summary>
        void Close();
    }
}

#endif
