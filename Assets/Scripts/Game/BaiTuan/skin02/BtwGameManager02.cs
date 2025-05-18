using UnityEngine;
using System.Collections;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.View;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.BaiTuan.skin02
{
    public class BtwGameManager02 : BtwGameManager
    {
        private bool isCounting = false;
        protected int StartStatus;

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            base.OnGetGameInfo(gameInfo);
            if (gameInfo.ContainsKey("status"))
            {
                StartStatus = gameInfo.GetInt("status");
            }
            if (gameInfo.ContainsKey("record"))
            {
                var strArray = gameInfo.GetSFSArray("record");
                AddHistoryResult(strArray);
            }
            if (gameInfo.ContainsKey("bankers"))
            {
                var banker = App.GetGameData<BtwGameData>().BankerPlayer;
                //banker.SetBankerTime(requestData.GetInt("bankers"));
                banker.Show();
            }
            if (StartStatus == 3 || StartStatus == 4)
            {
                if (gameInfo.ContainsKey("glist"))
                {
                    int[] selfBet = new int[0];
                    if (gameInfo.ContainsKey("user") && gameInfo.GetSFSObject("user").ContainsKey("betGolds"))
                        selfBet = gameInfo.GetSFSObject("user").GetIntArray("betGolds");
                    var bets = gameInfo.GetIntArray("glist");
                    ShowNumCtrl.Init(bets, selfBet);
                }
            }
            if (StartStatus == 4)
                CardsCtrl.GiveCardsOnFrist(gameInfo);
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var gdata = App.GetGameData<BtwGameData>();
            switch ((BtwSkin02RequestType)type)
            {
                case BtwSkin02RequestType.XiaZhu:
                    YxDebug.Log("-----------------下注--------------");
                    WaitWin.Hide();
                    BetCtrl.Bet(response);
                    ShowNumCtrl.RefreshNum(response);
                    break;
                case BtwSkin02RequestType.ZhuangChange:
                    YxDebug.Log("-----------------获取上庄列表--------------");
                    WaitWin.Hide();
                    UserListCtrl.RefreshBanker(response);
                    ProgressCtrl.SetNum(response);
                    BetCtrl.ShowChip();
                    break;
                case BtwSkin02RequestType.Start:
                    YxDebug.Log("-----------------开始下注--------------");
                    WaitWin.Hide();
                    gdata.BeginBet = true;
                    CardsCtrl.ReSetPonits();
                    CardsCtrl.gameObject.SetActive(false);
                    ShowNumCtrl.Reset();
                    ReSetGame();
                    BetCtrl.ShowChip();
                    ProgressCtrl.ReSetCountdown(18);
                    ProgressCtrl.BeginCountdown();
                    ApplyCtrl.RefreshBanker();
                    TimeBet.gameObject.SetActive(true);
                    if (gdata.BankerPlayer.Info.Seat != -1)
                        gdata.BankerPlayer.AddBankerTime();
                    Facade.Instance<MusicManager>().Play("beginbet");
                    YxMessageTip.Show("开始下注");
                    break;
                case BtwSkin02RequestType.Stop:
                    YxDebug.Log("-----------------停止下注--------------");
                    gdata.BeginBet = false;
                    BetCtrl.HideChip();
                    ProgressCtrl.EndCountdown();
                    TimeBet.gameObject.SetActive(false);
                    Facade.Instance<MusicManager>().Play("stopbet");
                    WaitWin.Hide();
                    break;
                case BtwSkin02RequestType.RollResult:
                    YxDebug.Log("-----------------发牌--------------");
                    CardsCtrl.BeginGiveCards(response);
                    CardsCtrl.gameObject.SetActive(true);
                    WaitWin.Hide();
                    break;
                case BtwSkin02RequestType.GameResult:
                    YxDebug.Log("-----------------结算--------------");
                    WaitWin.Hide();
                    gdata.SetGameStatus(YxEGameStatus.Normal);
                    CardsCtrl.ReceiveResult(response);
                    ResultBet(CardsCtrl.Bpg, ShowNumCtrl.ZBet);
                    StartCoroutine(ResultMoveChip());
                    if (ResultWin != null)
                        StartCoroutine(ShowResultView(response));
                    RefreshSelf(response);
                    ProgressCtrl.SetNumOnResult(response);
                    break;
                case BtwSkin02RequestType.GroupBet:
                    YxDebug.Log("-----------------流式下注--------------");
                    BetCtrl.GroupBet(response);
                    ProgressCtrl.GroupRefreshNum(response);
                    ShowNumCtrl.GroupRefreshNum(response);
                    break;
                default:
                    YxDebug.Log("-*-*-Wrong RequestType : " + type);
                    break;
            }
        }

        private void RefreshSelf(ISFSObject response)
        {
            var gdata = App.GetGameData<BtwGameData>();
            gdata.ThisCanInGold = gdata.GetPlayerInfo().CoinA;
            var banker = gdata.BankerPlayer;
            if (response.ContainsKey("bankerGold"))
                banker.Coin = response.GetLong("bankerGold");
            if (response.ContainsKey("total"))
            {
                App.GameData.GetPlayer().Coin = response.GetLong("total");
            }
        }

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
                App.GetRServer<BtwGameServer>().BetAsLastGame(wBet);
            }
        }

        private void AddHistoryResult(ISFSArray Datas)
        {
            foreach (ISFSObject data in Datas)
            {
                ResultListCtrl.AddResultOnFrist(data.GetIntArray("win"));
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

        private IEnumerator ShowResultView(ISFSObject response)
        {
            yield return new WaitForSeconds(2f);
            ResultWindow02 window02 = ResultWin as ResultWindow02;
            window02.ShowResultView(response);
            StopAllCoroutines();
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

        IEnumerator ResultMoveChip()
        {
            yield return new WaitForSeconds(1f);
            ResultMoveChips();
        }

        void ResultMoveChips()
        {
            var bpg = CardsCtrl.Bpg;
            if (bpg == null || bpg.Length <= 0) return;
            var bet02 = (BetCtrl02)BetCtrl;
            for (int i = 0; i < bpg.Length; i++)
            {
                bet02.MoveAllBet(i, bpg[i] > 0);
            }
            CancelInvoke("ResultMoveChips");
        }

        void Reset()
        {
            BetCtrl.Reset();
            CancelInvoke();
        }
    }
}