using System;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.BaiTuan
{
    public class ProgressCtrl : MonoBehaviour
    {

        public UILabel Num;
        public UILabel CountdownNum;
        public UILabel CountdownNum1;
        public UILabel CountdownNum2;

        private long _num;

        public void RefreshNum(ISFSObject responseData)
        {
            var gold = responseData.GetInt("gold");
            var temp = _num - gold;
            Num.text = YxUtiles.ReduceNumber(temp);
            App.GetGameData<BtwGameData>().CurrentCanInGold = temp;
        }

        public void RefreshNum(int gold)
        {
            _num -= gold;
            Num.text = YxUtiles.ReduceNumber(_num);
            App.GetGameData<BtwGameData>().CurrentCanInGold = _num;
        }

        //private float _speedTarget = 1;

        public void SetNumOnResult(ISFSObject response)
        {
            var gdata = App.GetGameData<BtwGameData>();
            gdata.CurrentCanInGold = response.ContainsKey("bankerGold") ? response.GetLong("bankerGold") : 1147483647;
            _num = gdata.CurrentCanInGold;
        }

        public void SetNum(ISFSObject responseData)
        {
            var gdata = App.GetGameData<BtwGameData>();
            if (!responseData.ContainsKey("bankers")) return;
            var bankers = responseData.GetSFSArray("bankers");
            var b = responseData.GetInt("banker");
            YxBaseGameUserInfo info = null;
            if (b == -1)
            {
                info = new YxBaseGameUserInfo
                {
                    NickM = "系统",
                    CoinA = 1147483647,
                    Seat = -1,
                    TotalCount = 0,
                    WinTotalCoin = 0
                };
                gdata.BankerPlayer.SetMaxCoin(1147483646);
                gdata.BankerPlayer.Info = info;
                _num = 1147483646;
                Num.text = "∞";
                gdata.CurrentCanInGold = _num;
                return;
            }
            if (bankers.Size() == 0)
            {
                gdata.CurrentCanInGold = 0;
                _num = 0;
                Num.text = "0";
            }
            else if (!gdata.BeginBet || bankers.Size() >= 1)
            {
                _num = gdata.BankerPlayer.Coin;
                Num.text = YxUtiles.ReduceNumber(_num);
                gdata.CurrentCanInGold = _num;
            }
        }

        public void GroupRefreshNum(ISFSObject responseData)
        {
            if (!responseData.ContainsKey("coin"))
                return;

            var gdata = App.GetGameData<BtwGameData>();

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
            _num -= total; //(Int32.Parse(Num.text) - gold);
            Num.text = YxUtiles.ReduceNumber(_num);
            gdata.CurrentCanInGold = _num;
        }

        public virtual void BeginCountdown()
        {
            CancelInvoke("CyclePerform_Num");
            InvokeRepeating("CyclePerform_Num", 0, 1);
        }

        public virtual void CyclePerform_Num()
        {
            Facade.Instance<MusicManager>().Play("Clock");
            CountdownNum.text = (int.Parse(CountdownNum.text) - 1) + "";
            CountdownNum1.text = (int.Parse(CountdownNum.text) - 1) / 10 + "";
            CountdownNum2.text = (int.Parse(CountdownNum.text) - 1) % 10 + "";
            if (int.Parse(CountdownNum.text) > 0) return;
            CountdownNum2.text = 0 + "";
            EndCountdown();
        }


        public virtual void EndCountdown()
        {
            CancelInvoke("CyclePerform_Num");
            App.GetGameData<BtwGameData>().BeginBet = false;
        }

        public virtual void ReSetCountdown(int s)
        {
            CountdownNum.text = s + "";
        }
    }
}