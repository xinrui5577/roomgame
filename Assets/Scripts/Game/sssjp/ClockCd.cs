using System;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.sssjp
{
    public class ClockCd : MonoBehaviour
    {
        [SerializeField]
        private UILabel _timeLabel = null;

        public bool HaveWarning;

        public int WarningTime = 15;

        public int SpaceTime = 1;

        private Action _warning;

        private int TimeStart;//开始发牌的时间戳
        private int TimeEnd;//结束的时间戳
        private int TimeDifference;//时间差（当前时间擢-开始发牌的时间戳）

        private void Awake()
        {
            _warning = DoSpaceWarning;
        }
        float DownTime = 1;
        private void Update()
        {
            if (TimeDifference > 0)
            {
                DownTime -= Time.deltaTime;
                if (DownTime <= 0)
                {
                    DownTime = 1;
                    TimeDifference = TimeEnd - (int)GetTimeStamp();

                    _timeLabel.text = TimeDifference.ToString();

                    if (HaveWarning)
                    {
                        _warning();
                    }

                    if (TimeDifference >= 0) return;
                    StopCountDown();
                }
            }
        }
        /// <summary>
        /// 开始倒计时
        /// </summary>
        /// <param name="time"></param>
        public void BeginCountDown(int time,int startTime,bool firstbool)
        {
            if (firstbool)
            {
                TimeStart = (int)GetTimeStamp();
            }
            else
            {
                //TimeStart = startTime;
                TimeStart = (int)GetTimeStamp();
            }
            TimeEnd = TimeStart + time;
            Debug.Log("<color=#00FF2BFF>" + "本地器时间擢" + TimeEnd + "</color>");
            TimeDifference = TimeEnd - TimeStart;
            gameObject.SetActive(true);
        }
        //获取当前时间擢
        public static long GetTimeStamp(bool bflag = true)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long ret;
            if (bflag)
                ret = Convert.ToInt64(ts.TotalSeconds);
            else
                ret = Convert.ToInt64(ts.TotalMilliseconds);
            return ret;
        }

        /// <summary>
        /// 定时警告
        /// </summary>
        private void DoSpaceWarning()
        {
            if ( TimeDifference <= WarningTime && TimeDifference > 0 && TimeDifference % SpaceTime == 0)
            {
                DoWarning();
            }
        }
        /// <summary>
        /// 单次警告
        /// </summary>
        private void DoOnceWarning()
        {
            if (TimeDifference == WarningTime)
                DoWarning();
        }
       
        /// <summary>
        /// 警告
        /// </summary>
        void DoWarning()
        {
            #if (UNITY_EDITOR || UNITY_PC)
                YxDebug.Log("Time's Up! Time's Up!");
            #elif (UNITY_ANDROID || UNITY_IOS)
                //调用震动
                Handheld.Vibrate();    
            #endif
        }

        /// <summary>
        /// 停止倒计时
        /// </summary>
        public void StopCountDown()
        {
            _timeLabel.text = "0";
            StopAllCoroutines();
            gameObject.SetActive(false);
            CancelInvoke();
        }
        /// <summary>
        /// 时钟移动到目标父层级下
        /// </summary>
        /// <param name="parent">目标父层级</param>
        public void MoveClock(Transform parent)
        {
            transform.parent = parent;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
        }

    }
}