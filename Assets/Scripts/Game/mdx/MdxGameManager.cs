using System;
using System.Collections;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;
using UnityEngine;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.mdx
{
    public class MdxGameManager : YxGameManager
    {
        public BetCtrl BetCtrl;
        public ProgressCtrl ProgressCtrl;
        //public CardsCtrl CardsCtrl;
        public ResultWindow ResultWin;
        public ShowNumCtrl ShowNumCtrl;
        public YxMessageTip TipWin;
        public PlayerBetListCtrl PlayerBetListCtrl;
        public ResultListCtrl ResultListCtrl;
        public RoomInfo RoomInfo;
        public BankerTip BankerTip;
        public TipStringFormat TipStringFormat;

        public RockDices RockDices;
        protected override void OnStart()
        {
            base.OnStart();
            ProgressCtrl.Hide();
            if(TipWin!=null) TipWin.Hide();
        }

        

        public void ResetGame()
        {
            var gdata = App.GetGameData<MdxGameData>();
            var cards = gdata.Cards;
            foreach (var temp in cards)
            {
                if (temp==null)continue;
                temp.SetActive(false);
                DestroyObject(temp);
            }
            cards.Clear();
            BetCtrl.Init();
            RockDices.Reset();
            ResultWin.Reset();
            StopAllCoroutines();
            PlayerBetListCtrl.CleanView();
        }

        public void OnQuitGame()
        {
            if (App.GetGameData<MdxGameData>().CouldOut)
            {
                YxMessageBox.DynamicShow(new YxMessageBoxData
                {
                    Msg = TipStringFormat.SureAboutQuit,
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
                    Msg = TipStringFormat.IsInGameing,
                    IsTopShow = false,
                    Delayed = 5,
                });
            }
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<MdxGameData>();
            var forbinden = gameInfo.GetBool("forbiden");
            gdata.GetPlayerInfo<MdxUserInfo>().Forbiden = forbinden;
            if (gameInfo.ContainsKey("bankLimit"))
            {
                int bankerLimit = gameInfo.GetInt("bankLimit");
                //初始化庄家控制
                gdata.MinApplyBanker = bankerLimit;
            }
            if (gameInfo.ContainsKey("bankLimitMax"))
            {
                int bankerLimit = gameInfo.GetInt("bankLimitMax");
                gdata.MaxApplyBanker = bankerLimit;
            }

            CheckReJion(gameInfo);

            if (forbinden)
            {
                BetCtrl.HideChip();
                ShowTip();
            }
            BetCtrl.InitChips();
            ShowNumCtrl.Init();
            if (gameInfo.ContainsKey("banker"))
            {
                int seat = gameInfo.GetInt("banker");
                int allowTotal = seat < 0 ? int.MaxValue : gameInfo.GetInt("allowTotal");
                SetBankerInfo(seat, allowTotal);
                ShowNumCtrl.OnChangeBanker(allowTotal);
            }
            if (gameInfo.ContainsKey("state"))
            {
                gdata.SetGameStatus((YxEGameStatus)gameInfo.GetInt("state"));
            }

            if (gameInfo.ContainsKey("bet"))
            {
                var bet = gameInfo.GetIntArray("bet");
                int len = bet.Length;
                for (int i = 0; i < len; i++)
                {
                    ShowNumCtrl.SetLabels(i, bet[i]);
                }
            }
            if (gameInfo.ContainsKey("users"))
            {
                BetCtrl.ProcessUsersInfo(gameInfo.GetSFSArray("users"));
            }
            if (gameInfo.ContainsKey("ct"))
            {
                int cd = GetCdTime(gameInfo);
                int status = gameInfo.GetInt("status");
                ProgressCtrl.PlayClock(GetTypeString(status), cd);
            }

            ResultListCtrl.GetHistoryInfo(gameInfo);
            RoomInfo.OnGetGameInfo(gameInfo);
        }

        int GetCdTime(ISFSObject gameInfo)
        {
            int dif = (int) (gameInfo.GetLong("ct") - gameInfo.GetLong("st"));
            int cd = gameInfo.GetInt("cd");
            return cd - dif;
        }

        string GetTypeString(int type)
        {
            switch (type)
            {
                case 3:
                    return "开始下注";
                case 4:
                    return "正在摇奖";
                case 5:
                    return "正在结算";
                default:
                    return "开始抢庄";
            }
        }

        

        void SetBankerInfo(int seat,int maxBet)
        {
            var gdata = App.GetGameData<MdxGameData>();
            if (gdata.BankSeat == seat) return;
            gdata.BankSeat = seat;
            if (seat >= 0)
            {
                var userInfo = gdata.GetPlayerInfo(seat, true);
                var banker = gdata.CurrentBanker;
                banker.StopWaiting();
                if (userInfo == null)
                {
                    SetSystemBanker(GetRandomName());
                    return;
                }
                var bankerInfo = new YxBaseGameUserInfo
                {
                    NickM = userInfo.NickM,
                    CoinA = maxBet,
                    AvatarX = userInfo.AvatarX,
                    Seat = seat
                };
                banker.Info = bankerInfo;
                banker.gameObject.SetActive(true);
                if (seat == gdata.SelfSeat)
                {
                    gdata.SetGameStatus(YxEGameStatus.PlayAndConfine);
                    gdata.GetPlayer<MdxPlayer>().Coin -= maxBet;
                }
            }
            else
            {
                SetSystemBanker("等待抢庄");
            }
        }
       
        private string GetRandomName()
        {
            System.Random random = new System.Random();
            int nameLength = random.Next(5, 9);
            char[] charArray = new char[nameLength]; 
            for (int i = 0; i < nameLength; i++)
            {
                charArray[i] = (char)random.Next(97, 122);
            }
            return new string(charArray);
        }

        void SetSystemBanker(string bankerName)
        {
            var gdata = App.GetGameData<MdxGameData>();
            var banker = gdata.CurrentBanker;
            if (gdata.BankSeat < 0)
            {
                banker.NameLabel.Text(bankerName);
                banker.CoinLabel.Text("--");
                banker.HeadPortrait.SetTexture(null);
                //gdata.CurrentBanker.Info = bankerInfo;
                banker.gameObject.SetActive(true);
                banker.PlayWait();
            }
        }

        public void ShowTip()
        {
            BetCtrl.CurrentSelectChip = null;
            var tipData = new MessageTipData()
            { 
                Msg = App.GameData.GetPlayerInfo<MdxUserInfo>().Msg,
                Delayed = 3
            };
            App.GetGameManager<MdxGameManager>().TipWin.UpdateView(tipData);
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int status, ISFSObject info)
        {
        }

        public override void BeginNewGame(ISFSObject sfsObject)
        {
            base.BeginNewGame(sfsObject);
            RoomInfo.Reset();
            
            ShowNumCtrl.ReSet();

        }

        private bool _showBankerTip;
        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var gdata = App.GetGameData<MdxGameData>();
            switch (type)
            {
                case RequestType.ChangeBanker:
                    int bankSeat = response.GetInt("banker");

                    int maxBet = bankSeat < 0 ? int.MaxValue : response.GetInt("allowTotal");
                    SetBankerInfo(bankSeat, maxBet);
                    ShowNumCtrl.OnChangeBanker(maxBet);

                    if (_showBankerTip)
                    {
                        BankerTip.ShowBankerTip();
                        _showBankerTip = false;
                    }
                    break;
                case RequestType.Reward:
                    var self = gdata.GetPlayer();
                    self.Coin -= 100;
                    break;
                case RequestType.Bet:
                    BetCtrl.Bet(response);
                    PlayerBetListCtrl.AddBetInfo(response);
                    RoomInfo.OnPlayerBet();
                    ShowNumCtrl.SetOneBet(response);
                    break;
                case RequestType.BeginBet:
                    gdata.SetGameStatus(YxEGameStatus.Play);
                    gdata.BeginBet = true;
                    ProgressCtrl.PlayClock("开始下注",response.ContainsKey("cd") ? response.GetInt("cd") : 12, SetStopBet);
                    
                    if (gdata.CurrentBanker!=null)
                    {
                        if (gdata.SelfSeat != gdata.BankSeat)
                        {
                            BetCtrl.ShowChipBtns();
                        }
                        else
                        {
                            BetCtrl.ShowBankerMark();
                            YxMessageBox.Show(new YxMessageBoxData { Msg = TipStringFormat.IsBankerNoBet , Delayed = 2 });
                        }
                    }
                    else
                    {
                        BetCtrl.ShowChip();
                    }

                    ResetGame();        //重置了庄家信息后,重置游戏
                    break;
                case RequestType.EndBet:
                    SetStopBet();
                    gdata.BeginBet = false;
                    BetCtrl.HideChip();
                    ProgressCtrl.PlayClock("停止下注", 0);
                    break;
                case RequestType.RockDices:
                    ProgressCtrl.Hide();
                    gdata.DiceVals = response.GetIntArray("dices");
                    RockDices.ShowRockDices();
                    ResultWin.Hide();
                    ProgressCtrl.PlayClock("正在摇奖", response.GetInt("cd"));
                    break;
                case RequestType.Result:
                    //游戏状态设置
                    gdata.SetGameStatus(YxEGameStatus.Normal);
                    gdata.IsGaming = false;
                    ProgressCtrl.PlayClock("正在结算", 0);

                    //显示结算
                    ResultWin.UpdateView(response);
                    ResultWin.Show();

                    ResultListCtrl.AddResult(response);
                    //刷新玩家
                    gdata.GetPlayerInfo<MdxUserInfo>().SimpleParse(response);
                    gdata.GetPlayer().UpdateView();
                    //隐藏动画
                    RockDices.Hide();
                    //添加结算数据
                    PlayerBetListCtrl.GetResultPlayerInfo(response);
                    StartCoroutine(MoveAllChip(response.GetInt("win")));
                    break;
                case RequestType.BankerList:
                    ProgressCtrl.PlayClock("正在摇奖", response.GetInt("cd"));
                    break;
                case RequestType.GroupBet:
                    BetCtrl.GroupBet(response);
                    PlayerBetListCtrl.AddGroupBetInfo(response);
                    RoomInfo.OnGroupBet(response);
                    ShowNumCtrl.SetGroupBet(response);
                    break;
                case RequestType.WinBankerTime:
                    _showBankerTip = true;
                    ProgressCtrl.PlayClock("开始抢庄", response.GetInt("cd"));
                    BetCtrl.ShowBankerMark();
                    break;
                default:
                    YxDebug.Log("RequestType : " + type);
                    break;
            }
        }

        private IEnumerator MoveAllChip(int win)
        {
            yield return new WaitForSeconds(3f);
            BetCtrl.MoveAllBet();
            App.GameData.GetPlayer<MdxPlayer>().ShowSelfWinLabel(win);
        }


        protected void SetStopBet()
        {
            ProgressCtrl.StopClock(RequestType.EndBet);
        }
       

        public override void UserOut(int localSeat, ISFSObject responseData)
        {
            RoomInfo.OnUserOut();
        }

        public override void OnOtherPlayerJoinRoom(ISFSObject sfsObject)
        {
            base.OnOtherPlayerJoinRoom(sfsObject);
            RoomInfo.OnPlayerJoinIn();
        }

        public override void UserIdle(int localSeat, ISFSObject responseData)
        {
        }

        public override void UserOnLine(int localSeat, ISFSObject responseData)
        {
        }


        protected virtual void CheckReJion(ISFSObject requestData)
        {
            int cd = requestData.GetInt("cd");
            var st = requestData.GetLong("st");
            var ct = requestData.GetLong("ct");
            if (st != 0)
            {
                var gdata = App.GetGameData<MdxGameData>();
                if (ct - st < cd)
                {
                    gdata.BeginBet = true;
                }
            }
        }

        public void OnChangeSound(bool toggle)
        {
            Facade.Instance<MusicManager>().OnOffVolume(toggle);
        }
    }

    [Serializable]
    public class TipStringFormat
    {
        public string BaoZiFormat = "豹子";
        public string NoEnoughGold = "金币不足不能下注";
        public string BetOneSide = "押注只能押一边";
        public string BetOutLimit = "超出上限，无法下注.";
        public string BankErrorInput = "输入有误,请重新输入!!";
        public string SureAboutQuit = "确定要退出游戏吗!?";
        public string IsInGameing = "正在游戏中,无法退出游戏!!";
        public string IsBankerNoBet = "您是本局的庄家!";
        public string ChoiseFastBet = "您选择了快押,会按可下注的最大限额下注，是否确定";
        public string NoBank = "没有庄家,无法下注,请等待玩家上庄";
        public string NoEnoughGoldForBank = "您输入的数额超过了您的筹码,请确认后重新输入";
        public string InputTooLongForBank = "抢庄金额不能大于 {0:N0}";
    }


}
