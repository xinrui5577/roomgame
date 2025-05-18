using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.lhc
{
    public class HistoryItem : MonoBehaviour
    {
        public UILabel CurPeriods;
        public UILabel LotteryDate;
        public UILabel LotteryDay;
        public GameObject BanIcon;
        public UILabel CurWinGold;
        public UILabel CurTotalBet;

        public UIGrid CurBetGrid;
        public BetPosItem CurBetPosItem;

        public List<UISprite> HistoryList = new List<UISprite>();

        public void InitHistory(int curPeriods, string openTime, string[] winValue, object everyPosBet, bool isBank, int winGold, int betAllGold, string week = "")
        {

            CurPeriods.text = string.Format("第{0}期", curPeriods);
            LotteryDate.text = string.Format("开奖日期：{0}", openTime);
            if (!string.IsNullOrEmpty(week) && LotteryDay)
            {
                LotteryDay.text = string.Format("开奖日：{0}", week);
            }

            if (isBank)
            {
                BanIcon.SetActive(true);
            }

            CurWinGold.text = YxUtiles.GetShowNumber(winGold).ToString();
            CurTotalBet.text = YxUtiles.GetShowNumber(betAllGold).ToString();

            if (everyPosBet != null)
            {
                var betValue = everyPosBet as List<object>;
                if (betValue != null)
                {
                    if (CurBetGrid.transform.childCount > 0)
                    {
                        DestroyImmediate(CurBetGrid.transform.GetChild(0).gameObject);
                    }

                    for (int i = 0; i < betValue.Count; i++)
                    {
                        if (int.Parse(betValue[i].ToString()) == 0) continue;
                        var item = YxWindowUtils.CreateItem(CurBetPosItem, CurBetGrid.transform);
                        item.InitView((i + 1).ToString(), int.Parse(betValue[i].ToString()));
                    }
                }

                CurBetGrid.repositionNow = true;
            }

            var gdata = App.GetGameData<LhcGameData>();

            for (int i = 0; i < HistoryList.Count; i++)
            {
                foreach (var value in gdata.BetPosColors)
                {
                    if (value.Value==int.Parse(winValue[i]))
                    {
                        HistoryList[i].spriteName = value.Color;
                    }
                }

                HistoryList[i].GetComponentInChildren<UILabel>().text = winValue[i];
            }
        }
    }
}
