using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.bjl3d
{
    public class Bjl3DGameManager : YxGameManager
    {
        public BankerInfoUI TheBankerInfoUI;
        public BetHowMuchPromptUI TheBetHowMuchPromptUI;
        public BetMoneyUI TheBetMoneyUI;
        public CameraMgr TheCameraMgr;
        public CoinTypeInfoUI TheCoinTypeInfoUI;
        public CountDownUI TheCountDownUI;
        public GameScene TheGameScene;
        public GameUI TheGameUI;
        public LuziInfoUI TheLuziInfoUI;
        public PaiPathScene ThePaiPathScene;
        public PlanScene ThePlanScene;
        public StateUI TheStateUI;
        public UerInfoCountDownLuziUI TheUerInfoCountDownLuziUI;
        public WaitForRankerListUI TheWaitForRankerListUI;
        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<Bjl3DGameData>();
            var self = gdata.GetPlayer();
            TheCoinTypeInfoUI.InitChipBtns(gdata.AnteRate);
            CheckReJion(gameInfo);//检查重连
            self.UpdateView();
            TheBankerInfoUI.ShowUserInfoUI();//设置庄家的信息
            TheBetHowMuchPromptUI.HowMuchPrompt();//初始化筹码面板上的可下注钱数
            TheBetHowMuchPromptUI.SetLuziInfo(gdata.History);//设置游戏的历史纪录信息
            TheBetHowMuchPromptUI.BottomLuzi();// 游戏底部的初始化历史纪录显示
            TheLuziInfoUI.ShowHistoryEx();//初始化的时候显示历史记录的效果
            TheBankerInfoUI.GameInnings();//面板显示游戏运行多少局
            gdata.GameConfig.isXianshi = false;
        }

        public void CheckReJion(ISFSObject requestData)
        {
            var st = requestData.GetLong("st");
            var ct = requestData.GetLong("ct");
            if (st != 0)
            {
                if (ct - st < 14)
                {
                    App.GetGameData<Bjl3DGameData>().GameConfig.GameState = 5;
                    TheStateUI.StateShow(5);
                }
            }
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int status, ISFSObject info)
        {
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var gdata = App.GetGameData<Bjl3DGameData>();
            var gameMgr = App.GetGameManager<Bjl3DGameManager>();
            var self = gdata.GetPlayer<Bjld3DPlayer>();
            switch (type)
            {
                //类型1 下注阶段
                case RequestType.Bet:
                    gdata.P = response.GetInt("p");
                    var goldOne = response.GetInt("gold");
                    var userSeat = response.GetInt("seat");
                    if (response.ContainsKey("allow"))
                    {
                        gdata.Allow = response.GetIntArray("allow");
                    }
                    ThePlanScene.UserNoteDataFun(userSeat, goldOne);//接收玩家的下注信息
                    TheGameScene.UserNoteDataFun(userSeat, goldOne);//下注位置特效的显示
                    self.UpdateView(); //显示游戏面板玩家自己的信息
                    TheBetHowMuchPromptUI.HowMuchPrompt();//正常接收筹码面板可下注数
                    break;
                //类型3 申请上庄
                case RequestType.ApplyBanker:
                    break;
                //类型4 申请下庄
                case RequestType.ApplyQuit:
                    break;
                //类型5 开始下注
                case RequestType.BeginBet:
                    if (gdata.GameConfig.isXianshi)
                    {
                        TheBankerInfoUI.GameInnings();//面板显示游戏运行多少局
                    }
                    gdata.GameConfig.isXianshi = true;
                    gdata.GameConfig.GameState = 5;
                    TheStateUI.StateShow(5);
                    gameMgr.TheCountDownUI.NoticeXiaZhuFun();//游戏开始的时候时间的倒计时
                    break;
                //类型6 结束下注
                case RequestType.EndBet:
                    gdata.GameConfig.GameState = 6;
                    break;
                //类型7 发牌阶段
                case RequestType.GiveCards:
                    gdata.GameConfig.GameState = 7;
                    TheStateUI.StateShow(7);
                    ISFSObject xian = response.GetSFSObject("xian");
                    gdata.XianCards = xian.GetIntArray("cards");
                    gdata.XianValue = xian.GetInt("value");

                    ISFSObject zhuang = response.GetSFSObject("zhuang");
                    gdata.ZhuangCards = zhuang.GetIntArray("cards");
                    gdata.ZhuangValue = zhuang.GetInt("value");

                    gameMgr.TheCountDownUI.SendCardFun();//发牌阶段的处理操作
                    ThePaiPathScene.SendCardFun();//开始发牌
                    TheBetHowMuchPromptUI.BottomLuzi();//历史纪录的消息
                    break;
                //类型8 游戏结束
                case RequestType.Result:
                    gdata.GameConfig.GameState = 8;
                    App.GameData.GStatus = YxEGameStatus.Normal;
                    gdata.Win = response.GetInt("win");//显示自己输赢
                    gdata.Total = response.GetLong("total");//当前金币数量
                    gdata.GetPlayerInfo().CoinA = App.GetGameData<Bjl3DGameData>().Total;
                    gdata.BetMoney = response.GetIntArray("pg");//结算的时候面板显示各个下注位置的钱数
                    gdata.BetJiesuan = response.GetIntArray("wp");//结算的时候面板显示各个下注位置的输赢
                    gdata.TodayWin = response.GetInt("todayWin");//今天输赢情况

                    TheGameScene.ClearBetEffect();
                    gameMgr.TheCountDownUI.ShowWinAreasFun();//显示游戏结束后的中奖区域
                    gameMgr.TheCountDownUI.GameResultFun();//打开下拉菜单
                    TheLuziInfoUI.ShowHistoryEx();//显示历史记录的效果
                    TheBetHowMuchPromptUI.Data();//刷新游戏的历史纪录的点数
                    TheGameUI.GameResult();//显示比赛结果

                    break;
                //类型9 庄家列表
                case RequestType.BankerList:
                    gdata.GameConfig.GameState = 9;
                    gdata.ResultBnakerTotal = response.GetLong("bankTotal");
                    TheWaitForRankerListUI.RankerListData();//先清理庄家列表
                    gdata.BankList = response.GetSFSArray("bankers");
                    gdata.B = response.GetInt("banker");
                    TheBankerInfoUI.ShowUserInfoUI();//刷新庄家的信息
                    break;
            }
        }
         
    }
}
