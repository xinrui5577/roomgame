using System;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.View;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.nn41
{
    public class NnGameServer : YxGameServer
    {
        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {
            callBackDic[GameKey + RequestCmd.Ready] = OnUserReady;
            callBackDic[InteractParameter.Hup] = OnHandsUp;
            callBackDic[GameKey + InteractParameter.GameOver] = OnGameOver;
        }

        private void OnUserReady(ISFSObject responseData)
        {
            if (HasGetGameInfo == false) return;
            var serverSeat = responseData.GetInt(InteractParameter.Seat);
            App.GetGameManager<NnGameManager>().OnUserReady(serverSeat);
        }

        public void OnHandsUp(ISFSObject requestData)
        {
            var data = new HupData
            {
                Name = requestData.GetUtfString(InteractParameter.UserName),
                Id = requestData.GetInt(RequestKey.KeyId),
                Operation = requestData.GetInt(RequestKey.KeyType)
            };
            int time = App.GetGameData<NnGameData>().HupTime;
            if (requestData.ContainsKey(InteractParameter.CdTime))
            {
                time = requestData.GetInt(InteractParameter.CdTime);
            }
            App.GetGameManager<NnGameManager>().HupWindow.ShowHandUp(data, time);
        }

        private void OnGameOver(ISFSObject requestData)
        {
            var gmanager = App.GetGameManager<NnGameManager>();
            gmanager.HupWindow.DirectClose();
            gmanager.GameOverCtrl.AnalysisData(requestData);
        }

        public void RequestGameInfo()
        {
            SendRequest(new ExtensionRequest(GameKey + InteractParameter.Crjin, SFSObject.NewInstance()));
        }

        public void SendLiang()
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutInt(InteractParameter.Type, 13);//亮牌
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, iobj));
        }

        public void Ante(int gold)
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt(InteractParameter.Type, 21);
            sfsObject.PutInt(InteractParameter.Gold, gold);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }

        public void RobBank(int rate)
        {
            SFSObject data = SFSObject.NewInstance();
            data.PutInt(InteractParameter.Rate, rate);
            data.PutInt(InteractParameter.Type, RequestType.RobBank);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, data));
        }

        public void Banker()
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt(InteractParameter.Type, 20);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }

        public void ReadyGame()
        {
            IRequest request = new ExtensionRequest(GameKey + RequestCmd.Ready, SFSObject.NewInstance());
            SendRequest(request);
        }

        public void BeginGame()
        {
            SFSObject data = SFSObject.NewInstance();
            data.PutInt(InteractParameter.Type, RequestType.BeginGame);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, data));
        }

        public void StartHandsUp(int yon)
        {
            var gdata = App.GetGameData<NnGameData>();

            if (gdata.CurrentRound == 0)
            {
                //局外只有房主才能解散，其他玩家只能退出
                if (gdata.OwnerId==int.Parse(gdata.GetPlayerInfo().UserId))
                {
                    YxDebug.Log("局外房主申请");
                    SendFrameRequest(InteractParameter.Dissolve, new SFSObject());
                }
                else
                {
                   App.QuitGame();
                }
            }
            else
            {
                //局内任何玩家都可以发起投票
                ISFSObject iobj = new SFSObject();
                iobj.PutUtfString(InteractParameter.Cmd, InteractParameter.Dismiss);
                iobj.PutInt(RequestKey.KeyType, yon);
                iobj.PutInt(RequestKey.KeyId, int.Parse(gdata.GetPlayerInfo().UserId));
                SendFrameRequest(InteractParameter.Hup, iobj);
            }
        }

        public void UserAnimate(int index, int seat)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutInt(InteractParameter.AniIndex, index);
            iobj.PutInt(InteractParameter.Seat, App.GameData.SelfSeat);
            iobj.PutInt(InteractParameter.OtherSeat, seat);//其他的玩家座位号
            IRequest request = new ExtensionRequest(GameKey + InteractParameter.Talk, iobj);
            SendRequest(request);
        }
    }
}
