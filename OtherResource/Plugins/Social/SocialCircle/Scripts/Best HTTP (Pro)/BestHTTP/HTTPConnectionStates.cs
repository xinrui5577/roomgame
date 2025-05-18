/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

namespace BestHTTP
{
    /// <summary>
    /// Possible states of a Http Connection.
    /// The ideal lifecycle of a connection that has KeepAlive is the following: Initial => [Processing => WaitForRecycle => Free] => Closed.
    /// </summary>
    internal enum HTTPConnectionStates
    {
        /// <summary>
        /// This Connection instance is just created.
        /// </summary>
        Initial,

        /// <summary>
        /// This Connection is processing a request
        /// </summary>
        Processing,

        /// <summary>
        /// The request redirected.
        /// </summary>
        Redirected,

        /// <summary>
        /// The connection is upgraded from http.
        /// </summary>
        Upgraded,

        /// <summary>
        /// Wait for the upgraded protocol to shut down.
        /// </summary>
        WaitForProtocolShutdown,

        /// <summary>
        /// The Connection is finished processing the request, it's waiting now to deliver it's result.
        /// </summary>
        WaitForRecycle,

        /// <summary>
        /// The request result's delivered, it's now up to processing again.
        /// </summary>
        Free,

        /// <summary>
        /// A request from outside of the plugin to abort the connection.
        /// </summary>
        AbortRequested,

        /// <summary>
        /// The request is not finished in the given time.
        /// </summary>
        TimedOut,

        /// <summary>
        /// If it's not a KeepAlive connection, or something happend, then we close this connection and remove from the pool.
        /// </summary>
        Closed
    }
}
