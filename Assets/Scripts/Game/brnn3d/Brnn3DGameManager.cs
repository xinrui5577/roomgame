using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.brnn3d
{
    public class Brnn3DGameManager : YxGameManager
    {
        public ApplyXiaZhuangMgr TheApplyXiaZhuangMgr;
        public CameraMove_3D TheCameraMove3D;
        public DownUIController TheDownUICtrl;
        public BetModeMgr TheBetModeMgr;
        public BeiShuMode TheBeiShuMode;
        public MidUIController TheMidUICtrl;
        public CardMachine TheCardMachine;
        public UpUIController TheUpUICtrl;
        public DicMode TheDicMode;
        public PaiMode ThePaiMode;
        public NoteUI TheNoteUI;//todo 后期替换成统一的
        public ZhongJiangMode TheZhongJiangMode;
        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            var ratecfg = gameInfo.GetUtfString("ratecfg");
            if (!string.IsNullOrEmpty(ratecfg)) TheBeiShuMode.SetRateInfo(ratecfg.Split('#'));
            App.GameData.GetPlayer().UpdateView();
            TheUpUICtrl.TheBankersManager.SetBankerInfoUIData();//设置庄家的信息
            TheCameraMove3D.CameraMoveByPath(0);//3D摄像机的移动到桌面上
            TheDownUICtrl.ResetChip();
        }
         
        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int status, ISFSObject info)
        {
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var gdata = App.GetGameData<Brnn3dGameData>();
            switch (type)
            {
                //下注阶段1
                case RequestType.Bet:
                    Facade.Instance<MusicManager>().Play("GameBegin");
                    var seat = response.GetInt("seat");
                    gdata.BetPos = response.GetInt("p");
                    gdata.BetMoney = response.GetInt("gold");

                    TheApplyXiaZhuangMgr.SetApplayXiaZhuangUIData();//设置申请上下装按钮的状态

                    TheBetModeMgr.SetBetModeChouMaData(seat);//接收初始化玩家投注筹码的消息
                    if (gdata.SelfSeat == seat)
                    {
                        App.GameData.GetPlayer().UpdateView();
                    }
                    break;
                //判断庄家阶段4
                case RequestType.BankerList:
                    gdata.BankList = response.GetSFSArray("bankers");
                    gdata.B = response.GetInt("banker");
                    TheUpUICtrl.TheBankersManager.TheBankerListUI.DeleteBankerListUI();//更新前先删除
                    TheUpUICtrl.TheBankersManager.SetBankerInfoUIData();//设置庄家的信息

                    TheApplyXiaZhuangMgr.SetApplayXiaZhuangUIData();//设置申请上下装按钮的状态

                    //CountDownUI.Instance.SetNum(); //设置下注的最大值

                    break;
                //开始下注阶段5
                case RequestType.BeginBet:
                    gdata.Bundle++;
                    App.GameData.GStatus = gdata.SelfSeat == gdata.B ? YxEGameStatus.PlayAndConfine : YxEGameStatus.Normal;
                    TheUpUICtrl.TheGamesNumUI.SetGamesNumUI();//设置游戏运行的局数
                    gdata.IsBet = true;
                    TheUpUICtrl.TheStateUI.SetStateUI(2);//游戏运行的阶段显示，此时为下注
                    Facade.Instance<MusicManager>().Play("xiazhubeijing");
                    TheApplyXiaZhuangMgr.SetApplayXiaZhuangUIData();//设置申请上下装按钮的状态

                    TheCameraMove3D.CameraMoveByPath(0);//3D摄像机的移动到桌面上

                    TheMidUICtrl.TheNiuNumberUI.HideNiuNumberUI(); //隐藏牛数

                    ThePaiMode.DeletPaiList(); //清空牌的列表

                    TheUpUICtrl.TheCountDownUI.SetTimeCount(16);//时间的控制

                    TheDownUICtrl.DownUILeftUIOn_OffClicked(true);//隐藏历史纪录的面板


                    break;
                //结束下注阶段6
                case RequestType.EndBet:
                    gdata.IsBet = false;
                    TheMidUICtrl.TheBetMoneyUI.BetMoneyQingKongInfo();//隐藏下注界面
                    break;


                //发牌阶段7
                case RequestType.GiveCards:
                    gdata.IsBet = false;
                    if (response.ContainsKey("total"))
                    {
                        gdata.GetPlayerInfo().CoinA = response.GetLong("total");
                    }

                    gdata.Cards = response.GetSFSArray("cards");
                    gdata.Nn = response.GetSFSArray("nn");
                    TheUpUICtrl.TheStateUI.SetStateUI(3);//游戏运行的阶段显示，此时为开牌
                    TheApplyXiaZhuangMgr.SetApplayXiaZhuangUIData();//设置申请上下装按钮的状态


                    gdata.DicNum = response.GetInt("dice");
                    gdata.BankList = response.GetSFSArray("bankers");
                    gdata.B = response.GetInt("banker");
                    gdata.SendCardPosition = gdata.DicNum - 1;

                    TheDicMode.PlayDic(); //扔骰子并显示点数 
                    TheDicMode.StopDic(); //骰子消失

                    ThePaiMode.BeginGiveCards(); //开始发牌


                    break;
                //结束阶段8
                case RequestType.Result:
                    gdata.GStatus = gdata.SelfSeat == gdata.B ? YxEGameStatus.PlayAndConfine : YxEGameStatus.Normal;
                    Facade.Instance<MusicManager>().Play("GameOver");
                    gdata.IsBet = false;
                    gdata.BankList = response.GetSFSArray("bankers");
                    gdata.B = response.GetInt("banker");
                    var self = gdata.GetPlayerInfo<Brnn3DUserInfo>();
                    var winCoin = response.ContainsKey("win") ? response.GetInt("win") : 0;
                    self.WinCoin = winCoin;
                    self.WinTotalCoin += winCoin;
                    var banker = TheUpUICtrl.TheBankersManager.Banker.GetInfo<Brnn3DUserInfo>();
                    if (banker != null)
                    {
                        var win = response.ContainsKey("bwin") ? response.GetLong("bwin") : 0;
                        banker.WinCoin = win;
                        banker.WinTotalCoin += win;
                    }
                    
                    TheUpUICtrl.TheStateUI.SetStateUI(4);//游戏运行的阶段显示，此时为结算
                    TheCameraMove3D.Move();

                    gdata.IsOut = true;
                    gdata.GStatus = YxEGameStatus.Normal;
                    //CountDownUI.Instance.SetNum();//设置下注的最大值
                    break;
            }
        }

        public override void UserOut(int localSeat, ISFSObject responseData)
        {
        }

        public override void UserIdle(int localSeat, ISFSObject responseData)
        {
        }

        public override void UserOnLine(int localSeat, ISFSObject responseData)
        {
        }
    }
}
