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

namespace BestHTTP.SocketIO.JsonEncoders
{
    /// <summary>
    /// Interface to be able to write custom Json encoders/decoders.
    /// </summary>
    public interface IJsonEncoder
    {
        /// <summary>
        /// The Decode function must create a list of objects from the Json formatted string parameter. If the decoding fails, it should return null.
        /// </summary>
        List<object> Decode(string json);

        /// <summary>
        /// The Encode function must create a json formatted string from the parameter. If the encoding fails, it should return null.
        /// </summary>
        string Encode(List<object> obj);
    }
}

#endif
