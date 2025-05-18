using System.Collections.Generic;
using Sfs2X.Entities.Data;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;

//using Assets.CommonCode;
//using Assets.Scripts.FourManMJ;
//using ChessComponent;

namespace Assets.Scripts.Game.slyz{
    public class SlyzGameServer : YxGameServer
    {

        //public Game Game;
        public override void Init(Dictionary<string, System.Action<ISFSObject>> responseDic)
        {
        }

        public void SendStart()
        {
            var data = SFSObject.NewInstance();
            data.PutInt(RequestKey.KeyType, 1);
            SendGameRequest(data);
        }
    }
}
