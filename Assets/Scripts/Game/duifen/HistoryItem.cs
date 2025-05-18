using UnityEngine;
using System;
using System.Collections.Generic;
#pragma warning disable 649


namespace Assets.Scripts.Game.duifen
{
    public class HistoryItem : MonoBehaviour
    {

        /// <summary>
        /// 局数Label
        /// </summary>
        [SerializeField]
        private UILabel _turnLabel;

        [SerializeField]
        private UILabel _timeLabel;

        [SerializeField]
        private Transform _usersParent;

        [SerializeField]
        private UIWidget _widget;

        public int SizeY
        {
            get { return _widget.height; }
        }

        public void InitItem(HistoryItemInfo itemInfo)
        {

            List<HistoryUserInfo> userInfoList = itemInfo.Users;

            //先将所有的玩家信息隐藏
            foreach (Transform child in _usersParent)
            {
                child.gameObject.SetActive(false);
            }

   
            int count = userInfoList.Count;

            for (int i = 0; i < count; i++)            //不对应座位号
            {
                Transform child = _usersParent.GetChild(i);
                child.GetComponent<HistoryUser>().InitUser(userInfoList[i]);
                child.gameObject.SetActive(true);
            }

            _usersParent.GetComponent<UIGrid>().Reposition();
            InitTurn(itemInfo.RoundVal);
            InitTime(itemInfo.Time);
        }


        public void InitTurn(int count)
        {
            if (_turnLabel == null)
                return;

            string curTurn = count.ToString();
            _turnLabel.text = curTurn;
            gameObject.SetActive(true);
        }


        private void InitTime(long time)
        {
            if (_timeLabel == null)
                return;

            _timeLabel.text = ToRealTime(time);
        }

        /// <summary>
        /// 将时间戳转换为当前时间
        /// </summary>
        /// <param name="timpStamp"></param>
        /// <returns></returns>
        public string ToRealTime(long timpStamp)
        {
            if (timpStamp <= 0)
            {
                return "";
            }
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));  //时间戳开始时间
            long unixTime = timpStamp * 10000000L;
            TimeSpan toNow = new TimeSpan(unixTime);
            DateTime dt = dtStart.Add(toNow);
            return "时间 : " + string.Format("{0:yyyy/MM/dd HH:mm:ss}", dt);
        }
        



    }
}