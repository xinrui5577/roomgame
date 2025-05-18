using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.SportsCarClub
{
    public class RightBottomManager : MonoBehaviour
    {
        #region 上下庄
        /// <summary>
        /// 上庄金额限制
        /// </summary>
        public int BankerLimit = 0;

        public UIButton ApplyBankerBtn;

        public UIButton ApplyQuitBtn;

        /// <summary>
        /// 申请上庄
        /// </summary>
        public void ApplyBanker()
        {
            if (App.GameData.GetPlayerInfo() == null)
                return;

            if (App.GameData.GetPlayerInfo().CoinA < BankerLimit)
            {
                var limit = YxUtiles.GetShowNumberToString(BankerLimit);
                YxMessageTip.Show("开店资金需要" + limit + "金币.");
                return;
            }

            CschGameServer.GetInstance().SendRequest(RequestType.ApplyBanker, null);
        }
        /// <summary>
        /// 申请下庄
        /// </summary>
        public void ApplyQuit()
        {
            CschGameServer.GetInstance().SendRequest(RequestType.ApplyQuit, null);
            YxMessageTip.Show("此局游戏结束后自动下庄，请稍后！！！");
            //ApplyQuitBtn.isEnabled = false;
        }

        #endregion

        #region 重复上轮
        /// <summary>
        /// 当前的总轮数
        /// </summary>
        public int CurNum;
        /// <summary>
        /// 上轮ID
        /// </summary>
        public int LastTimeNum;
        /// <summary>
        /// 上轮的值
        /// </summary>
        public List<int> UpValue;
        /// <summary>
        /// 重复上轮按键
        /// </summary>
        public UIButton LastTime;
        /// <summary>
        /// 上一轮的下注值
        /// </summary>
        public List<int> UpBetValue = new List<int>(10);
        /// <summary>
        /// 刷新重复上轮按键
        /// </summary>
        public void RefreshLastTimeBtn()
        {
            var self = App.GameData.GetPlayerInfo();
            if (App.GetGameManager<CschGameManager>().BankerMgr.IsBanker(self.Seat))
            {
                LastTime.isEnabled = false;
                return;
            }

            int[] golds = UpValue.ToArray();

            int gold = 0;

            foreach (int goldv in golds)
            {
                gold += goldv;
            }

            if (self.CoinA < gold)
            {
                LastTime.isEnabled = false;
                return;
            }

            LastTime.isEnabled = UpValue.Count != 0;
        }
        /// <summary>
        /// 重复上轮
        /// </summary>
        public void OnClickLastTime()
        {
            var self = App.GameData.GetPlayerInfo();

            if (App.GetGameManager<CschGameManager>().BankerMgr.IsBanker(self.Seat))// || CurNum == LastTimeNum)
                return;

            if (!BetManager.GetInstance().IsBeginBet)
                return;
            
            if (App.GetGameManager<CschGameManager>().BankerMgr.IsBanker(self.Seat))
            {
                YxMessageTip.Show("您是店主,不能跟自己玩哦!");
                return;
            }

            var golds = UpValue.ToArray();

            var gold = 0;
            var regions = App.GetGameManager<CschGameManager>().Regions;
            for (int i = 0; i < golds.Length; i++)
            {
                if (golds[i] > regions[i].CanBet)
                {
                    //golds[i] = 0;
                    LastTime.isEnabled = false;
                    YxMessageTip.Show("不能下注,店家金币不足!");
                    return;
                }
                gold += golds[i];
            }

            if (self.CoinA < gold)
            {
                YxMessageTip.Show("金币不足,请充值或换更小的筹码!");
                return;
            }

            IDictionary bet = new Dictionary<string, object>();
            bet.Add("golds", golds);
            CschGameServer.GetInstance().SendRequest(RequestType.Bet, bet);
            //LastTime.isEnabled = false;
        }

        #endregion
    }
}
