using System;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649

namespace Assets.Scripts.Game.sssjp
{
    public class HistoryItem : MonoBehaviour
    {

        [SerializeField]
        private UILabel _timeLabel;

        [SerializeField]
        private UILabel _turnLabel;

        [SerializeField]
        private UILabel _turnShadowLabel;


        [SerializeField]
        private Transform _usersParent;


        [SerializeField]
        private UISprite _line1;
 
        //背景图片
        [SerializeField]
        private UISprite _backGround;

        /// <summary>
        /// 背景图的高度
        /// </summary>
        public int SizeY
        {
            get { return _backGround.height; }
        }

        [SerializeField] private int[] _backGroundheight;

        [SerializeField] private GameObject[] _hLines;

        /// <summary>
        /// 初始化单元信息
        /// </summary>
        /// <param name="itemInfo"></param>
        public void InitItem(HistoryItemInfo itemInfo)
        {

            InitTurn(itemInfo.RoundVal);
            InitTime(itemInfo.Time);

            //先将所有的玩家信息隐藏
            foreach (Transform child in _usersParent)
            {
                child.gameObject.SetActive(false);
            }

            List<HistoryUserInfo> userInfoList = itemInfo.Users;

            int count = userInfoList.Count;
            int setCount = (count - 1) / 2;
            _line1.gameObject.SetActive(setCount > 0);

            int lens = _hLines.Length;
            for (int i = 0; i < lens; i++)
            {
                _hLines[i].SetActive(i < setCount);
            }
            _backGround.height = _backGroundheight[setCount];

            for (int i = 0; i < count; i++)            //不对应座位号
            {
                Transform child = _usersParent.GetChild(i);
                child.GetComponent<HistoryUser>().InitUser(userInfoList[i]);
            }
        }


        public void InitTurn(int count)
        {
            string curTurn = string.Format("第{0}局", count);
            _turnLabel.text = curTurn;
            _turnShadowLabel.text = curTurn;
            gameObject.SetActive(true);
        }


        private void InitTime(long time)
        {
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