using System;
using System.Globalization;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.brnn
{
    public class ProgressCtrl : MonoBehaviour
    {

        public UILabel Num;
        public UILabel CountdownNum;
        public UILabel StatusLabel;

        protected int NumVal;


        public void RefreshNum(ISFSObject responseData)
        {
            int gold = responseData.GetInt("gold");
            NumVal = NumVal - gold; //(Int32.Parse(Num.text) - gold);
            Num.text = YxUtiles.ReduceNumber(NumVal);
            App.GetGameData<BrnnGameData>().CurrentCanInGold = NumVal;
        }

        public void SetNum(ISFSObject responseData)
        {
            ISFSArray banbers = responseData.GetSFSArray("bankers");
            var gdata = App.GetGameData<BrnnGameData>();
            if (banbers == null || banbers.Size() == 0)
            {
                gdata.CurrentCanInGold = 0;
                Num.text = "0";
                NumVal = 0;
            }
            else
            {
                NumVal = Convert.ToInt32((int)gdata.CurrentBanker.Coin / gdata.MaxNiuRate);
                Num.text = YxUtiles.ReduceNumber(NumVal);
                gdata.CurrentCanInGold = NumVal;
            }
        }

        public virtual void BeginCountdown()
        {
            StatusLabel.text = "下注时间";
            CancelInvoke("CyclePerform_Num");
            InvokeRepeating("CyclePerform_Num", 0, 1);
        }

        public virtual void CyclePerform_Num()
        {
            Facade.Instance<MusicManager>().Play("Clock");
            CountdownNum.text = (Int32.Parse(CountdownNum.text) - 1).ToString(CultureInfo.InvariantCulture);
            if (Int32.Parse(CountdownNum.text) > 0) return;
            CountdownNum.text = "0";
            EndCountdown();
        }

        public void GroupRefreshNum(ISFSObject responseData)
        {
            if (!responseData.ContainsKey("coin"))
                return;

            var gdata = App.GetGameData<BrnnGameData>();

            ISFSArray sfsArray = responseData.GetSFSArray("coin");
            int total = 0;
            var selfSeat = gdata.GetPlayerInfo().Seat;
            foreach (ISFSObject item in sfsArray)
            {
                if (selfSeat == item.GetInt("seat"))
                {
                    continue;
                }
                total += item.GetInt("gold");
            }
            NumVal = NumVal - total; //(Int32.Parse(Num.text) - gold);
            Num.text = YxUtiles.ReduceNumber(NumVal);
            gdata.CurrentCanInGold = NumVal;
        }

        public virtual void EndCountdown()
        {
            StatusLabel.text = "开牌时间";
            CancelInvoke("CyclePerform_Num");
            App.GetGameData<BrnnGameData>().BeginBet = false;
        }

        public virtual void ReSetCountdown(int s)
        {
            CountdownNum.text = s.ToString(CultureInfo.InvariantCulture);
        }
    }
}