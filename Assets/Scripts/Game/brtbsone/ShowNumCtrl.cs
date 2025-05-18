using System;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.brtbsone
{
    public class ShowNumCtrl : MonoBehaviour
    {
        /// <summary>
        /// 每门的总注
        /// </summary>
        public UILabel[] ZLabels;
        /// <summary>
        /// 每门的个人下注
        /// </summary>
        public UILabel[] WLabels;

        /// <summary>
        /// 所有人的每门下注
        /// </summary>
        internal int[] ZBet;

        internal int[] LastWBet;

        /// <summary>
        /// 自己每门下注
        /// </summary>
        [HideInInspector]
        public int[] WBet;

        public void Init()
        {
            int zlen = ZLabels.Length;
            int wlen = WLabels.Length;
            App.GetGameData<BrttzGameData>().ZNumber = new int[zlen];



            ZBet = new int[zlen];
            WBet = new int[wlen];
        }

        public void Init(int[] bets, int[] self)
        {
            ZBet = bets;
            for (int i = 0; i < ZLabels.Length; i++)
            {
                ZLabels[i].text = YxUtiles.ReduceNumber(ZBet[i]);
                if (ZBet[i] > 0)
                    ZLabels[i].transform.parent.gameObject.SetActive(true);
            }
            if (self.Length == 0)
                return;
            WBet = self;
            for (int i = 0; i < WLabels.Length; i++)
            {
                WLabels[i].text = YxUtiles.ReduceNumber(WBet[i]);
                if (WBet[i] > 0) WLabels[i].transform.parent.gameObject.SetActive(true);
            }
        }

        public void RefreshNum(ISFSObject responseData)
        {
            var gdata = App.GetGameData<BrttzGameData>();
            int gold = responseData.GetInt(Parameter.Gold);
            string temp = responseData.GetUtfString(Parameter.P);
            int p = App.GetGameManager<BrttzGameManager>().BetCtrl.GetInt(temp);
            int seat = responseData.GetInt(Parameter.Seat);
            if (seat == gdata.GetPlayerInfo().Seat)
            {
                WBet[p] += gold;
                WLabels[p].text = YxUtiles.ReduceNumber(WBet[p]);
                WLabels[p].transform.parent.gameObject.SetActive(true);
            }
            ZBet[p] += gold;
            if (ZBet[p] > 0)
            {
                ZLabels[p].text = YxUtiles.ReduceNumber(ZBet[p]);
                ZLabels[p].transform.parent.gameObject.SetActive(true);
            }
            App.GetGameData<BrttzGameData>().ZNumber[p] += gold;
        }

        public void GroupRefreshNum(ISFSObject responseData)
        {
            if (!responseData.ContainsKey(Parameter.Coin))
                return;
            ISFSArray sfsArray = responseData.GetSFSArray(Parameter.Coin);
            var gdata = App.GetGameData<BrttzGameData>();
            int selfSeat = gdata.GetPlayerInfo().Seat;
            foreach (ISFSObject item in sfsArray)
            {
                int seat = item.GetInt(Parameter.Seat);
                if (seat == selfSeat)
                    continue;

                int gold = item.GetInt(Parameter.Gold);
                string p = item.GetUtfString(Parameter.P);
                int target = App.GetGameManager<BrttzGameManager>().BetCtrl.GetInt(p);
                ZBet[target] += gold;
                if (ZBet[target] > 0)
                {
                    ZLabels[target].text = YxUtiles.ReduceNumber(ZBet[target]);
                    ZLabels[target].transform.parent.gameObject.SetActive(true);
                }
                gdata.ZNumber[target] += gold;
            }
        }

        public void SetNum(ISFSObject responseData)
        {
            var gdata = App.GetGameData<BrttzGameData>();
            var zNumber = gdata.ZNumber;
            if (!responseData.ContainsKey("glist")) return;
            var golds = responseData.GetIntArray("glist");
            for (var i = 0; i < golds.Length; i++)
            {
                if (ZLabels.Length <= i) { return; }
                ZLabels[i].text = YxUtiles.ReduceNumber(golds[i]);
                zNumber[i] = golds[i];
            }
        }

        public void Reset()
        {
            var gdata = App.GetGameData<BrttzGameData>();
            var zNumber = gdata.ZNumber;
            var zNumberLen = zNumber.Length;

            if (WBet != null && WBet.Length > 0)
            {
                LastWBet = new int[WBet.Length];
                WBet.CopyTo(LastWBet, 0);
            }

            for (var i = 0; i < zNumberLen; i++)
            {
                zNumber[i] = 0;
                WBet[i] = 0;
                ZBet[i] = 0;
            }

            foreach (var label in ZLabels)
            {
                label.text = "0";
                label.transform.parent.gameObject.SetActive(false);
            }

            foreach (var label in WLabels)
            {
                label.text = "0";
                label.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
