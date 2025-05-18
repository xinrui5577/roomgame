using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.lhc
{
    public class ResultCtrl : MonoBehaviour
    {
        public GameObject Parent;
        public UIGrid BetGrid;
        public BetPosItem BetPosItem;
        public UIGrid WinGrid;
        public GameObject GoldItem;
        public List<RecordItem> CurRecords = new List<RecordItem>();

        private LhcGameManager _gmanager
        {
            get { return App.GetGameManager<LhcGameManager>(); }
        }
        public void ShowPanel()
        {
            if(!Parent.activeSelf)Parent.SetActive(true);
        }

        public void OnClose()
        {
            Parent.SetActive(false);
        }
      
        public void FreshView(Dictionary<string, object> response)
        {
           if(response.ContainsKey("delayTime"))  {

                var delayTime = int.Parse(response["delayTime"].ToString());
                _gmanager.FreshGameResult(delayTime);
                return;
            }


            if (!Parent.activeSelf)
            {
                Parent.SetActive(true);
            }
            else
            {
                return;
            }
            var everyPosBet = response.ContainsKey("everyPosBet") ? response["everyPosBet"] : null;
            if (everyPosBet is Dictionary<string, object>)
            {
                var betValue = everyPosBet as Dictionary<string, object>;
                if (BetGrid.transform.childCount > 0)
                {
                    DestroyImmediate(BetGrid.transform.GetChild(0).gameObject); 
                }

                foreach (var key in betValue.Keys)
                {
                    if (int.Parse(betValue[key].ToString()) == 0)
                    {
                        continue;
                    }

                    var item= YxWindowUtils.CreateItem(BetPosItem, BetGrid.transform);
                    item.InitView(key, int.Parse(betValue[key].ToString()));
                }

                BetGrid.repositionNow = true;
            }
            var userWinCoin = response.ContainsKey("userWinCoin")
                ? int.Parse(response["userWinCoin"].ToString())
                : 0;
            var reduceGold = YxUtiles.GetShowNumber(userWinCoin).ToString();
            bool positiveNum = false;
            if (userWinCoin >= 0)
            {
                positiveNum = true;
                reduceGold = "+" + reduceGold;
            }
            GetGoldSprite(reduceGold, positiveNum);

            var curWinNum = response.ContainsKey("curWinNum")
                ? response["curWinNum"].ToString()
                : "";
            if(string.IsNullOrEmpty(curWinNum))return;
            var gdata = App.GetGameData<LhcGameData>();
            var curWinNums = curWinNum.Split(',');
            gdata.LastRecord.Clear();

            for (int i = 0; i < CurRecords.Count; i++)
            {
                gdata.LastRecord.Add(int.Parse(curWinNums[i]));
                foreach (var value in gdata.BetPosColors)
                {
                    if (value.Value ==int.Parse(curWinNums[i]))
                    {
                        CurRecords[i].ItemBg.spriteName = value.Color;
                        CurRecords[i].ItemNum.text = value.Pos;
                    }
                }
            }
           
            gdata.FreshPlayerView(response);

            gdata.ResultTime = response.ContainsKey("resultTime") ? int.Parse(response["resultTime"].ToString()) :0;
            gdata.LotteryNum = response.ContainsKey("lotteryNum") ? int.Parse(response["lotteryNum"].ToString()) : 0;
            gdata.LastLotteryNum = response.ContainsKey("lastLotteryNum")
                ? int.Parse(response["lastLotteryNum"].ToString())
                : 0;
            var betAllowsStr = response.ContainsKey("betAllows") ? response["betAllows"].ToString() : "";
            gdata.PararmBetAllows(betAllowsStr);

            var selfBet = response.ContainsKey("selfBet") ? response["selfBet"] : null;
            gdata.PararmSelfBets(selfBet);

            var allBet = response.ContainsKey("allBet") ? response["allBet"] : null;

            gdata.PararmAllBets(allBet);

            gdata.BetDic = null;
            var gamanager = App.GetGameManager<LhcGameManager>();
            gamanager.ShowLastRecord();
            gamanager.BetChipCtrl.InitBetData();
            gamanager.FreshLotteryNum();
            gamanager.FreshResultTime();
        }

        private void GetGoldSprite(string gold,bool positiveNum)
        {
            if (WinGrid.transform.childCount > 0)
            {
                DestroyImmediate(WinGrid.transform.GetChild(0).gameObject); 
            }
            foreach (var t in gold)
            {
                var item = YxWindowUtils.CreateGameObject(GoldItem,WinGrid.transform);
                item.GetComponent<UISprite>().spriteName= ShowSingle(t.ToString(), positiveNum);
                item.GetComponent<UISprite>().MakePixelPerfect();
            }

            WinGrid.repositionNow = true;
        }

        private string ShowSingle(string val, bool positiveNum)
        {
            var str = positiveNum ? "+" : "-";

            switch (val)
            {
                case "+":
                case "-":
                    break;
                default:
                    str += val;
                    break;
            }
            return str; 
        }
    }
}
