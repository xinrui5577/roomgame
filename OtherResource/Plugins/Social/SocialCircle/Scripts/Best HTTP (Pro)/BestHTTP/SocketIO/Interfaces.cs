/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_SOCKETIO

namespace BestHTTP.SocketIO
{
    using BestHTTP.SocketIO.Transports;

    /// <summary>
    /// Interface to hide internal functions from the user by implementing it as an explicit interface.
    /// </summary>
    interface IManager
    {
        void Remove(Socket socket);
        void Close(bool removeSockets = true);
        void TryToReconnect();
        bool OnTransportConnected(ITransport transport);
        void OnTransportError(ITransport trans, string err);
        void SendPacket(Packet packet);
        void OnPacket(Packet packet);
        void EmitEvent(string eventName, params object[] args);
        void EmitEvent(SocketIOEventTypes type, params object[] args);
        void EmitError(SocketIOErrors errCode, string msg);
        void EmitAll(string eventName, params object[] args);
    }

    /// <summary>
    /// Interface to hide internal functions from the user by implementing it as an explicit interface.
    /// </summary>
    interface ISocket
    {
        void Open();
        void Disconnect(bool remove);
        void OnPacket(Packet packet);
        void EmitEvent(SocketIOEventTypes type, params object[] args);
        void EmitEvent(string eventName, params object[] args);
        void EmitError(SocketIOErrors errCode, string msg);
    }
}

#endif
