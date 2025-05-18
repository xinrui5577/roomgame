using System;
using UnityEngine;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using com.yxixia.utile.YxDebug;
using YxFramwork.Common;


namespace Assets.Scripts.Game.sss
{
    public class HistoryResultMgr : MonoBehaviour
    {
        public UIPanel Panel;
        public Transform HistoryItemsParent;
        public GameObject HistoryItemPrefab;

        public List<Transform> ItemList = new List<Transform>();

        public List<HistoryItemInfo> HistoryList = new List<HistoryItemInfo>();


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

        protected void OnEnable()
        {
            RefreshItemsCount();
            ShowHistoryViewInfo();
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
                ItemList.Add(go.transform);
            }
        }


        public void GetHistoryInfo(List<UserMatchInfo> matchInfos)
        {
            var gdata = App.GetGameData<SssGameData>();
            HistoryItemInfo historyInfo = new HistoryItemInfo
            {
                Users = new List<HistoryUserInfo>(),
                RoundVal = gdata.CurRound,
            };

            int bankerSeat = gdata.BankerSeat;
            foreach (UserMatchInfo info in matchInfos)
            {
                SssPlayer user = gdata.GetPlayer<SssPlayer>(info.Seat, true);
                if (user.Info == null)
                    continue;

                HistoryUserInfo userInfo = new HistoryUserInfo();
                userInfo.SssUserInfo = user.GetInfo<SssUserInfo>();
                userInfo.Seat = user.Info.Seat;
                userInfo.Pokers = info.Cards.ToArray();
                userInfo.AddScore = info.AddScore.ToArray();
                userInfo.NormalScore = info.NormalScores.ToArray();
                userInfo.Score = info.TtScore;
                userInfo.DunType = info.DunTypeList.ToArray();
                userInfo.Sprcial = info.Special;
                userInfo.ShootInfo = info.Shoot;
                userInfo.IsBanker = gdata.IsBankerModel && bankerSeat == user.Info.Seat;
                historyInfo.Users.Add(userInfo);
            }

            SetGetShootTime(historyInfo.Users);
            HistoryList.Add(historyInfo);

            if (MaxCount == 0)   //如果是0,则不限制个数
                return;

            while (HistoryList.Count > MaxCount)
            {
                HistoryList.Remove(HistoryList[0]);
            }
        }

        private void SetGetShootTime(List<HistoryUserInfo> users)
        {
            for (int i = 0; i < users.Count; i++)
            {
                HistoryUserInfo user = users[i];
                var shootTargs = user.ShootInfo.ShootTargs;
                if (shootTargs == null || shootTargs.Length < 1)
                    continue;
                for (int k = 0; k < shootTargs.Length; k++)
                {
                    for (int j = 0; j < users.Count; j++)
                    {
                        var getShootUser = users[j];
                        if (shootTargs[k] == getShootUser.SssUserInfo.Seat)
                        {
                            getShootUser.GetShootTime++;
                            users[j] = getShootUser;
                            break;
                        }
                    }
                }
            }
        }


        ///// <summary>
        ///// 将数据加入到战绩中
        ///// </summary>
        ///// <param name="data">传入的数据</param>
        //public void GetHistoryInfo(ISFSObject data)
        //{

        //    //解析人物牌信息
        //    ISFSArray dataArray = data.GetSFSArray("playerinfo");
        //    foreach (ISFSObject item in dataArray)
        //    {
        //        int seat = item.GetInt("seat");
        //        SssPlayer sssUser = main.UserSeatSort[main.ToLocalSeat(seat)];
        //        if (sssUser.CurSssUserInfo != null)
        //        {
        //            //存入玩家信息
        //            HistoryUserInfo sssUserInfo = new HistoryUserInfo
        //            {
        //                SssUserInfo = sssUser.CurSssUserInfo,
        //                Seat = seat,
        //            };

        //            //存入扑克信息
        //            List<int> cards = new List<int>();
        //            ISFSArray dunsInfo = item.GetSFSArray("duninfo");

        //            foreach (ISFSObject dun in dunsInfo)
        //            {
        //                int[] tempArrey = dun.GetIntArray("cards");
        //                foreach (int card in tempArrey)
        //                {
        //                    cards.Add(card);
        //                }
        //            }

        //            //把信息处理完的玩家加入列表
        //            sssUserInfo.Pokers = cards.ToArray();
        //            historyInfo.Users.Add(sssUserInfo);
        //        }
        //    }
        //    HistoryList.Add(historyInfo);

        //    if (MaxCount == 0)   //如果是0,则不限制个数
        //        return;


        //    while(HistoryList.Count > MaxCount)
        //    {
        //        HistoryList.Remove(HistoryList[0]);
        //    }

        //}

        public void AddHistoryInfo(ISFSObject data)
        {
            HistoryItemInfo curItem = HistoryList[HistoryList.Count - 1];
            List<HistoryUserInfo> usersInfo = curItem.Users;

            if (usersInfo == null || usersInfo.Count == 0)
            {
                YxDebug.LogError("HistoryItem init error,the HistoryUserInfo.users is NULL or EMPTY !!!!");
                return;
            }

            if (data.ContainsKey("st"))
            {
                curItem.Time = data.GetLong("st");
            }

            ISFSArray scoreArray = data.GetSFSArray("score");
            foreach (ISFSObject scoreItem in scoreArray)
            {
                int scoreSeat = scoreItem.GetInt("seat");

                for (int i = 0; i < usersInfo.Count; i++)
                {
                    HistoryUserInfo info = usersInfo[i];
                    if (info.Seat != scoreSeat)
                        continue;

                    info.Score = scoreItem.GetInt("score");
                    usersInfo[i] = info;        //由于是值类型,需要重新赋值
                }
            }
            HistoryList[HistoryList.Count - 1] = curItem;
        }

        #region  显示记录
        public void ShowHistoryViewInfo()
        {
            //倒序排列,优先显示最近完成的一局
            int count = HistoryList.Count;          //一共有多少个历史记录
            int showCount = MaxCount > count ? count : MaxCount;    //找到较小的那个

            int shiftY = 0;
            for (int i = 0; i < showCount; i++)
            {
                if (i >= ItemList.Count)
                    break;

                Transform itemTran = ItemList[count - 1 - i];
                itemTran.localPosition = new Vector3(0, shiftY, 0);

                HistoryItem itemHis = itemTran.GetComponent<HistoryItem>();
                itemHis.InitItem(HistoryList[count - 1 - i]);
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
        public void CheckOffset()
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

        #endregion


        public void OnClickCloseHistoryViewBtn()
        {
            transform.parent.gameObject.SetActive(false);
            HistoryItemsParent.DestroyChildren();
            ItemList.Clear();
        }

        public void Reset()
        {
            ItemList.Clear();
            HistoryList.Clear();
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
        public SssUserInfo SssUserInfo;
        public int Seat;
        public int[] Pokers;
        public int[] NormalScore;
        public int[] AddScore;
        public int[] DunType;
        public int Sprcial;
        public int Score;
        public ShootInfo ShootInfo;
        public int GetShootTime;
        public bool IsBanker;
    }
}


