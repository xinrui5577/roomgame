using System;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;

namespace Assets.Scripts.Game.brnn3d
{
    public class Brnn3dGameServer : YxGameServer
    { 
        public void ApplyBanker()
        {
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutInt("type", 2);
            SendGameRequest(sfsObject);
        }
        public void ApplyQuit()
        {
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutInt("type", 3);
            SendGameRequest(sfsObject);
        }

        public void UserBet(int table, int gold)
        {
            Debug.LogError(table + " "+gold);
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutInt("p", table);
            sfsObject.PutInt("gold", gold);
            sfsObject.PutInt("type", 1);
            SendGameRequest(sfsObject);
        }
    }
}
