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

using LitJson;

namespace BestHTTP.SocketIO.JsonEncoders
{
    /// <summary>
    /// This IJsonEncoder implementation uses the LitJson library located in the Examples\LitJson directory.
    /// </summary>
    public sealed class LitJsonEncoder : IJsonEncoder
    {
        public List<object> Decode(string json)
        {
            JsonReader reader = new JsonReader(json);
            return JsonMapper.ToObject<List<object>>(reader);
        }

        public string Encode(List<object> obj)
        {
            JsonWriter writer = new JsonWriter();
            JsonMapper.ToJson(obj, writer);

            return writer.ToString();
        }
    }
}

#endif
