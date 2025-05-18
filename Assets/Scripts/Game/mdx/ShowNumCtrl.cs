using Assets.Scripts.Common.Adapters;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.mdx
{
    public class ShowNumCtrl : MonoBehaviour
    {
        public NguiLabelAdapter[] ZLabels;
        public NguiLabelAdapter[] MaxLabels;

        public NguiLabelAdapter[] SelfBetLabel;

        /// <summary>
        /// 总下注的数组
        /// </summary>
        internal int[] ZBet;

        /// <summary>
        /// 自己下注的数组
        /// </summary>
        internal int[] MaxBet;

        internal int[] SelfBet;

       public void Init()
       {
            App.GetGameData<MdxGameData>().MaxBet = new int[ZLabels.Length];
            ZBet = new int[ZLabels.Length];
            MaxBet = new int[MaxLabels.Length];
            SelfBet = new int[SelfBetLabel.Length];
       }

        public void OnChangeBanker(int max)
        {
            var gdata = App.GetGameData<MdxGameData>();
            int bankSeat = gdata.BankSeat;
            for (int i = 0; i < MaxLabels.Length; i++)
            {
                MaxBet[i] = max;
                gdata.MaxBet[i] = max;
                if (bankSeat < 0)
                {
                    MaxLabels[i].Label.text = "--";
                }
                else
                {
                    MaxLabels[i].Text(MaxBet[i]);
                }
            }
        }

        public void SetOneBet(ISFSObject responseData)
        {
            var gold = responseData.GetInt("gold");
            var p = MdxTools.GetP(responseData.GetUtfString("p"));
            int seat = responseData.GetInt("seat");
            if (seat == App.GameData.SelfSeat)
            {
                SetSelfBet(p, gold);
            }
            SetLabels(p, gold);
        }

        public void SetGroupBet(ISFSObject responseData)
        {
            //var gdata = App.GetGameData<MdxGameData>();
            //var zNumber = gdata.MaxBet;
            var golds = responseData.GetIntArray("glist");
            for (var i = 0; i < golds.Length; i++)
            {
                if (ZLabels.Length <= i) break;
                var gold = golds[i];
                SetLabels(i, gold);
                //var gold = golds[i];
                //SetNumLabel(ZLabels[i], gold);
                //zNumber[i] = gold;
                //ZBet[i] = gold;
            }
        }   

        


        public void SetLabels(int p, int gold)
        {
            //一边下注,其下注的上限减少,另一边的下注上限增加
            int len = MaxBet.Length;
            int otherOne = (p + 1) % len;
            ZBet[p] += gold;

            var gdata = App.GetGameData<MdxGameData>();
            if (gdata.BankSeat < 0) return;
            MaxBet[p] -= gold;
            MaxBet[otherOne] += gold;
            SetNumLabel(ZLabels[p], ZBet[p]);
            SetNumLabel(MaxLabels[p], MaxBet[p]);
            SetNumLabel(MaxLabels[otherOne], MaxBet[otherOne]);

            //保存游戏数据,用于判断本玩家下注数额上限
            gdata.MaxBet[p] -= gold;
            gdata.MaxBet[otherOne] += gold;
        }

        void SetSelfBet(int p, int gold)
        {
            SelfBet[p] += gold;
            SetNumLabel(SelfBetLabel[p], SelfBet[p]);
            SelfBetLabel[p].gameObject.SetActive(true);
        }


        ///// <summary>
        ///// 进入游戏时,显示已下注数值
        ///// </summary>
        ///// <param name="glist"></param>
        //public void SetGroupBet(int[] glist)
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
        void SetNumLabel(NguiLabelAdapter label,int gold)
        {
            label.Text(gold);
        }

        public virtual void ReSet()
        {
            var gdata = App.GetGameData<MdxGameData>();
            var zNumber = gdata.MaxBet;
            var zNumberLen = zNumber.Length;
            int bankSeat = gdata.BankSeat;
            for (var i = 0; i < zNumberLen; i++)
            {
                ZBet[i] = 0;
                if (bankSeat < 0)
                {
                    MaxLabels[i].Label.text = "--";
                }
                else
                {
                    SetNumLabel(MaxLabels[i], MaxBet[i]);
                }
                SetNumLabel(ZLabels[i], 0);
                SelfBetLabel[i].gameObject.SetActive(false);
                SelfBet[i] = 0;
            }
        }

        public void GroupRefreshNum(ISFSObject responseData)
        {
            if (!responseData.ContainsKey("coin"))
                return;
            ISFSArray sfsArray = responseData.GetSFSArray("coin");
            var gdata = App.GetGameData<MdxGameData>();
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
                gdata.MaxBet[p] += gold;

            }
        }
    }
}
