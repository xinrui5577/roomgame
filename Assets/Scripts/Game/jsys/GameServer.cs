using System;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using YxFramwork.Controller;

namespace Assets.Scripts.Game.jsys
{
    public class GameServer : YxGameServer
    { 
        //发送押注数据
        public void ClickedToSend(int[] yaZhu)
        {
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutInt("type",1);
            sfsObject.PutIntArray("antes", yaZhu);
            
            SendGameRequest(sfsObject);
        }
    }
}
