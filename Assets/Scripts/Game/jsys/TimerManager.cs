using System;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jsys
{
    public class TimerManager : MonoBehaviour
    {
        public Text Timertext;

        public Transform TimerImg;

        public Text TimeTextBotton;

        private long _currTimer;

        private Action _callBack;
        private float currTimer;

        public void GameXiazhuBack()
        {
            var gameMgr = App.GetGameManager<JsysGameManager>();
            var musicMgr = Facade.Instance<MusicManager>();
            musicMgr.Stop();
            musicMgr.Play("Dengdai");
            gameMgr.TurnGroupsMgr.GameConfig.TurnTableState = (int)GameConfig.GoldSharkState.Bet;
            if (!gameMgr.TurnGroupsMgr.GameConfig.IsBetPanelOnShow)
            {
                gameMgr.BetPanelMgr.ShowUI();
            }
        }
        public void SetClock(long s)
        {
            if (s == 0)
            {
                return;
            }
            _currTimer = s;
            InvokeRepeating("CountDown", 1, 1);
        }
        private void CountDown()
        {
            _currTimer--;

            if (_currTimer <= 0)
            {
                TimeTextBotton.text = "0";
                TimerImg.gameObject.SetActive(false);
                Timertext.text = "0";
                CancelInvoke("CountDown");
                return;
            }
            TimerImg.gameObject.SetActive(true);

            Timertext.text = _currTimer + "";
            TimeTextBotton.text = _currTimer + "";
        }

        private int waittime;
        public void Wait(int s)
        {
            waittime = s;
            InvokeRepeating("PanDuan", 1, 1);
        }
        private void PanDuan()
        {
            waittime--;
            if (waittime <= 0)
            {
                CancelInvoke("PanDuan");
            }
        }
    }
}
