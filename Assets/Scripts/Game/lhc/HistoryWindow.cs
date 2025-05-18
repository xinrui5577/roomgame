using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lhc
{
    public class HistoryWindow : YxNguiWindow
    {
        public GameObject Parent;
        public UIGrid HistoryGrid;
        public HistoryItem HistoryItem;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (HistoryGrid.transform.childCount > 0)
            {
                DestroyImmediate(HistoryGrid.transform.GetChild(0).gameObject);
            }

            var gdata = App.GetGameData<LhcGameData>();
            var data = gdata.HistoryData;
            if (data is List<object>)
            {
                var historyData = data as List<object>;
                if (historyData.Count == 0) return;
                foreach (var history in historyData)
                {
                    if (history is Dictionary<string, object>)
                    {
                        var singeleHistory = history as Dictionary<string, object>;
                        var lotteryNum = singeleHistory.ContainsKey("lottery_num")
                            ? int.Parse(singeleHistory["lottery_num"].ToString())
                            : 0;
                        var winningNum = singeleHistory.ContainsKey("winning_num")
                            ? singeleHistory["winning_num"].ToString()
                            : "";
                        var lotteryOpenDt = singeleHistory.ContainsKey("lottery_open_dt")
                            ? singeleHistory["lottery_open_dt"].ToString()
                            : "";
                        var bank = singeleHistory.ContainsKey("bank") && (bool)singeleHistory["bank"];
                        var curWin = singeleHistory.ContainsKey("userWinCoin")
                            ? int.Parse(singeleHistory["userWinCoin"].ToString())
                            : 0;
                        var curWeek = singeleHistory.ContainsKey("week") ? singeleHistory["week"].ToString() : "";
                        var everyPosBet = singeleHistory.ContainsKey("everyPosBet") ? singeleHistory["everyPosBet"] : null;
                        var userBetAll = singeleHistory.ContainsKey("userBetAll") ? int.Parse(singeleHistory["userBetAll"].ToString()) : 0;
                        var numlist = winningNum.Split(',');
                        var item = YxWindowUtils.CreateItem(HistoryItem, HistoryGrid.transform);
                        item.InitHistory(lotteryNum, lotteryOpenDt, numlist, everyPosBet, bank, curWin, userBetAll, curWeek);
                    }
                }

                HistoryGrid.repositionNow = true;
            }
        }
    }
}
