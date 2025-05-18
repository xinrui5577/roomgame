using UnityEngine;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using System;
using System.Globalization;
using Assets.Scripts.Game.paijiu.ImgPress.Main;
#pragma warning disable 649


namespace Assets.Scripts.Game.paijiu
{
    public class HistoryResultMgr : MonoBehaviour
    {

        public UIPanel Panel;
        public Transform HistoryItemsParent;
        public GameObject HistoryItemPrefab;
        public GameObject Containser;

        public List<Transform> ItemList = new List<Transform>();

        public List<HistoryItemInfo> HistoryList = new List<HistoryItemInfo>();

        public UILabel RoomId;

        public UILabel Round;

        public UILabel TimeLabel;

        [SerializeField]
        private GameObject _info;

        private string _joinTime;

        public void SetRoomInfo(ISFSObject data)
        {
            _joinTime = ToRealTime(data.GetLong("svt"));
        }

        /// <summary>
        /// 时间戳的起始时间
        /// </summary>
        DateTime _dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

        /// <summary>
        /// 将时间戳转换为正常时间
        /// </summary>
        /// <param name="timpStamp">服务器传输的时间数据</param>
        /// <returns></returns>
        string ToRealTime(long timpStamp)
        {
            long unixTime = timpStamp * 10000000L;
            TimeSpan toNow = new TimeSpan(unixTime);
            DateTime dt = _dtStart.Add(toNow);
            return dt.ToString("yyyy/MM/dd     HH:mm:ss");
        }


        /// <summary>
        /// 结算结果是否是固定个数循环播放的
        /// </summary>
        [Tooltip("结果是否是循环播放的")]
        public bool Loop = false;

        /// <summary>
        /// 一共需要创建多少个结算单位实例
        /// </summary>
        [Tooltip("循环模式中,表示记录多少个结果")]
        public int MaxCount = 10;

        HistoryItemInfo _curHistoryItemInfo;

        /// <summary>
        /// 放于游戏开始交互,记录参与游戏的玩家信息
        /// </summary>
        /// <param name="seats"></param>
        public void CreateHistoryItem(int[] seats)
        {
            _curHistoryItemInfo = new HistoryItemInfo
            {
                Users = new List<HistoryUserInfo>(),
                RoundVal = App.GetGameData<PaiJiuGameData>().CurRound
            };
            var userList = App.GetGameData<PaiJiuGameData>().PlayerList;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < seats.Length; i++)
            {
                int seat = seats[i];
                PaiJiuUserInfo paiJiuUserInfo = (PaiJiuUserInfo)userList[seat].Info;
                if (paiJiuUserInfo != null)
                {
                    HistoryUserInfo hui = new HistoryUserInfo
                    {
                        Seat = seat,
                        PaiJiuUserInfo = paiJiuUserInfo,
                    };

                    _curHistoryItemInfo.Users.Add(hui);
                }
            }
        }

        public void ShowHistoryView()
        {
            Containser.SetActive(true);
            UpdateRoomInfo();
            RefreshItemsCount();
            ShowHistoryViewInfo();
        }

        private void UpdateRoomInfo()
        {
            if (App.GetGameData<PaiJiuGameData>().IsRoomGame)
            {
                _info.SetActive(true);
                var roomInfo = App.GetGameManager<PaiJiuGameManager>().RoomInfo;
                TimeLabel.text = _joinTime;
                Round.text = roomInfo.CurrentRound + "/" + roomInfo.MaxRound;
                RoomId.text = roomInfo.RoomID.ToString();
            }
            else
            {
                _info.SetActive(false);
                TimeLabel.text = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            }
        }


        void RefreshItemsCount()
        {
            int listCount = HistoryList.Count;
            int count = MaxCount > listCount ? listCount : MaxCount;//获取信息个数和最大单元个数中较小的那个
            if (ItemList.Count >= count)
                return;

            int dif = count - ItemList.Count;
            for (int i = 0; i < dif; i++)
            {
                GameObject go = Instantiate(HistoryItemPrefab);
                go.SetActive(true);
                Transform goTran = go.transform;
                goTran.parent = HistoryItemsParent;
                goTran.localScale = Vector3.one;
                goTran.localPosition = HistoryItemPrefab.transform.localPosition;
                ItemList.Add(go.transform);
            }
        }


        /// <summary>
        /// 将数据加入到战绩中
        /// </summary>
        /// <param name="data">传入的数据</param>
        public void GetHistoryInfo(ISFSObject data)
        {
            if (_curHistoryItemInfo.RoundVal <= 0)
                return;

            ISFSArray dataArray = data.GetSFSArray("players");
            foreach (ISFSObject item in dataArray)
            {
                if (!item.ContainsKey("isPlayed") || !item.GetBool("isPlayed"))
                {
                    continue;
                }

                int seat = item.GetInt("seat");
                List<HistoryUserInfo> hisUsers = _curHistoryItemInfo.Users;

                for (int i = 0; i < hisUsers.Count; i++)
                {
                    HistoryUserInfo tempUser = hisUsers[i];
                    if (tempUser.Seat != seat)
                    {
                        continue;
                    }

                    tempUser.IsFold = item.GetBool("isGiveUp");

                    tempUser.Score = item.GetInt("gold");
                    hisUsers[i] = tempUser;
                }
            }

            HistoryList.Add(_curHistoryItemInfo);


            //调整存储的个数
            if (MaxCount == 0)   //如果是0,则不限制个数
                return;
            while (HistoryList.Count > MaxCount)
            {
                HistoryList.Remove(HistoryList[0]);
            }

        }



        public void ShowHistoryViewInfo()
        {
            if (HistoryList == null || HistoryList.Count <= 0)
            {
                return;
            }

            //倒序排列,优先显示最近完成的一局
            int count = HistoryList.Count;          //一共有多少个历史记录
            int showCount = MaxCount > count ? count : MaxCount;    //找到较小的那个
            //int showCount = 7;    //用于测试

            int shiftY = 0;
            for (int i = 0; i < showCount; i++)
            {
                if (i >= ItemList.Count)
                    break;

                Transform itemTran = ItemList[count - 1 - i];
                //Transform itemTran = ItemList[showCount - 1 - i];   //用于测试
                itemTran.localPosition = new Vector3(itemTran.localPosition.x, shiftY, 0);

                HistoryItem itemHis = itemTran.GetComponent<HistoryItem>();
                itemHis.InitItem(HistoryList[count - 1 - i]);
                itemHis.InitItem(HistoryList[showCount - 1 - i]);  //用于测试
                shiftY = shiftY - itemHis.SizeY - 20;
            }
        }

        private float _offsetY;
        private int _count = 1;
        /// <summary>
        /// 每个单元的纵向间距
        /// </summary>
        private float _lineSpaceY = 370;

        /// <summary>
        /// 循环信息,引用与ScrollBar的OnValueChange
        /// </summary>
        public void OnDragValueChanged()
        {

            if (!Loop)  //如果不是循环模式,删除
                return;

            if (Math.Abs(_offsetY - Panel.clipOffset.y) <= 0)
                return;


            _offsetY = Panel.clipOffset.y;

            if (_offsetY < -_lineSpaceY * _count + 1 && _count + ItemList.Count - 1 < HistoryList.Count)
            {
                int index = (_count - 1) % ItemList.Count;
                Transform tempTran = ItemList[index];
                Vector3 tempV3 = tempTran.localPosition;
                tempTran.localPosition = new Vector3(tempV3.x, tempV3.y - _lineSpaceY * ItemList.Count, tempV3.z);
                ++_count;
                ItemList[index].GetComponent<HistoryItem>().InitItem(HistoryList[_count + 3]);
            }
            else if (_offsetY > -_lineSpaceY * _count && _count - 1 > 0)
            {

                int index = (_count - 2) % ItemList.Count;
                Transform tempTran = ItemList[index];
                Vector3 tempV3 = tempTran.localPosition;
                tempTran.localPosition = new Vector3(tempV3.x, tempV3.y + _lineSpaceY * ItemList.Count, tempV3.z);
                --_count;
                ItemList[index].GetComponent<HistoryItem>().InitItem(HistoryList[_count - 1]);
            }
        }

        public void OnClickCloseHistoryViewBtn()
        {
            Containser.SetActive(false);
        }

        public void Reset()
        {
            ItemList.Clear();
            HistoryList.Clear();
            _curHistoryItemInfo = new HistoryItemInfo();
        }
    }
    public struct HistoryItemInfo
    {
        public List<HistoryUserInfo> Users;
        public int RoundVal;
        public long Time;
    }

    public struct HistoryUserInfo
    {
        public PaiJiuUserInfo PaiJiuUserInfo;
        public int Seat;
        public List<int> Pokers;
        public int Score;
        public bool IsFold;
    }
}


