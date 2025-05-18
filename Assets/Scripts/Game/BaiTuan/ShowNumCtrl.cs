using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.BaiTuan
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
            App.GetGameData<BtwGameData>().ZNumber = new int[zlen];



            ZBet = new int[zlen];
            WBet = new int[wlen];
        }

        public void Init(int[] bets, int[] self)
        {
            ZBet = bets;
            for (int i = 0; i < ZLabels.Length; i++)
            {
                ZLabels[i].text = YxUtiles.ReduceNumber(ZBet[i]);
                if (ZBet[i] > 0) ZLabels[i].transform.parent.gameObject.SetActive(true);
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
            var gdata = App.GetGameData<BtwGameData>();
            int gold = responseData.GetInt("gold");
            int p = responseData.GetInt("p");
            int seat = responseData.GetInt("seat");
            if (seat == gdata.GetPlayerInfo().Seat)
            {
                WBet[p] += gold;
                WLabels[p].text = YxUtiles.ReduceNumber(WBet[p]);
                WLabels[p].gameObject.SetActive(true);
            }
            ZBet[p] += gold;
            ZLabels[p].text = YxUtiles.ReduceNumber(ZBet[p]);
            ZLabels[p].gameObject.SetActive(true);
            App.GetGameData<BtwGameData>().ZNumber[p] += gold;
        }

        public void GroupRefreshNum(ISFSObject responseData)
        {
            if (!responseData.ContainsKey("coin"))
                return;
            ISFSArray sfsArray = responseData.GetSFSArray("coin");
            var gdata = App.GetGameData<BtwGameData>();
            int selfSeat = gdata.GetPlayerInfo().Seat;
            foreach (ISFSObject item in sfsArray)
            {
                int seat = item.GetInt("seat");
                if (seat == selfSeat)
                    continue;

                int gold = item.GetInt("gold");
                int p = item.GetInt("p");
                ZBet[p] += gold;
                ZLabels[p].text = YxUtiles.ReduceNumber(ZBet[p]);
                ZLabels[p].gameObject.SetActive(true);
                gdata.ZNumber[p] += gold;
            }
        }

        public void SetNum(ISFSObject responseData)
        {
            var gdata = App.GetGameData<BtwGameData>();
            var zNumber = gdata.ZNumber;
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
            var gdata = App.GetGameData<BtwGameData>();
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
                label.gameObject.SetActive(false);
            }

            foreach (var label in WLabels)
            {
                label.text = "0";
                label.gameObject.SetActive(false);
            }
        }
    }
}