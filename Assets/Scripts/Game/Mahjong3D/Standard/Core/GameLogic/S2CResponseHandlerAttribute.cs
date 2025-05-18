using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    class S2CResponseHandlerAttribute : Attribute
    {
        public int ProtocolKey;
        public string GameKey;

        /// <summary>
        /// 监听响应事件， 方法只能是public才会被监听
        /// </summary>
        /// <param name="protocolKey">协议号</param>
        /// <param name="gameKey">gameKey</param>
        public S2CResponseHandlerAttribute(int protocolKey, string gameKey = DefaultUtils.DefName)
        {
            ProtocolKey = protocolKey;
            GameKey = gameKey;
        }
    }
}