using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.bjlb
{
    public class ShowNumCtrl : MonoBehaviour
    {
        public UILabel[] ZLabels;
        public UILabel[] WLabels;

        /// <summary>
        /// 总下注的数组
        /// </summary>
        internal int[] ZBet;

        /// <summary>
        /// 自己下注的数组
        /// </summary>
        internal int[] WBet;

       public void Init()
        {
            App.GetGameData<BjlGameData>().ZNumber = new int[ZLabels.Length];
            ZBet = new int[ZLabels.Length];
            WBet = new int[WLabels.Length];
        }

        public void RefreshNum(ISFSObject responseData)
        {
            var gold = responseData.GetInt("gold");
            var p = responseData.GetInt("p");
            var gdata = App.GetGameData<BjlGameData>();

            var seat = responseData.GetInt("seat");
            if (seat == gdata.SelfSeat)
            {
                WBet[p] += gold;
                var wLabel = WLabels[p];
                SetNumLabel(wLabel, WBet[p]);
            }

            ZBet[p] += gold;
            SetNumLabel(ZLabels[p], ZBet[p]);
            App.GetGameData<BjlGameData>().ZNumber[p] += gold;
        }
        public void SetNum(ISFSObject responseData)
        {
            var gdata = App.GetGameData<BjlGameData>();
            var zNumber = gdata.ZNumber;
            var golds = responseData.GetIntArray("glist");
            for (var i = 0; i < golds.Length; i++)
            {
                if (ZLabels.Length <= i) break;
                var gold = golds[i];
                SetNumLabel(ZLabels[i], gold);
                zNumber[i] = gold;
                ZBet[i] = gold;
            }
        }


        ///// <summary>
        ///// 进入游戏时,显示已下注数值
        ///// </summary>
        ///// <param name="glist"></param>
        //public void SetNum(int[] glist)
        //{
        //    int len = glist.Length;
        //    int labelLen = ZLabels.Length;
        //    for (int i = 0; i < len; i++)
        //    {
        //        if (i >= labelLen) break;
        //        var gold = glist[i];
        //        SetNumLabel(ZLabels[i], gold);
        //        ZBet[i] = gold;
        //    }
        //}


        /// <summary>
        /// 设置label内容
        /// </summary>
        /// <param name="label"></param>
        /// <param name="gold"></param>
        void SetNumLabel(UILabel label,int gold)
        {
            label.text = YxUtiles.ReduceNumber(gold);
            label.gameObject.SetActive(gold > 0);
        }

        public virtual void ReSet()
        {
            var gdata = App.GetGameData<BjlGameData>();
            var zNumber = gdata.ZNumber;
            var zNumberLen = zNumber.Length;
            for (var i = 0; i < zNumberLen; i++)
            {
                zNumber[i] = 0;
                WBet[i] = 0;
                ZBet[i] = 0;
            }

            foreach (var label in ZLabels)
            {
                SetNumLabel(label, 0);
            }
            foreach (var label in WLabels)
            {
                SetNumLabel(label, 0);
            }
        }

        public void GroupRefreshNum(ISFSObject responseData)
        {
            if (!responseData.ContainsKey("coin"))
                return;
            ISFSArray sfsArray = responseData.GetSFSArray("coin");
            var gdata = App.GetGameData<BjlGameData>();
            int selfSeat = gdata.GetPlayerInfo().Seat;
            foreach (ISFSObject item in sfsArray)
            {
                int seat = item.GetInt("seat");
                if (seat == selfSeat)
                    continue;

                int gold = item.GetInt("gold");
                int p = item.GetInt("p");
                ZBet[p] += gold;
                SetNumLabel(ZLabels[p], ZBet[p]);
                gdata.ZNumber[p] += gold;
            }
        }



    }
}
