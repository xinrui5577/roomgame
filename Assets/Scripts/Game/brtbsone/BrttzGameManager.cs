using UnityEngine;
using System.Collections;
using YxFramwork.Manager;
using Sfs2X.Entities.Data;
using System;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;
using YxFramwork.ConstDefine;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.View;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.brtbsone
{
    //游戏初始化
    public class BrttzGameManager : YxGameManager
    {
        public BankerListCtrl BankerListCtrl;
        public BetCtrl BetCtrl;
        public ProgressCtrl ProgressCtrl;
        public BrttzCardsCtrl BrttzCardsCtrl;
        public ShowNumCtrl ShowNumCtrl;
        public ApplyCtrl ApplyCtrl;
        public YxWindow Wait;
        public TableResultInfo TableResultInfo;
        public ResultListCtrl ResultListCtrl;
        public BrttzPlayerManager PlayerManager;
        public bool HaveCardShow = false;

        [HideInInspector] public bool CanQuitGame = true;

        protected int StartStatus;
        public override void GameStatus(int status, ISFSObject info)
        {
        }
        /// <summary>
        /// 获取gameinfo
        /// </summary>
        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            ResetData();
            ShowNumCtrl.Init();
            CheckReJion(gameInfo);
            if (gameInfo.ContainsKey(Parameter.Status))
            {
                StartStatus = gameInfo.GetInt(Parameter.Status);
            }
            if (gameInfo.ContainsKey(Parameter.Bankers))
                App.GetGameData<BrttzGameData>().BankerPlayer.Show();
            if (gameInfo.ContainsKey(Parameter.Record))
            {
                var strArray = gameInfo.GetSFSArray(Parameter.Record);
                AddHistoryResult(strArray);
            }
            BrttzCardsCtrl.InitHistoryCards();
            if (gameInfo.ContainsKey(Parameter.RCards))
                BrttzCardsCtrl.GetHistoryCards(gameInfo);
            BankerListCtrl.RefreshBankerList(gameInfo);
            ProgressCtrl.SetNum(gameInfo);
            RefreshUserInfo();
            BrttzCardsCtrl.GetGameInfoOnCheck(gameInfo);
            BrttzCardsCtrl.GetIsXiPai(gameInfo);
            BetCtrl.InitChips();
            if (StartStatus == 20 && gameInfo.ContainsKey(Parameter.Dices))
            {
                BrttzCardsCtrl.GetDicesPoints(gameInfo.GetIntArray(Parameter.Dices));
            }
            if (StartStatus == 4)
            {
                BrttzCardsCtrl.GiveCardsOnFrist(gameInfo);
            }
            if (StartStatus == 3 || StartStatus == 4)
            {
                if (gameInfo.ContainsKey(Parameter.Bet))
                {
                    int[] selfBet = new int[0];
                    if (gameInfo.ContainsKey("user") && gameInfo.GetSFSObject("user").ContainsKey("betGolds"))
                        selfBet = gameInfo.GetSFSObject("user").GetIntArray("betGolds");
                    var bets = gameInfo.GetIntArray(Parameter.Bet);
                    ShowNumCtrl.Init(bets, selfBet);
                }
            }
            if (gameInfo.ContainsKey(Parameter.Bround))
                App.GetGameData<BrttzGameData>().BankerPlayer.SetBankerTime(gameInfo.GetInt(Parameter.Bround));

            if (gameInfo.ContainsKey(Parameter.SameCardNum))
            {
                BrttzCardsCtrl.HistoryCardsCtrl.MaxMahjongNum = gameInfo.GetInt(Parameter.SameCardNum);
            }
            if (PlayerManager != null)
                PlayerManager.InitTablePlayerInfo(gameInfo);
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {

        }

        public virtual void OnGetPlayers(ISFSObject responseData)
        {
            if (!responseData.IsNull("playerlist"))
            {
                string[] playerStr = responseData.GetUtfStringArray("playerlist");
                //UserListCtrl.RefreshPlayer(playerStr);
            }
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var gdata = App.GetGameData<BrttzGameData>();
            switch (type)
            {
                case RequestType.BankerList:
                    YxDebug.Log("----------------获取上庄列表:----" + type);
                    BankerListCtrl.RefreshBankerList(response);
                    ProgressCtrl.SetNum(response);
                    BetCtrl.ShowChip();
                    break;

                case RequestType.StartBet:
                    YxDebug.Log("----------------开始下注:----" + type);
                    Wait.Hide();
                    gdata.BeginBet = true;
                    gdata.SetGameStatus(YxEGameStatus.Play);
                    if (!HaveCardShow)
                    {
                        ResetData();
                        BrttzCardsCtrl.GetIsXiPai(response);
                    }
                    ApplyCtrl.RefreshBanker();
                    if (gdata.BankerPlayer.Info.Seat != -1)
                        gdata.BankerPlayer.AddBankerTime();
                    var startCd = response.ContainsKey(Parameter.Cd) ? response.GetInt(Parameter.Cd) : 18;
                    ProgressCtrl.ReSetCountdown(startCd + 1, type);
                    ProgressCtrl.BeginCountdown(true);
                    CanQuitGame = true;
                    if (PlayerManager != null)
                        PlayerManager.InitTablePlayerInfo(response);
                    YxMessageTip.Show("开始下注");
                    break;

                case RequestType.StopBet:
                    YxDebug.Log("----------------停止下注:----" + type);
                    gdata.SetGameStatus(YxEGameStatus.Play);
                    gdata.BeginBet = false;
                    BetCtrl.HideChip();
                    var endCd = response.ContainsKey(Parameter.Cd) ? response.GetInt(Parameter.Cd) : 14;
                    ProgressCtrl.ReSetCountdown(endCd, type);
                    ProgressCtrl.BeginCountdown();
                    if (!HaveCardShow) BrttzCardsCtrl.GetDicesPoints(response);
                    break;

                case RequestType.Bet:
                    YxDebug.Log("----------------下注:----" + type);
                    gdata.SetGameStatus(YxEGameStatus.Play);
                    BetCtrl.Bet(response);
                    ProgressCtrl.RefreshNum(response);
                    ShowNumCtrl.RefreshNum(response);
                    if (response.GetInt("seat") == gdata.SelfSeat && gdata.GetPlayerInfo().CoinA < gdata.MiniApplyBanker)
                        ApplyCtrl.Instance.SetStutus(2);
                    break;

                case RequestType.SendCard:
                    YxDebug.Log("----------------发牌:----" + type);
                    gdata.SetGameStatus(YxEGameStatus.Play);
                    BrttzCardsCtrl.BeginGiveCards(response);
                    break;

                case RequestType.Result:
                    YxDebug.Log("----------------结算:----" + type);
                    Wait.Hide();
                    gdata.SetGameStatus(YxEGameStatus.Normal);
                    BrttzCardsCtrl.ReceiveResult(response);
                    ResultBet(BrttzCardsCtrl.Bpg, ShowNumCtrl.ZBet);
                    StartCoroutine("ResultMoveChip");
                    TableResultInfo.ShowTableResultInfo(response);
                    RefreshSelf(response);
                    ResultListCtrl.AddResult(response.GetIntArray("wins"));
                    ApplyCtrl.HideApplyBanker();
                    ProgressCtrl.SetNumOnResult(response);
                    CanQuitGame = true;
                    break;

                case RequestType.GroupBet:
                    YxDebug.Log("----------------流式下注:----" + type);
                    BetCtrl.GroupBet(response);
                    ProgressCtrl.GroupRefreshNum(response);
                    ShowNumCtrl.GroupRefreshNum(response);
                    break;

                default:
                    YxDebug.Log("Wrong RequestType : " + type);
                    break;
            }
            OnGetPlayers(response);
        }

        public override void UserOnLine(int localSeat, ISFSObject responseData)
        {
            base.UserOnLine(localSeat, responseData);
            BrttzCardsCtrl.Dices.Reset();
        }

        /// <summary>
        /// 检测重连信息
        /// </summary>
        /// <param name="requestData"></param>
        private void CheckReJion(ISFSObject requestData)
        {
            long st = requestData.GetLong("st");
            long ct = requestData.GetLong("ct");
            if (st != 0)
            {
                if (ct - st < 15)
                {
                    App.GetGameData<BrttzGameData>().BeginBet = true;
                    BrttzCardsCtrl.Reset();
                    BetCtrl.ShowChip();
                    ProgressCtrl.ReSetCountdown(Convert.ToInt32(ct - st));
                    ProgressCtrl.BeginCountdown();
                }
                ShowNumCtrl.SetNum(requestData);
                Wait.Show();
            }
        }

        private void RefreshSelf(ISFSObject response)
        {
            var gdata = App.GetGameData<BrttzGameData>();

            gdata.ThisCanInGold = gdata.GetPlayerInfo().CoinA;
            var banker = gdata.BankerPlayer;
            if (response.ContainsKey(Parameter.Bwin))
                banker.WinTotalCoin += response.GetLong(Parameter.Bwin);
            if (response.ContainsKey("bankerGold"))
                banker.Coin = response.GetLong("bankerGold");
            if (response.ContainsKey(Parameter.Total))
                App.GameData.GetPlayer().Coin = response.GetLong(Parameter.Total);
        }

        public void RefreshUserInfo()
        {
            var gdata = App.GetGameData<BrttzGameData>();
            var self = gdata.GetPlayer();
            self.UpdateView();
            gdata.ThisCanInGold = gdata.GetPlayerInfo().CoinA;
        }

        private void AddHistoryResult(ISFSArray Datas)
        {
            foreach (ISFSObject data in Datas)
            {
                ResultListCtrl.AddResult(data.GetIntArray("win"));
            }
        }

        /// <summary>
        /// 计算补全输赢的筹码
        /// </summary>
        /// <param name="bWin">庄家输赢数据</param>
        /// <param name="menBet">每门下注额</param>
        private void ResultBet(int[] bWin, int[] menBet)
        {
            if (bWin == null || bWin.Length <= 0) return;
            if (menBet == null || bWin.Length <= 0) return;

            for (int i = 0; i < bWin.Length; i++)
            {
                int bw = bWin[i];
                if (bw == 0) continue;//考虑了和的情况
                if (bw < 0)
                {
                    BetCtrl.ThrowChips(-bw, i, true);  //1为庄家下注位置
                }
                else
                {
                    BetCtrl.ThrowChips(Mathf.Abs(bw - menBet[i]), i, false);
                }
            }
        }

        public void OnClickBackBtn()
        {
            bool isBanker = App.GetGameData<BrttzGameData>().IsBanker;
            if (isBanker)
            {
                YxMessageBox.Show(new YxMessageBoxData
                {
                    Msg = "您现在是庄家,无法离开游戏!",
                    Delayed = 5
                });
            }
            else if (!CanQuitGame)
            {
                YxMessageBox.Show(new YxMessageBoxData
                {
                    Msg = "您已经下注了,无法离开游戏!",
                    Delayed = 5
                });
            }
            else if (CanQuitGame)
            {
                OnQuitGameClick();
            }
        }

        /// <summary>
        /// 当点击续押时调用
        /// </summary>
        public void SendLastGameBet()
        {
            int[] wBet = ShowNumCtrl.LastWBet;
            long sum = GetSum(wBet);
            if (wBet == null || sum == 0)
            {
                YxMessageTip.Show("上局游戏您没有下注!!");
                return;
            }
            if (BetCtrl.CouldBet(sum))
            {
                App.GetRServer<BrttzGameServer>().BetAsLastGame(wBet);
            }
        }

        int GetSum(int[] array)
        {
            int sum = 0;
            int len = array.Length;
            for (int i = 0; i < len; i++)
            {
                sum += array[i];
            }
            return sum;
        }

        IEnumerator ResultMoveChip()
        {
            yield return new WaitForSeconds(1f);
            ResultMoveChips();
        }

        void ResultMoveChips()
        {
            var bpg = BrttzCardsCtrl.Bpg;
            if (bpg == null || bpg.Length <= 0)
                return;
            for (int i = 0; i < bpg.Length; i++)
            {
                BetCtrl.MoveAllBet(i, bpg[i] > 0);
            }
        }

        protected void ResetData()
        {
            ShowNumCtrl.Reset();
            BrttzCardsCtrl.Reset();
            BetCtrl.Reset();
            BetCtrl.BetByCoin.SetChipBtnsState(true);
            CancelInvoke();
        }
    }
}