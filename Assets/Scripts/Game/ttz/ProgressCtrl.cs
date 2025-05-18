using System;
using System.Globalization;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Manager;
using YxFramwork.Framework.Core;
using Assets.Scripts.Common.Adapters;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.ttz
{
    public class ProgressCtrl : MonoBehaviour
    {
        public GameObject Clock;
        public NguiLabelAdapter CountdownNum;
        public UISprite CountDownSprite;
        public UISprite StateSprite;
        private int _cdSum;
        private bool _timing;
        private float _cdStep = 0;

        private long _num;
        private bool _isBegin = false;

        public virtual void BeginCountdown(bool IsBeginBet = false)
        {
            //设置游戏状态
            Clock.SetActive(true);
            CancelInvoke("CyclePerformNum");
            InvokeRepeating("CyclePerformNum", 0, 1);
            //播放开始下注
            _isBegin = IsBeginBet;
            if (IsBeginBet) Facade.Instance<MusicManager>().Play("beginbet");
        }

        public virtual void EndCountdown()
        {
            Clock.SetActive(false);
            CancelInvoke("CyclePerformNum");
            if (_isBegin) Facade.Instance<MusicManager>().Play("stopbet");
            //App.GetGameData<BrttzGameData>().BeginBet = false;
        }

        protected virtual void CyclePerformNum()
        {
            int time = Int32.Parse(CountdownNum.Value) - 1;
            CountdownNum.Text(time.ToString(CultureInfo.InvariantCulture));
            CountDownSprite.fillAmount -= _cdStep;
            if (Int32.Parse(CountdownNum.Value) > 0) return;
            CountdownNum.Text("0");
            CountDownSprite.fillAmount = 0;
            EndCountdown();
        }

        public virtual void ReSetCountdown(int s, int type = -1)
        {
            CountdownNum.Text(s.ToString(CultureInfo.InvariantCulture));
            _cdStep = 1f / s;
            CountDownSprite.fillAmount = 1;
            switch (type)
            {
                case (int)RequestType.StartBet:
                    StateSprite.spriteName = "xiazhu";
                    break;
                case (int)RequestType.StopBet:
                    StateSprite.spriteName = "kaipai";
                    break;
                default:
                    StateSprite.spriteName = "wait";
                    break;
            }
        }

        public void RefreshNum(ISFSObject reSfsObject)
        {
            int gold = reSfsObject.GetInt("gold");
            _num = _num - gold; //(Int32.Parse(Num.text) - gold);
            App.GetGameData<BrttzGameData>().CurrentCanInGold = _num;
        }

        public void SetNumOnResult(ISFSObject response)
        {
            var gdata = App.GetGameData<BrttzGameData>();
            gdata.CurrentCanInGold = response.ContainsKey("bankerGold") ? response.GetLong("bankerGold") : 1147483647;
            _num = gdata.CurrentCanInGold;
        }

        public void SetNum(ISFSObject responseData)
        {
            var gdata = App.GetGameData<BrttzGameData>();
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
                gdata.CurrentCanInGold = _num;
                return;
            }
            if (bankers.Size() == 0)
            {
                gdata.CurrentCanInGold = 0;
                _num = 0;
            }
            else if (!gdata.BeginBet || bankers.Size() >= 1)
            {
                _num = gdata.BankerPlayer.Coin;
                gdata.CurrentCanInGold = _num;
            }
        }

        public void GroupRefreshNum(ISFSObject responseData)
        {
            if (!responseData.ContainsKey("coin"))
                return;

            var gdata = App.GetGameData<BrttzGameData>();

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
            _num = _num - total; //(Int32.Parse(Num.text) - gold);
            gdata.CurrentCanInGold = _num;
        }
    }
}