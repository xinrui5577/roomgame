using System;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Manager;
using YxFramwork.View;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.BaiTuan
{
    public class BtwGameManager : YxGameManager
    {
        public UserListCtrl UserListCtrl;
        public BetCtrl BetCtrl;
        public ProgressCtrl ProgressCtrl;
        public CardsCtrl CardsCtrl;
        public ResultWindow ResultWin;
        public ShowNumCtrl ShowNumCtrl;
        public ResultListCtrl ResultListCtrl;
        public ApplyCtrl ApplyCtrl;
        public YxWindow WaitWin;
        public GameObject TimeBet;
        protected void Awake()
        {
            Application.targetFrameRate = 20;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        public void ReSetGame()
        {
            var gdata = App.GetGameData<BtwGameData>();
            foreach (var temp in gdata.CurrentChipList)
            {
                temp.SetActive(false);
                DestroyObject(temp);

            }
            gdata.CurrentChipList.Clear();
            foreach (var temp in gdata.CurrentCardList)
            {
                temp.SetActive(false);
                DestroyObject(temp);

            }
            gdata.CurrentCardList.Clear();
            foreach (var temp in gdata.DownCardArray)
            {
                if (temp != null)
                {
                    temp.SetActive(false);
                    DestroyObject(temp);
                }

            }
            foreach (var temp in gdata.TopCardArray)
            {
                if (temp != null)
                {
                    temp.SetActive(false);
                    DestroyObject(temp);
                }

            }
            foreach (var temp in gdata.LeftCardArray)
            {
                if (temp != null)
                {
                    temp.SetActive(false);
                    DestroyObject(temp);
                }

            }
            foreach (var temp in gdata.RightCardArray)
            {
                if (temp != null)
                {
                    temp.SetActive(false);
                    DestroyObject(temp);
                }

            }
            BetCtrl.Init();
        }

        public void RefreshUserInfo()
        {
            var gdata = App.GetGameData<BtwGameData>();
            var self = gdata.GetPlayer();
            self.UpdateView();
            gdata.ThisCanInGold = self.Coin;
        }

        public override void OnQuitGameClick()
        {
            if (App.GetGameData<BtwGameData>().IsBanker)
            {

                YxMessageBox.Show(new YxMessageBoxData
                {
                    Msg = "团长同志！战斗还没有结束，您不可以临阵脱逃！",
                    Delayed = 5,
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
                return;
            }
            base.OnQuitGameClick();
        }


        public override void OnGetGameInfo(ISFSObject requestData)
        {
            ShowNumCtrl.Init();
            UserListCtrl.RefreshBanker(requestData);
            CheckReJion(requestData);
            ProgressCtrl.SetNum(requestData);
            RefreshUserInfo();
            BetCtrl.InitChips();
            if (requestData.ContainsKey("bround"))
            {
                App.GetGameData<BtwGameData>().BankerPlayer.SetBankerTime(requestData.GetInt("bround"));
            }
        }

        protected void CheckReJion(ISFSObject requestData)
        {
            var st = requestData.GetLong("st");
            var ct = requestData.GetLong("ct");
            if (st != 0)
            {
                if (ct - st < 15)
                {
                    CardsCtrl.ReSetPonits();
                    App.GetGameData<BtwGameData>().BeginBet = true;
                    BetCtrl.ShowChip();
                }
                ShowNumCtrl.SetNum(requestData);
                WaitWin.Show();
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
            if (response.ContainsKey("playerlist") && !response.IsNull("playerlist"))
            {
                var playerStr = response.GetUtfStringArray("playerlist");
                UserListCtrl.RefreshPlayer(playerStr);
            }
            var gdata = App.GetGameData<BtwGameData>();
            switch ((BtwRequestType)type)
            {
                case BtwRequestType.Bet:
                    YxDebug.Log("-----------------下注--------------");
                    WaitWin.Hide();
                    BetCtrl.Bet(response);
                    ShowNumCtrl.RefreshNum(response);
                    break;
                case BtwRequestType.BankerList:
                    YxDebug.Log("-----------------上庄列表--------------");
                    WaitWin.Hide();
                    UserListCtrl.RefreshBanker(response);
                    ProgressCtrl.SetNum(response);
                    BetCtrl.ShowChip();
                    break;
                case BtwRequestType.BeginBet:
                    YxDebug.Log("-----------------开始下注--------------");
                    WaitWin.Hide();
                    gdata.BeginBet = true;
                    CardsCtrl.ReSetPonits();
                    CardsCtrl.gameObject.SetActive(false);
                    ShowNumCtrl.Reset();
                    ReSetGame();
                    BetCtrl.ShowChip();
                    ProgressCtrl.ReSetCountdown(15);
                    ProgressCtrl.BeginCountdown();
                    ApplyCtrl.RefreshBanker();
                    TimeBet.gameObject.SetActive(true);
                    YxMessageTip.Show("开始下注");
                    break;
                case BtwRequestType.EndBet:
                    YxDebug.Log("-----------------停止下注--------------");
                    gdata.BeginBet = false;
                    BetCtrl.HideChip();
                    ProgressCtrl.EndCountdown();
                    TimeBet.gameObject.SetActive(false);
                    WaitWin.Hide();
                    break;
                case BtwRequestType.GiveCards:
                    YxDebug.Log("-----------------发牌--------------");
                    CardsCtrl.BeginGiveCards(response);
                    CardsCtrl.gameObject.SetActive(true);
                    WaitWin.Hide();
                    break;
                case BtwRequestType.Result:
                    YxDebug.Log("-----------------结算--------------");
                    gdata.GStatus = YxEGameStatus.Normal;
                    if (ResultWin != null) ResultWin.ShowWithData(response);
                    UserListCtrl.RefreshBanker(response);
                    ProgressCtrl.SetNum(response);
                    ApplyCtrl.HideApplyBanker();
                    WaitWin.Hide();
                    break;
            }
        }

        public override void BeginNewGame(ISFSObject sfsObject)
        {
            base.BeginNewGame(sfsObject);
            WaitWin.Hide();
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