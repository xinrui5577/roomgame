using UnityEngine;
using System.Collections;
using YxFramwork.Common;
using YxFramwork.View;
using YxFramwork.Enums;
using Sfs2X.Entities.Data;
using YxFramwork.Manager;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.bjlb.bjlb_skin02
{
    public class BjlGameManager02 : BjlGameManager
    {
        public BankerTime02 BankerTime;

        public CardsCtrl02 CardsCtrl02;

        public TableResultInfo TableResultInfo;

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            base.OnGetGameInfo(gameInfo);

            if (App.GameKey.Equals("bjlb"))
            {
                int bankerLimit = gameInfo.GetInt("bankLimit");
                //初始化庄家控制
                BankerCtrl.Init(bankerLimit);
                UserListCtrl.RefreshBanker(gameInfo);
                ((UserListCtrl02)UserListCtrl).InitBankerLimit(bankerLimit);
            }

            var gdata = App.GetGameData<BjlGameData>();

            if (gameInfo.ContainsKey("record"))
            {
                var recordArray = gameInfo.GetIntArray("record");
                var trendcfg = gdata.TrendConfig;
                foreach (var trend in recordArray)
                {
                    trendcfg.AddTrend(trend);
                }
                trendcfg.TrendView.UpdateView();
            }
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var gdata = App.GetGameData<BjlGameData>();
            switch (type)
            {
                case RequestType.BeginBet:
                    Reset();
                    if (App.GameKey.Equals("bjlb"))
                    {
                        BankerCtrl.SetApplyBankerBtnActive(true);
                    }
                    gdata.BeginBet = true;
                    ((ProgressCtrl02)ProgressCtrl).PlayClock(response.ContainsKey("cd") ? response.GetInt("cd") : 12, RequestType.BeginBet, SetStopBet);
                    //重新设置注的层
                    if (gdata.CurrentBanker != null)
                    {
                        var curBanekr = gdata.CurrentBanker;
                        curBanekr.TotalCount++;
                        curBanekr.UpdateView();
                        //限制自己为庄家时下注
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
                            curBanekr.UpdateView();
                        }
                        
                        BankerCtrl.RefreshBanker();
                        
                        if (gdata.SelfSeat != gdata.BankSeat)
                        {
                            BetCtrl.ShowChip();
                        }
                        else
                        {
                            YxMessageBox.Show(new YxMessageBoxData { Msg = "您是本局的庄家!", Delayed = 2 });
                        }
                    }
                    else
                    {
                        BetCtrl.ShowChip();
                    }

                    if (response.ContainsKey("bankRound"))
                    {
                        BankerTime.SetBankerTime(response.GetInt("bankRound"));
                    }
                    else
                    {
                        BankerTime.HideBankerTime();
                    }
                    WaitWin.Hide();
                    Facade.Instance<MusicManager>().Play("BeginBet");
                    break;

                case RequestType.EndBet:
                    base.GameResponseStatus(type, response);
                  
                    ProgressCtrl.StopClock(RequestType.EndBet);
                    Facade.Instance<MusicManager>().Play("StopBet");

                    if (App.GameKey.Equals("bjlb"))
                    {
                        BankerCtrl.SetApplyBankerBtnActive(false);
                    }
                    break;

                case RequestType.Result:
                    gdata.SetGameStatus(YxEGameStatus.Normal);
                    gdata.IsGaming = false;
                    var hisData = gdata.TrendConfig.HistoryData;
                    int count = hisData.Count;
                    if (count > 0)
                    {
                        int winIndex = GetMenIndex(hisData[count - 1]);
                        var bpg = response.GetIntArray("bpg");
                        ResultBet(winIndex, bpg);
                        StartCoroutine(ResultMoveChip(winIndex,bpg));
                    }


                    ProgressCtrl.StopClock(RequestType.Result);
                    UserListCtrl.RefreshBanker(response);
                    RefreshSelf(response);
                    TableResultInfo.ShowTableResultInfo(response);
                    CardsCtrl02.SetCardCtrlActive(false);
                    break;
                case RequestType.GiveCards:

                    if (response.ContainsKey("cd"))
                    {
                        ((ProgressCtrl02)ProgressCtrl).PlayClock(response.ContainsKey("cd") ? response.GetInt("cd") : 12, RequestType.GiveCards);
                    }
                    else
                    {
                        ProgressCtrl.Hide();
                    }

                    CardsCtrl02.BeginGiveCards(response);
                    ShowNumCtrl.GroupRefreshNum(response);
                    WaitWin.Hide();

                    break;

                default:
                    base.GameResponseStatus(type, response);
                    break;
            }
        }

        private int GetMenIndex(int p)
        {
            if (p > 0)  //龙
            {
                return 0;
            }
            return p < 0 ? 1 : 2;
        }


        /// <summary>
        /// 庄家补全筹码
        /// </summary>
        /// <param name="winIndex">补全的门</param>
        /// <param name="resultArray">每门的输赢数组</param>
        private void ResultBet(int winIndex, int[] resultArray)
        {
            if (resultArray == null) return;
            int len = resultArray.Length;
            if (len <= 0) return;

            BetCtrl.ThrowChips(-resultArray[winIndex], winIndex, true);
        }

        IEnumerator ResultMoveChip(int winIndex,int[] bpg)
        {
            yield return new WaitForSeconds(1f);
            ResultMoveChips(winIndex, bpg);
        }


        void ResultMoveChips(int winIndex,int[] bpg)
        {
            var bet02 = (BetCtrl02)BetCtrl;
            var menBet = ShowNumCtrl.ZBet;

            for (int i = 0; i < 3; i++)
            {
                if (menBet[i] <= 0) continue;
                float persent = (float) bpg[i]/menBet[i];
             
                bet02.MoveAllBet(i,persent, i != winIndex);
            }
        }

        private void RefreshSelf(ISFSObject response)
        {
            if (response.ContainsKey("gold"))
            {
                App.GameData.GetPlayer().Coin = response.GetLong("gold");
            }
        }


        void Reset()
        {
            ResetGame();
            CardsCtrl.Reset();
            BetCtrl.Reset();
            BetCtrl.AllBet.SetChipBtnsState(true);
            CancelInvoke();
            CardsCtrl02.SetCardCtrlActive(false);
        }

    }
}