/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if !BESTHTTP_DISABLE_SIGNALR

using System.Collections.Generic;

namespace BestHTTP.SignalR.JsonEncoders
{
    /// <summary>
    /// Interface to be able to write custom Json encoders/decoders.
    /// </summary>
    public interface IJsonEncoder
    {
        /// <summary>
        /// This function must create a json formatted string from the given object. If the encoding fails, it should return null.
        /// </summary>
        string Encode(object obj);

        /// <summary>
        /// This function must create a dictionary the Json formatted string parameter. If the decoding fails, it should return null.
        /// </summary>
        IDictionary<string, object> DecodeMessage(string json);
    }
}

#endif
