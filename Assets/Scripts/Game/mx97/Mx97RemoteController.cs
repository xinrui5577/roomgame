using System;
using System.Collections.Generic;
using Sfs2X.Core;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using YxFramwork.Controller;

namespace Assets.Scripts.Game.mx97{
    public class Mx97RemoteController : YxGameServer
    {
        protected void OnRoomNameChange(BaseEvent evt)
        {
        }

        //public SmartFox Smart;
        // 不对称协议 发送进场之后的回复函数 请求key为GlobalData.GameKey+RequestCMD.QuickGame
        /// <summary>
        /// 设置回调
        /// </summary>
        /// <param name="dictionary">收到服务器数据后根据Cmd调用对应的回掉方法</param>
        public override void Init(Dictionary<string, Action<ISFSObject>> dictionary)
        {
        }

        public void OnUserVariableUpdate(BaseEvent eEvent) {
        }
       
        public void Ready()
        { 
                string key = GameKey + YxFramwork.ConstDefine.RequestCmd.Ready;
                IRequest request = new ExtensionRequest(key, SFSObject.NewInstance());
                SendRequest(request);
        }
    }
}
