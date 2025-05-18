using System;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Utils;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.Manager;
using YxFramwork.View;
using YxFramwork.Enums;

namespace Assets.Scripts.Game.toubao
{
    public class TouBaoGameServer : YxGameServer
    {

        //--------------------------------
        /// <summary>
        /// 交互初始化
        /// </summary>
        /// <param name="callBackDic"></param>
        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {
            callBackDic["bank"] = OnBank;
            callBackDic["buy"] = OnBuy;
        }
        
        public virtual void OnBank(ISFSObject requestData)
        {
            var gdata = App.GetGameData<GlobalData>();
            YxWindowManager.HideWaitFor();
            if (requestData.GetBool("result"))
            {
                var self = gdata.GetPlayer();
                self.Info.Parse(requestData);
                self.UpdateView();
            }
        }

        public virtual void OnBuy(ISFSObject requestData)
        {
            var gdata = App.GetGameData<GlobalData>();
            YxWindowManager.HideWaitFor();
            if (requestData.GetBool("result"))
            {
                var self = gdata.GetPlayer();
                self.Info.Parse(requestData);
                self.UpdateView();
            }
        }

        public void UserBet(string table, int gold)
        {
            App.GameData.GStatus = YxEGameStatus.PlayAndConfine;
            var sfsObject = new SFSObject();
            sfsObject.PutInt("type", RequestType.XiaZhu);
            sfsObject.PutUtfString("p", table);
            sfsObject.PutInt("gold", gold);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }

        public void ApplyBaker(bool flag)
        {
            var sfsObject = new SFSObject();
            int type = flag ? RequestType.Bet : RequestType.XiaZhuang;
            sfsObject.PutInt("type", type);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }
        public void ApplyQuit()
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt("type", RequestType.XiaZhuang);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }
    }

}

