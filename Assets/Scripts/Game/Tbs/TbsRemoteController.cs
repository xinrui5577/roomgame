using System;
using System.Collections;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;

namespace Assets.Scripts.Game.Tbs
{
    public class TbsRemoteController : YxGameServer
    {
        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {
            var gameKey = GameKey;
            YxDebug.Log(gameKey);
            App.GetGameManager<TbsGameManager>().ResponseSfsObjects = new Queue<ISFSObject>();
            callBackDic["hup"] = OnHandsUp;
            callBackDic[GameKey + "over"] = OnGameOver;
        }
        /// <summary>
        /// 发送准备信息
        /// </summary>
        public void ReadyGame()
        {
            YxDebug.Log("发送准备!!");
            string key = GameKey + RequestCmd.Ready;
            IRequest request = new ExtensionRequest(key, SFSObject.NewInstance());
            SendRequest(request);
        }
   
        /// <summary>
        /// 发送交互信息
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="data"></param>
        public void SendRequest(GameRequestType rt, IDictionary data)
        {
            YxDebug.Log("SendRequest == " + rt);

            if (!HasGetGameInfo)
            {
                YxDebug.LogError("GameInfo还没有初始化!!");
                return;
            }
            var sfsObject = new SFSObject();
            
            switch (rt)
            {
                case GameRequestType.Banker:
                    sfsObject.PutInt("type", (int)rt);
                    break;
                case GameRequestType.SetGuo:
                    sfsObject.PutInt("gold", (int)data["gold"]);
                    sfsObject.PutInt("type", (int)rt);
                    break;
                case GameRequestType.CutPoker:
                    sfsObject.PutInt("type",(int)rt);
                    break;
                case GameRequestType.BeginBet:
                    break;
                case GameRequestType.Bet:
                    sfsObject.PutInt("gold", (int)data["gold"]);
                    sfsObject.PutInt("type", (int)rt);
                    break;
                case GameRequestType.StopBet:
                    sfsObject.PutInt("type", (int)rt);
                    break;
                case GameRequestType.RollDice:
                    sfsObject.PutInt("type", (int)rt);
                    break;
                case GameRequestType.Allocate:
                    break;
                case GameRequestType.OpenPoker:
                    sfsObject.PutInt("type", (int)rt);
                    break;
                case GameRequestType.Result:
                    break;
                case GameRequestType.StartRollDice:
                    sfsObject.PutInt("type",(int)rt);
                    break;
                default:
                    YxDebug.Log("不存在的服务器交互!");
                    break;
            }
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }
        #region 投票解散的发送和接收
        // 发起解散投票
        public void StartHandsUp(int yon)
        {
            SendFrameRequest("dissolve", new SFSObject());
        }
        // 返回投票
        public void OnHandsUp(ISFSObject requestData)
        {
            var data = new HupData
            {
                Name = requestData.GetUtfString("username"),
                Id = requestData.GetInt(RequestKey.KeyId),
                Operation = requestData.GetInt(RequestKey.KeyType)
            };
            int time = App.GetGameData<TbsGameData>().HupTime;
            if (requestData.ContainsKey("cdTime"))
            {
                time = requestData.GetInt("cdTime");
            }
            App.GetGameManager<TbsGameManager>().HupMgr.ShowHandUp(data, time);
        }
        public void DealHupInfo(ISFSObject data)
        {
            var gdata = App.GetGameData<TbsGameData>();
            string hupInfo;
            if (data.ContainsKey("hup"))
            {
                hupInfo = data.GetUtfString("hup");
            }
            else
            {
                return;
            }
            //接收重连解散信息
            if (!string.IsNullOrEmpty(hupInfo))
            {
                long svt = data.ContainsKey("svt") ? data.GetLong("svt") : 0;
                long hupStart = data.ContainsKey("hupstart") ? data.GetLong("hupstart") : 0;
                var time = (int)(gdata.HupTime - (svt - hupStart));
                time = time < 0 ? 0 : time;
                string[] ids = hupInfo.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    for (int j = 0, max = gdata.PlayerList.Length; j < max; j++)
                    {
                        var id = ids[i];
                        if (gdata.GetPlayerInfo(j, true) != null && id.Equals(gdata.GetPlayerInfo(j,true).UserId))
                        {
                            //2发起投票 3同意 -1拒绝
                            ISFSObject hupData = new SFSObject();
                            hupData.PutUtfString("username", gdata.GetPlayerInfo(j, true).NickM);
                            hupData.PutInt(RequestKey.KeyType, i == 0 ? 2 : 3);
                            hupData.PutInt("cdTime", time);
                            hupData.PutInt(RequestKey.KeyId, int.Parse(id));
                            OnHandsUp(hupData);
                        }
                    }
                }
            }
        }
        #endregion
        private void OnGameOver(ISFSObject requestData)
        {
            var gmanager = App.GetGameManager<TbsGameManager>();
            gmanager.HupMgr.DirectClose();
            gmanager.GameOverMgr.SetData(requestData);
        }

    }

    /// <summary>
    /// 游戏服务交互
    /// </summary>
    public enum GameRequestType
    {
        Banker = 1,         //系统选庄or切锅
        SetGuo,         //设置锅钱数
        CutPoker,       //切牌
        BeginBet,       //开始下注
        Bet,            //下注
        StopBet,        //停止下注
        RollDice,       //掷骰子
        Allocate,       //发牌
        OpenPoker,           //翻牌
        Result,         //结算
        StartRollDice,      //开始掷骰子
        GameStart=10086,     //游戏开始刷新面板
    }
}