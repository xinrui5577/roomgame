using System;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.lhc
{
    public class LhcGameManager : YxPhpGameManager
    {
        public BetChipCtrl BetChipCtrl;
        public ResultCtrl ResultCtrl;

        public UIButton ApplyBankButton;
        public List<RecordItem> LastRecords = new List<RecordItem>();
        public GameObject LastRecordGrid;
        public UILabel RemainingTime;
        public UILabel DescLabel;
        public UILabel LastLotteryNum;
        public ErrorMessageWindow ErrorMessage;

        private int _time;

        public void OnReturnHall()
        {
            App.QuitGameWithMsgBox();
        }

        public void OnGameRecord()
        {
            App.GetRServer<LhcGameServer>().SendRstHistory();
        }

        public void OnGameRule()
        {
            YxWindowManager.OpenWindow("RuleWindow");
        }

        public void OnGameSetting()
        {
            YxWindowManager.OpenWindow("SettingWindow");
        }

        public void OnApplyBank()
        {
            var gdata = App.GetGameData<LhcGameData>();

            if (gdata.ApplyBankCondition() >= 0)
            {
                var win = YxWindowManager.OpenWindow("ApplyBankWindow");
                win.UpdateView();
            }
            else
            {
                YxMessageBox.Show(string.Format("您拥有的钱不满足上庄条件(最小上庄金币为{0}) 请充值后再试！！！", YxUtiles.GetShowNumber(gdata.CurrentBanker.Coin + gdata.BankAdd)));
            }
        }

        public void ApplyBankChange()
        {
            ApplyBankButton.GetComponent<BoxCollider>().enabled = false;
            ApplyBankButton.GetComponent<UISprite>().color = Color.gray;
        }

        public void ChangeBankBtnState()
        {
            ApplyBankButton.GetComponent<BoxCollider>().enabled = true;
            ApplyBankButton.GetComponent<UISprite>().color = Color.white;
        }

        public void OnSendResult()
        {
            App.GetRServer<LhcGameServer>().RequestResult();
        }

        public override void OnGetGameInfo(Dictionary<string, object> gameInfo)
        {
            var gdata = App.GetGameData<LhcGameData>();
            if (gdata.CurrentBanker.Info.Seat == gdata.SelfSeat)
            {
                ApplyBankButton.GetComponent<BoxCollider>().enabled = false;
                ApplyBankButton.GetComponent<UISprite>().color = Color.gray;
            }

            if (gdata.WaitLottery)
            {
                ErrorMessage.ShowMsg("下注结算阶段请稍后");
            }

            var anteRate = gdata.AnteRate;
            BetChipCtrl.CreatChipBtn(anteRate);
            FreshLotteryNum();
            BetChipCtrl.InitBetData();
            ShowLastRecord();
            FreshResultTime();
        }

        public void FreshResultTime()
        {
            CancelInvoke("OnFreshTime");
            InvokeRepeating("OnFreshTime", 0, 1);
        }
        /// <summary>
        /// 刷新游戏界面上方
        /// </summary>
        public void FreshLotteryNum()
        {
            var gdata = App.GetGameData<LhcGameData>();
            DescLabel.text = gdata.Desc;
            LastLotteryNum.text = string.Format("上期为第{0}期", gdata.LastLotteryNum);
        }

        public void FreshGameResult(int time)
        {
            _time = time;
            InvokeRepeating("OnFreshGameRusultTime", 0, 1);
        }

        private void OnFreshGameRusultTime()
        {
            ErrorMessage.ShowMsg("等待开奖 稍后");
            _time--;
            if (_time == 0)
            {
                ErrorMessage.HideWindow();
                CancelInvoke("OnFreshGameRusultTime");
                OnSendResult();
            }
        }

        public void OnFreshTime()
        {
            var gdata = App.GetGameData<LhcGameData>();
            gdata.ResultTime--;

            if (gdata.ResultTime <= gdata.StopBetTime * 60)
            {
                ErrorMessage.ShowMsg("下注结算阶段请稍后");
            }

            if (gdata.ResultTime <= 0)
            {
                ErrorMessage.HideWindow();
                RemainingTime.text = "等待开奖 稍后";
                ErrorMessage.SendResultState();
                CancelInvoke("OnFreshTime");
                return;
            }

            var ts = new TimeSpan(0, 0, gdata.ResultTime);
            RemainingTime.text = ts.Days * 24 + ts.Hours + ":" + ts.Minutes + ":" + ts.Seconds;
        }

        private void HideTiShi()
        {
            RemainingTime.gameObject.SetActive(false);
        }

        /// <summary>
        /// 上次的开奖记录
        /// </summary>
        public void ShowLastRecord()
        {
            var gdata = App.GetGameData<LhcGameData>();
            if (gdata.LastRecord.Count == 0) return;
            for (int i = 0; i < LastRecords.Count; i++)
            {
                foreach (var value in gdata.BetPosColors)
                {
                    if (value.Value == gdata.LastRecord[i])
                    {
                        LastRecords[i].ItemBg.spriteName = value.Color;
                        LastRecords[i].ItemNum.text = value.Pos;
                    }
                }
            }
            LastRecordGrid.SetActive(true);
        }

        public override void OnGetRejoinInfo(Dictionary<string, object> gameInfo)
        {
        }

        public override void GameStatus(int status, Dictionary<string, object> info)
        {
        }

        public override void UserIdle(int localSeat, Dictionary<string, object> responseData)
        {
            base.UserIdle(localSeat, responseData);
        }

        public override void UserOnLine(int localSeat, Dictionary<string, object> responseData)
        {
            base.UserOnLine(localSeat, responseData);
            if (responseData.ContainsKey("resultTime"))
            {
                var gdata = App.GetGameData<LhcGameData>();
                gdata.ResultTime = int.Parse(responseData["resultTime"].ToString());
                FreshResultTime();
            }
        }

        public override void GameResponseStatus(int type, Dictionary<string, object> response)
        {
            var gdata = App.GetGameData<LhcGameData>();
            if (response.ContainsKey("errorMessage"))
            {
                ErrorMessage.ShowBtnAndMsg(response["errorMessage"].ToString());
                return;
            }
            FreshBetPanel(response);
            gdata.FreshPlayerView(response);
          

            switch (type)
            {
                case 1:
                    if (response.ContainsKey("message"))
                    {
                        YxMessageBox.Show(response["message"].ToString());
                        return;
                    }
                    var p = response.ContainsKey("p") ? int.Parse(response["p"].ToString()) : -1;
                    var seat = response.ContainsKey("seat") ? int.Parse(response["seat"].ToString()) : -1;
                    var gold = response.ContainsKey("gold") ? int.Parse(response["gold"].ToString()) : -1;
                    BetChipCtrl.InitBetPanel(p, seat, gold);
                    break;
                case 2:
                    if (response.ContainsKey("message"))
                    {
                        YxMessageBox.Show(response["message"].ToString());
                    }
                    BetChipCtrl.ChangeBank();
                    break;
                case 3:
                    if (response.ContainsKey("message"))
                    {
                        YxMessageBox.Show(response["message"].ToString());
                        return;
                    }
                    gdata.HistoryData = response.ContainsKey("historyData") ? response["historyData"] : null;
                    if (gdata.HistoryData != null)
                    {
                        var win = YxWindowManager.OpenWindow("HistoryWindow");
                        win.UpdateView();
                    }
                    break;
                case 4:
                    if (response.ContainsKey("message"))
                    {
                        YxMessageBox.Show(response["message"].ToString());
                        return;
                    }
                    ErrorMessage.ShowResultState(response);
                    break;
                case 5:
                    if (response.ContainsKey("message"))
                    {
                        YxMessageBox.Show(response["message"].ToString());
                        return;
                    }
                    ResultCtrl.FreshView(response);
                    break;
            }
        }

        private void FreshBetPanel(Dictionary<string, object> response)
        {
            var gdata = App.GetGameData<LhcGameData>();
            var betAllowsStr = response.ContainsKey("betAllows") ? response["betAllows"].ToString() : "";
            var selfBet = response.ContainsKey("selfBet") ? response["selfBet"] : null;
            var allBet = response.ContainsKey("allBet") ? response["allBet"] : null;
            gdata.PararmBetAllows(betAllowsStr);
            gdata.PararmSelfBets(selfBet);
            gdata.PararmAllBets(allBet);
        }
    }
}
