using System.Collections;
using UnityEngine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongTimerCtrl : MahjongTablePart
    {
        public DigitelDisplay TimeNum;
        private Coroutine mTimeUpdataCor;

        public int Time
        {
            get { return TimeNum.Number; }
            set { TimeNum.SetTimer(value); }
        }

        private void Start() { Time = 0; }

        public override void OnReset()
        {
            Time = 0;
            TimeNum.gameObject.SetActive(false);
            if (mTimeUpdataCor != null)
            {
                StopCoroutine(mTimeUpdataCor);
            }
        }

        public void StartTimer(int time, Action callBack = null)
        {
            Time = time;
            TimeNum.gameObject.SetActive(true);
            if (mTimeUpdataCor != null)
            {
                StopCoroutine(mTimeUpdataCor);
            }
            if (gameObject.activeSelf)
            {
                mTimeUpdataCor = StartCoroutine(TimeUpdata(callBack));
            }
        }

        private IEnumerator TimeUpdata(Action callBack = null)
        {
            while (Time >= 0)
            {
                yield return new WaitForSeconds(1);
                Time--;
                var db = GameCenter.DataCenter;
                if (db.CurrOpChair == 0 && GameCenter.GameProcess.IsCurrState<StateGamePlaying>())
                {
                    if (Time <= 3 && Time >= 1)
                    {
                        MahjongUtility.PlayEnvironmentSound("clock");
                        if (MahjongUtility.ShakeCtrl == (int)CtrlSwitchType.Open && db.Config.MobileShake)
                        {
#if UNITY_ANDROID || UNITY_IOS
                            Handheld.Vibrate();
#endif
                        }
                    }
                    if (Time == 0)
                    {
                        Time = -1;
                        MahjongUtility.PlayEnvironmentSound("naozhong");                       
                    }
                }               
            }
            if (callBack != null)
            {
                callBack();
            }
        }

        public void StopTimer()
        {
            Time = 0;
            if (mTimeUpdataCor != null)
            {
                StopCoroutine(mTimeUpdataCor);
            }
        }
    }
}