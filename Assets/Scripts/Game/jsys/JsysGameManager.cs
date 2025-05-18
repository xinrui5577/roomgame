using System;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jsys
{
    public class JsysGameManager : YxGameManager
    {
        public AnimationManager AnimationMgr;
        public BetPanelManager BetPanelMgr;
        public HistoryManager HistoryMgr;
        public GoldSharkGameUIManager GoldSharkGameUIMgr;
        public HaidiManager HaidiMgr;
        public ModelManager ModelMgr;
        public ResultUIManager ResultUIMgr;
        public TurnGroupsManager TurnGroupsMgr;
        public TimerManager TimerMgr;
        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<JsysGameData>();
            BetPanelMgr.SetMoney(gdata.UserMoney); //金币显示   
            if (gdata.Multiplying != null)
            {
                //进入游戏的时候下注位置的倍率
                BetPanelMgr.ShowImultiply(gdata.Multiplying);
            }
            HistoryMgr.ShowHistory(gdata.History);
            BetPanelMgr.Gamewaitshow();
            BetPanelMgr.ShowBetButton(false);
            ResultUIMgr.ChuXian();
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int status, ISFSObject info)
        {
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var gdata = App.GetGameData<JsysGameData>();
            switch (type)
            {
                //开始下注阶段
                case RequestType.BetStar:
                    gdata.StartBet = true;
                    gdata.BetTime = response.GetInt("cd");
                    gdata.Multiplying = response.ContainsKey("rates") ? response.GetIntArray("rates") : null;
                    if (gdata.Multiplying != null)
                    {
                        //接收游戏的下注位置的倍率的变化
                        //for (var i = 0; i < gdata.Multiplying.Length; i++)
                        //{
                        //    //YxDebug.Log("位置" + i);
                        //    //YxDebug.Log("倍数" + App.GetGameData<GlobalData>().Multiplying[i]);
                        //}
                        BetPanelMgr.ShowImultiply(gdata.Multiplying);
                    }
                    //显示下注界面
                    BetPanelMgr.ShowUI();
                    BetPanelMgr.FreshBtn();
                    TimerMgr.SetClock(gdata.BetTime);
                    break;
                //下注结束阶段
                case RequestType.BetFinish:
                    BetPanelMgr.ShowBetButton(false);
                    //收到服务器下注结束后即发送本地筹码到服务器
                    BetPanelMgr.SendBet();
                    break;
                //结算阶段
                case RequestType.Result:
                    gdata.StartBet = false;
                    BetPanelMgr.ShowBetButton(false);
                    gdata.IsShark = response.ContainsKey("fish") && response.GetBool("fish");
                    if (gdata.IsShark)
                    {
                        gdata.FishIdx = response.ContainsKey("fishIdx") ? response.GetInt("fishIdx") : 1;
                    }
                    var user = response.GetSFSObject("user");
                    //user.ContainsKey("wcj") ? user.GetInt("wcj") : 0;
                    gdata.Winning = response.ContainsKey("caijin") ? response.GetInt("caijin") : 0;
                    gdata.Ante = user.GetInt("ante");
                    if (user.ContainsKey("ttgold"))
                    {
                        gdata.BetBehindMoney = user.GetLong("ttgold");
                        gdata.UserMoney = gdata.BetBehindMoney;
                    }
                    var gold = user.GetInt("gold");
                    gdata.Gold = gold;
                    gdata.WinGold += gold;
                    if (response.ContainsKey("rs"))
                    {
                        var random = new Random();
                        gdata.StarPos = random.Next(0, 28);
                        gdata.EndPos = response.GetInt("rs");
                    }
                    var time = response.GetInt("cd");
                    //计算此过程所需要的时间
                    TimerMgr.Wait(time);
                    //开始运行游戏
                    TurnGroupsMgr.PlayGame();
                    break;
            }
        }
    }
}
