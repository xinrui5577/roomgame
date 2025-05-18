using UnityEngine;
using System.Collections.Generic;
using System;
#pragma warning disable 649


namespace Assets.Scripts.Game.Texas.skin01
{
    public class HistoryItem : MonoBehaviour
    {

        //背景图片
        [SerializeField]
        private UISprite _backGround;

        [SerializeField]
        private UILabel _timeLabel;

        [SerializeField]
        private UILabel _roundLabel;

        [SerializeField]
        private Transform _usersParent;

        [SerializeField]
        private UISprite[] _lines;

        [SerializeField]
        private List<PokerCard> _publicPokers;

        public GameObject HisUserPrefab;

        /// <summary>
        /// 初始化单元信息
        /// </summary>
        /// <param name="itemInfo"></param>
        public void InitItem(HistoryItemInfo itemInfo)
        {
            InitTurn(itemInfo.RoundVal);
            InitTime(itemInfo.Time);
            InitPublicPokers(itemInfo.PublicPokers);

            //先将所有的玩家信息隐藏
            foreach (Transform child in _usersParent)
            {
                child.gameObject.SetActive(false);
            }

            List<HistoryUserInfo> userInfoList = itemInfo.Users;

            int count = userInfoList.Count;
            int setCount = (count - 1) / 3;

            SetLinesActive(setCount);
            SetBackGroundHeight(setCount);              //设置背景的高度

            for (int i = 0; i < count; i++)            //不对应座位号
            {
                var child = i >= _usersParent.childCount
                    ? _usersParent.gameObject.AddChild(HisUserPrefab)
                    : _usersParent.GetChild(i).gameObject;
                child.GetComponent<HistoryUser>().InitUser(userInfoList[i]);
            }
            var grid = _usersParent.GetComponent<UIGrid>();
            grid.repositionNow = true;
            grid.Reposition();
        }


        /// <summary>
        /// 初始化公共牌
        /// </summary>
        /// <param name="list"></param>
        private void InitPublicPokers(List<int> list)
        {
            int listCount = list.Count;
            for (int i = 0; i < _publicPokers.Count; i++)
            {
                if(i < listCount)
                {
                    var poker = _publicPokers[i];
                    poker.SetCardId(list[i]);
                    poker.SetCardFront();
                    poker.gameObject.SetActive(true);
                }
                else
                {
                    _publicPokers[i].gameObject.SetActive(false);
                }
            }
            //刷新公共牌位置
            _publicPokers[0].transform.GetComponentInParent<UIGrid>().Reposition();
        }


        /// <summary>
        /// 设置背景高度
        /// </summary>
        /// <param name="setCount"></param>
        private void SetBackGroundHeight(int setCount)
        {
            //背景的高度 = 首行的高度 + 显示的行数 * 每行的高度
            _backGround.height = (setCount + 1) * 156;
        }


        public void InitTurn(int count)
        {
            string curTurn = count.ToString();
            _roundLabel.text = curTurn;
            gameObject.SetActive(true);
        }
  
        private void InitTime(string time)
        {
            _timeLabel.text = time;
        }

        /// <summary>
        /// 显示线的个数
        /// </summary>
        /// <param name="showCount">显示的最高索引</param>
        private void SetLinesActive(int showCount)
        {
          
            if(showCount > _lines.Length)
            {
                com.yxixia.utile.YxDebug.YxDebug.Log(" === 要显示的线的个数错误 ==== ");
                return;
            }

            for (int i = 0; i < _lines.Length; i++)
            {
                _lines[i].gameObject.SetActive(i < showCount);
            }
        }

        /// <summary>
        /// 将时间戳转换为当前时间
        /// </summary>
        /// <param name="timpStamp"></param>
        /// <returns></returns>
        public string ToRealTime(long timpStamp)
        {
            if(timpStamp <= 0)
            {
                return "";
            }
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));  //时间戳开始时间
            long unixTime = timpStamp * 10000000L;
            TimeSpan toNow = new TimeSpan(unixTime);
            DateTime dt = dtStart.Add(toNow);
            return "时间 : " + string.Format("{0:yyyy/MM/dd HH:mm:ss}", dt);
        }

        [SerializeField]
        private TweenScale _tweenScale;

        private bool _isOpened;

        public void OnClickOpenBtn()
        {
            if (_isOpened)
            {
                _tweenScale.PlayReverse();
                _tweenScale.GetComponentInChildren<TweenScale>().PlayReverse();
                _tweenScale.GetComponentInChildren<TweenAlpha>().ResetToBeginning();
                _isOpened = false;
            }
            else
            {
                _tweenScale.PlayForward();
                _tweenScale.GetComponentInChildren<TweenAlpha>().PlayForward();
                _isOpened = true;
            }
        }
   }
}