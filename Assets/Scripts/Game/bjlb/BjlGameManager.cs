using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.bjlb
{
    public class BjlGameManager : YxGameManager
    {
        public UserListCtrl UserListCtrl;
        public BetCtrl BetCtrl;
        public ProgressCtrl ProgressCtrl;
        public CardsCtrl CardsCtrl;
        public ResultWindow ResultWin;
        public ShowNumCtrl ShowNumCtrl;
        public YxMessageTip TipWin;
        public YxWindow WaitWin;
        public UIToggle SoundToggle;
        public BankerCtrl BankerCtrl;
        protected override void OnStart()
        {
            base.OnStart();
            WaitWin.Show();
            ProgressCtrl.Hide();
            if(TipWin!=null) TipWin.Hide();
            if (SoundToggle != null)
            {
                SoundToggle.value = Facade.Instance<MusicManager>().MusicVolume>0;
            }
        }

        public void ResetGame()
        {
            var gdata = App.GetGameData<BjlGameData>();
            var cards = gdata.Cards;
            foreach (var temp in cards)
            {
                if (temp==null)continue;
                temp.SetActive(false);
                DestroyObject(temp);
            }
            cards.Clear();
            CardsCtrl.ReSet();
            BetCtrl.Init();
            ShowNumCtrl.ReSet();
        }

        public void OnQuitGame()
        {
            if (App.GetGameData<BjlGameData>().CouldOut)
            {

                YxMessageBox.DynamicShow(new YxMessageBoxData
                {
                    Msg = "确定要退出游戏吗!?",
                    IsTopShow = false,
                    Listener = (box, btnName) =>
                    {
                        if (btnName == YxMessageBox.BtnLeft)
                        {
                            App.QuitGame();
                        }
                    },
                    BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                });
            }
            else
            {
                YxMessageBox.DynamicShow(new YxMessageBoxData
                {
                    Msg = "正在游戏中,无法退出游戏!?",
                    IsTopShow = false,
                    Delayed = 5,
                });
            }
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<BjlGameData>();
            var forbinden = gameInfo.GetBool("forbiden");
            gdata.GetPlayerInfo<BjlUserInfo>().Forbiden = forbinden;
            if (App.GameKey.Equals("bjlb"))
            {
                int bankerLimit = gameInfo.GetInt("bankLimit");
                //初始化庄家控制
                BankerCtrl.Init(bankerLimit);
                UserListCtrl.RefreshBanker(gameInfo);
            }
            ShowNumCtrl.Init();
            CheckReJion(gameInfo);

            if (forbinden)
            {
                BetCtrl.HideChip();
                ShowTip();
            }
            BetCtrl.InitChips();
            OnGetPlayers(gameInfo);
            if (gameInfo.ContainsKey("state"))
            {
                gdata.SetGameStatus((YxEGameStatus)gameInfo.GetInt("state"));
            }
        }

        public void ShowTip()
        {
            BetCtrl.CurrentSelectChip = null;
            var tipData = new MessageTipData()
            { 
                Msg = App.GameData.GetPlayerInfo<BjlUserInfo>().Msg,
                Delayed = 3
            };
            App.GetGameManager<BjlGameManager>().TipWin.UpdateView(tipData);
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int status, ISFSObject info)
        {
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var gdata = App.GetGameData<BjlGameData>();
            switch (type)
            {
                case RequestType.Reward:
                    var self = gdata.GetPlayer();
                    self.Coin -= 100;
                    break;
                case RequestType.Bet:
                    BetCtrl.Bet(response);
                    ShowNumCtrl.RefreshNum(response);
                    break;
                case RequestType.BeginBet:
                    gdata.BeginBet = true;
                    ResetGame();
                    ProgressCtrl.PlayClock(response.ContainsKey("cd") ? response.GetInt("cd") : 12, SetStopBet);
                    //重新设置注的层
                    if (gdata.CurrentBanker!=null)
                    {
                        var banker = gdata.CurrentBanker;
                        banker.TotalCount++;
                        banker.UpdateView();

                        if (gdata.BankSeat < 0)
                        {
                            var bankerInfo = new BjlUserInfo
                            {
                                NickM = "系统庄",
                                Seat = -1,
                                CoinA = long.MaxValue,
                                TotalCount = 0,
                                WinTotalCoin = 0
                            };
                            gdata.CurrentBanker.Info = bankerInfo;
                            banker.UpdateView();
                        }

                        if (gdata.SelfSeat != gdata.BankSeat)
                        {
                            BetCtrl.ShowChip();
                        }
                        else
                        {
                            YxMessageBox.Show(new YxMessageBoxData { Msg = "您是本局的庄家!", Delayed = 2 });
                        }
                        BankerCtrl.RefreshBanker();
                    }
                    else
                    {
                        BetCtrl.ShowChip();
                    }

                    WaitWin.Hide();
                    break;
                case RequestType.EndBet:
                    SetStopBet();
                    gdata.BeginBet = false;
                    BetCtrl.HideChip();
                    WaitWin.Hide();
                    break;
                case RequestType.GiveCards:
                    ProgressCtrl.Hide();
                    CardsCtrl.BeginGiveCards(response);
                    ShowNumCtrl.GroupRefreshNum(response);
                    WaitWin.Hide();
                    break;
                case RequestType.Result:
                    gdata.SetGameStatus(YxEGameStatus.Normal);
                    gdata.IsGaming = false;
                    Result(response);
                    ResultWin.UpdateView(response);
                    ResultWin.Show();
                    gdata.GetPlayerInfo<BjlUserInfo>().SimpleParse(response);
                    gdata.GetPlayer().UpdateView();
                    if (App.GameKey.Equals("bjlb"))
                    {
                        BankerCtrl.SetApplyBankerBtnActive(false);
                    }
                    WaitWin.Hide();
                    break;
                case RequestType.BankerList:
                    if (App.GameKey.Equals("bjlb"))
                    {
                        UserListCtrl.RefreshBanker(response);
                    }
                    break;
                case RequestType.GroupBet:

                    BetCtrl.GroupBet(response);
                    ShowNumCtrl.GroupRefreshNum(response);
                    break;
                default:
                    YxDebug.Log("RequestType : " + type);
                    break;
            }
            OnGetPlayers(response);
        }

        public Talk TheTalk;
        private void Result(ISFSObject responseData)
        { 
            if (TheTalk == null) return;
            foreach (ISFSObject i in responseData.GetSFSArray("winplayers"))
            {
                var str = "[FFFFFF]" + i.GetUtfString("nick") + "[FFFFFF]赢了" + i.GetInt("win") + "[FFFFFF]金币。";
                TheTalk.AddTalkText(str, "全体成员");
            }
            if (responseData.ContainsKey("coupon") && responseData.GetInt("coupon") > 0)
            {
                var str = "[FFFFFF]恭喜您上局获得了[CCCC00]" + responseData.GetInt("coupon") + "[FF0000]奖券";
                TheTalk.AddTalkText(str, "全体成员");
            }
            if (responseData.ContainsKey("yqt") && responseData.GetInt("yqt") > 0)
            {
                var str = "[FFFFFF]恭喜您上局获得了[CCCC00]" + responseData.GetInt("yqt") + "[FF0000]银券通";
                TheTalk.AddTalkText(str, "全体成员");
            }
        }


        protected void SetStopBet()
        {
            ProgressCtrl.StopClock(RequestType.EndBet);
        }

        public void OnGetPlayers(ISFSObject responseData)
        {
            if (responseData.ContainsKey("playerlist"))
            {
                var playerStr = responseData.GetUtfStringArray("playerlist");
                UserListCtrl.RefreshPlayer(playerStr);
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
   

        protected virtual void CheckReJion(ISFSObject requestData)
        {
            var st = requestData.GetLong("st");
            var ct = requestData.GetLong("ct");
            if (st != 0)
            {
                var gdata = App.GetGameData<BjlGameData>();
                if (ct - st < 12)
                {
                    gdata.BeginBet = true;
                    //                    App.GameData.GStatus = GameStatus.PlayAndConfine;

                    //限制自己为庄家时下注
                    if (gdata.SelfSeat != gdata.BankSeat || App.GameKey.Equals("bjl"))
                    {
                        BetCtrl.ShowChip();
                    }
                    WaitWin.Show();
                }
                ShowNumCtrl.SetNum(requestData); 
                //todo 初始化已下的筹码
            }
        }

        public void OnChangeSound(bool toggle)
        {
            Facade.Instance<MusicManager>().OnOffVolume(toggle);
        }
    }
}
