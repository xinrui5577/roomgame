using UnityEngine;
using System.Collections;
using YxFramwork.Enums;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using Assets.Scripts.Game.brnn.brnn_skin01;
using YxFramwork.View;
using YxFramwork.Framework;


namespace Assets.Scripts.Game.brnn.brnn_skin02
{
    public class BrnnGameManager02 : BrnnGameManager01
    {

        private bool isCounting = false;

        public YxWindow WaitView;

        public GameSetting02 GameSetting;

        public TableResultInfo TableResultInfo;
        public override void OnGetGameInfo(ISFSObject requestData)
        {
            base.OnGetGameInfo(requestData);

            if (requestData.ContainsKey("bkmingold"))
                ((ProgressCtrl02)ProgressCtrl).SetBankerLimitLabel(requestData.GetInt("bkmingold"));
            if (GameSetting != null)
            {
                var music = YxFramwork.Framework.Core.Facade.Instance<YxFramwork.Manager.MusicManager>();
                MusicToggle.StartsActive = music.MusicVolume > 0;
                GameSetting.SetSound(music.MusicVolume > 0);
            }
            if (requestData.ContainsKey("record"))
            {
                var strArray = requestData.GetUtfStringArray("record");
                AddHistoryResultList(strArray);
            }
        }

        /// <summary>
        /// 结算结果显示在结算列表中
        /// </summary>
        /// <param name="strArray"></param>
        private void AddHistoryResultList(string[] strArray)
        {
            int length = strArray.Length;
            for (int i = 0; i < length; i++)
            {
                string temp = strArray[i];
                if (string.IsNullOrEmpty(temp))
                    continue;
                bool[] bo = GetBoolArray(temp);
                ResultListCtrl.AddResult(bo);
            }
        }

        private bool[] GetBoolArray(string str)
        {

            var temp = str.Split(',');
            int length = temp.Length;
            bool[] bo = new bool[length];
            for (int i = 0; i < length; i++)
            {
                bool b = int.Parse(temp[i]) > 0;
                bo[i] = b;
            }
            return bo;
        }


        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var gdata = App.GetGameData<BrnnGameData>();
            switch (type)
            {
                case RequestType.BeginBet:
                    Reset();
                    WaitView.Hide();
                    base.GameResponseStatus(type, response);
                    if (response.ContainsKey("bankRound"))
                    {
                        gdata.CurrentBanker.SetBankerTime(response.GetInt("bankRound"));
                    }
                    else
                    {
                        gdata.CurrentBanker.HideBankerTime();
                    }
                    YxMessageTip.Show("开始下注");
                    break;
                case RequestType.EndBet:
                    WaitView.Hide();
                    base.GameResponseStatus(type, response);
                    YxMessageTip.Show("停止下注");

                    break;
                case RequestType.Result:
                    WaitView.Hide();
                    gdata.SetGameStatus(YxEGameStatus.Normal);
                    CardsCtrl.ReceiveResult(response);
                    ResultBet(CardsCtrl.Bpg, ShowNumCtrl.ZBet);
                    StartCoroutine(ResultMoveChip());

                    if (ResultWin != null)
                    {
                        if(!isCounting)
                        {
                            StartCoroutine(ShowResultView(response));
                        }
                    }

                    TableResultInfo.ShowTableResultInfo(response);

                    UserListCtrl.RefreshBanker(response);
                    RefreshSelf(response);
                    gdata.GetPlayer<BrnnPlayer>().ReSet();
                    ApplyCtrl.HideApplyBanker();
                    ProgressCtrl.SetNum(response);
                                               
                    break;

                default:
                    base.GameResponseStatus(type, response);
                    break;
            }
        }

        private void RefreshSelf(ISFSObject response)
        {
           if(response.ContainsKey("gold"))
           {
               App.GameData.GetPlayer().Coin = response.GetLong("gold");
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
            if(BetCtrl.CouldBet(sum))
            {
                App.GetRServer<BrnnGameServer>().BetAsLastGame(wBet);
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
            ResultWin.ShowResultView(response);
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
            if (bpg == null || bpg.Length <= 0)
                return;
            var bet02 = (BetCtrl02)BetCtrl;
            for (int i = 0; i < bpg.Length; i++)
            {
                bet02.MoveAllBet(i, bpg[i] > 0);
            }
        }

        void Reset()
        {
            CardsCtrl.Reset();
            BetCtrl.Reset();
            BetCtrl.AllBet.SetChipBtnsState(true);
            CancelInvoke();
        }
    }
}