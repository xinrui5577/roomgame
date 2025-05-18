using System.Collections;
using YxFramwork.Manager;
using Sfs2X.Entities.Data;
using System;
using Assets.Scripts.Common.Windows;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;
using YxFramwork.ConstDefine;
using YxFramwork.View;
using YxFramwork.Framework.Core;
using YxFramwork.Enums;

namespace Assets.Scripts.Game.toubao
{
    public class TouBaoGameManager : YxGameManager
    {
        /// <summary>
        /// 是否可以退出
        /// </summary>
        public bool CanQuit;
        public YxNguiWindow WaitWin;
        public BetAreaPanel BetArea;
        public TouziCtrl TouziCtrl;
        public Result Result;
        public Clock Clock;
        public BankerManager BankerManager;
        public UserPanel UserPanel;
        public BankerCtrl BankerCtrl;
        public YxMessageTip TipWin;

        protected override void OnStart()
        {
            base.OnStart();
            WaitWin.Show();
            if (TipWin != null) TipWin.Hide();
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var gdata = App.GetGameData<GlobalData>();
            UserPanel user = App.GameData.GetPlayer() as UserPanel;
            switch (type)
            {
                case RequestType.Bet:
                    YxDebug.Log("申请上庄");
                    App.GetGameData<GlobalData>().BankerApplying = true;
                    BankerManager.ChangeTheBt();
                    break;
                case RequestType.XiaZhuang:
                    YxDebug.Log("取消上庄");
                    App.GetGameData<GlobalData>().BankerApplying = false;
                    BankerManager.ChangeTheBt();
                    break;
                case RequestType.XiaZhu:
                    YxDebug.Log("下注");
                    BetArea.Bet(response);
                    BetArea.ShowRefreshNum(response);
                    if (response.ContainsKey(RequestKey.KeySeat))
                    {
                        var seat = response.GetInt(RequestKey.KeySeat);
                        if (seat == App.GameData.SelfSeat)
                        {
                            user.SetRoundBetShow();
                            gdata.GetPlayer().UpdateView();
                        }
                    }
                    break;
                case RequestType.RollResult:
                    YxDebug.Log("骰子结果");
                    int[] diceArray = response.GetIntArray("dices");
                    TouziCtrl.PlaySaiZiAni(diceArray);
                    break;
                case RequestType.GameResult:
                    App.GetGameData<GlobalData>().ChangeGameState(GlobalData.GameState.Free);
                    Clock.SetClockNum(response.GetInt("cd"));
                    YxDebug.Log("结算");
                    CanQuit = !BankerManager.IsBanker(App.GameData.SelfSeat);
                    Result.UpdateView(response);
                    if (!BankerManager.IsBanker(-1))
                    {
                        BankerManager.Banker.WinTotalCoin += response.GetLong("bwin");
                        BankerManager.Banker.Coin = response.GetLong("bankerGold");

                    }
                    App.GameData.GetPlayer().Coin = response.GetLong("total");
                    user.InitOnce();
                    Result.Show();
                    break;
                case RequestType.Start:
                    YxDebug.Log("开始下注");
                    YxMessageTip.Show("开始下注");
                    App.GetGameData<GlobalData>().ChangeGameState(GlobalData.GameState.XiaZhu);
                    Clock.SetClockNum(response.GetInt("cd"));
                    CanQuit = !BankerManager.IsBanker(App.GameData.SelfSeat);
                    WaitWin.Hide();
                    BetArea.Init();
                    BetArea.CloseWinAnim();
                    BetArea.ReSetRefreshNum();
                    gdata.BeginBet = true;
                    if (BankerManager.Banker != null)
                    {
                        BankerManager.Banker.TotalCount++;
                        BankerManager.Banker.UpdateView();
                        //限制自己为庄家时下注
                        if (CanQuit)
                            BetArea.ShowChip();
                        else
                        {
                            App.GetGameData<GlobalData>().ChangeGameState(GlobalData.GameState.IsBanker);
                        }
                    }
                    else
                    {
                        BetArea.ShowChip();
                    }
                    Result.gameObject.SetActive(false);
                    break;
                case RequestType.Stop:
                    YxDebug.Log("停止下注");
                    gdata.BeginBet = false;
                    App.GetGameData<GlobalData>().ChangeGameState(GlobalData.GameState.Result);
                    BetArea.HideChip();
                    WaitWin.Hide();
                    break;
                case RequestType.ZhuangChange:
                    YxDebug.Log("发送庄家列表");
                    if (response.GetInt("banker") == -1)
                    {
                        BankerManager.SetBankerInfo(null);
                    }
                    BankerManager.RefreshBankerList(response.GetSFSArray("bankers"), response.GetInt("banker"));
                    BankerManager.RefreshPlayerList(response.GetSFSArray("bankers"), response.GetInt("banker"));
                    CanQuit = response.GetInt("banker") != App.GameData.SelfSeat;
                    break;
                default:
                    YxDebug.Log("RequestType : " + type);
                    break;
            }
        }

        public void OnBetDownClick(UISprite sprite, int rate)
        {
            BetArea.OnDeskClick(sprite, rate);
        }

        public override void GameStatus(int status, ISFSObject info)
        {

        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            BankerCtrl.Init(gameInfo.GetInt("bankLimit"));
            if (BankerManager.IsBanker(-1))
            {
                BankerManager.SetBankerInfo(null);
            }
            BankerManager.RefreshBankerList(gameInfo.GetSFSArray("bankers"), gameInfo.GetInt("banker"));
            BankerManager.RefreshPlayerList(gameInfo.GetSFSArray("bankers"), gameInfo.GetInt("banker"));
            CheckReJion(gameInfo);
            BetArea.InitChips();
        }


        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {

        }

        protected virtual void CheckReJion(ISFSObject requestData)
        {
            var st = requestData.GetLong("st");
            var ct = requestData.GetLong("ct");
            if (st != 0)
            {
                var gdata = App.GetGameData<GlobalData>();
                if (ct - st < 12)
                {
                    gdata.BeginBet = true;
                    App.GameData.GStatus = YxEGameStatus.PlayAndConfine;

                    //限制自己为庄家时下注
                    if (gdata.SelfSeat != gdata.BankSeat || App.GameKey.Equals("bjl"))
                    {
                        BetArea.ShowChip();
                    }
                    WaitWin.Show();
                }
            }
        }
    }
    public enum ListType
    {
        None,
        Banker,
        Player,
    }
}