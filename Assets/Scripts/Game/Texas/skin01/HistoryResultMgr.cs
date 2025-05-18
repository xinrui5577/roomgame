using UnityEngine;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using Assets.Scripts.Game.Texas.Main;
using YxFramwork.Common.Model;


namespace Assets.Scripts.Game.Texas.skin01
{
    public class HistoryResultMgr : MonoBehaviour
    {

        /// <summary>
        /// 显示的窗口
        /// </summary>
        public UIPanel ScrollView;

        /// <summary>
        /// 设置记录的单元格父层级
        /// </summary>
        public Transform HistoryItemsParent;

        /// <summary>
        /// 战绩记录的单元格预制
        /// </summary>
        public GameObject HistoryItemPrefab;

        /// <summary>
        /// 战绩记录的界面物体
        /// </summary>
        public GameObject Containser;

        [HideInInspector]
        public List<Transform> ItemList = new List<Transform>();
      
        public List<HistoryItemInfo> HistoryList = new List<HistoryItemInfo>();

        public UITable Table;
      
        /// <summary>
        /// 一共需要创建多少个结算单位实例
        /// </summary>
        [Tooltip("循环模式中,表示记录多少个结果")]
        public int MaxCount = 10;


        HistoryItemInfo _curHistoryItemInfo = new HistoryItemInfo();

        public void CreateHistoryItem(ISFSObject data)
        {
            var gdata = App.GetGameData<TexasGameData>();
            _curHistoryItemInfo = new HistoryItemInfo
            {
                Users = new List<HistoryUserInfo>(),
                RoundVal = gdata.CurRound
            };

            var playerList = gdata.PlayerList;
            
            foreach (var player in playerList)
            {
                if (!player.ReadyState) continue;
                var userInfo = player.Info;
                if (userInfo != null)
                {
                    var hui = new HistoryUserInfo
                    {
                        Seat = userInfo.Seat,
                        UserInfo = userInfo,
                    };
                    _curHistoryItemInfo.Users.Add(hui);
                }
            }
        }


        public void SetHistoryItemTime(string timeStr)
        {
            _curHistoryItemInfo.Time = timeStr;
        }

        /// <summary>
        /// 显示战绩界面
        /// </summary>
        public void ShowHistoryView()
        {
            Containser.SetActive(true);
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
                goTran.parent = Table.transform;
                goTran.localScale = Vector3.one;
                ItemList.Add(goTran);
            }
            Table.Reposition();
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
                //没有参与的玩家不用处理
                if (!item.GetBool("isPlayed"))
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
                    tempUser.Pokers = ToList(item.GetIntArray("cardArr"));

                    if (item.ContainsKey("teax"))
                        tempUser.PokerType = item.GetSFSObject("teax").GetInt("type");
                    else
                        tempUser.PokerType = 0;

                    hisUsers[i] = tempUser;
                }

                _curHistoryItemInfo.PublicPokers = GetPublicPokerVal();

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

        /// <summary>
        /// 将数组转换为列表
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        List<int> ToList(int[] cards)
        {
            List<int> cardList = new List<int>();
            foreach (var card in cards)
            {
                cardList.Add(card);
            }
            return cardList;
        }
       


        public List<int> GetPublicPokerVal()
        {

            var publicPokers = App.GetGameManager<TexasGameManager>().PublicPokers;
            List<int> valueList = new List<int>();

            foreach (var poker in publicPokers)
            {
                valueList.Add(poker.Id);
            }

            return valueList;
        }

        /// <summary>
        /// 利用table排位
        /// </summary>
        public void ShowHistoryViewInfo()
        {
            //倒序排列,优先显示最近完成的一局
            int count = HistoryList.Count;          //一共有多少个历史记录
            int showCount = MaxCount > count ? count : MaxCount;    //找到较小的那个

            //int shiftY = 0;
            for (int i = 0; i < showCount; i++)
            {
                if (i >= ItemList.Count)
                    break;

                Transform itemTran = ItemList[i];//[count - 1 - i];

                HistoryItem itemHis = itemTran.GetComponent<HistoryItem>();
                itemHis.InitItem(HistoryList[count - 1 - i]);
            }

            Table.Reposition();
        }


        //显示历史纪录,手动排位
        //public void ShowHistoryViewInfo()
        //{
        //    if (HistoryList == null || HistoryList.Count <= 0)
        //    {
        //        return;
        //    }

        //    //倒序排列,优先显示最近完成的一局
        //    int count = HistoryList.Count;          //一共有多少个历史记录
        //    int showCount = MaxCount > count ? count : MaxCount;    //找到较小的那个

        //    //int shiftY = 0;
        //    int shiftY = -90;
        //    for (int i = 0; i < showCount; i++)
        //    {
        //        if (i >= ItemList.Count)
        //            break;

        //        Transform itemTran = ItemList[count - 1 - i];
        //        itemTran.localPosition = new Vector3(HistoryItemPrefab.transform.localPosition.x, shiftY, 0);

        //        HistoryItem itemHis = itemTran.GetComponent<HistoryItem>();
        //        itemHis.InitItem(HistoryList[count - 1 - i]);
        //        shiftY = shiftY - itemHis.SizeY - 90;
        //    }
        //}


        /// <summary>
        /// 循环信息,引用与ScrollBar的OnValueChange
        /// </summary>
        //public void CheckOffset()
        //{

        //    if (!Loop)  //如果不是循环模式,删除
        //        return;

        //    if (_offset_Y == ScrollView.clipOffset.y)
        //        return;


        //    _offset_Y = ScrollView.clipOffset.y;

        //    if (_offset_Y < -_lineSpaceY * _count + 1 && _count + ItemList.Count - 1 < HistoryList.Count)
        //    {
        //        int index = (_count - 1) % ItemList.Count;
        //        Transform tempTran = ItemList[index];
        //        Vector3 tempV3 = tempTran.localPosition;
        //        tempTran.localPosition = new Vector3(tempV3.x, tempV3.y - _lineSpaceY * ItemList.Count, tempV3.z);
        //        ++_count;
        //        ItemList[index].GetComponent<HistoryItem>().InitItem(HistoryList[_count + 3]);
        //    }
        //    else if (_offset_Y > -_lineSpaceY * _count && _count - 1 > 0)
        //    {

        //        int index = (_count - 2) % ItemList.Count;
        //        Transform tempTran = ItemList[index];
        //        Vector3 tempV3 = tempTran.localPosition;
        //        tempTran.localPosition = new Vector3(tempV3.x, tempV3.y + _lineSpaceY * ItemList.Count, tempV3.z);
        //        --_count;
        //        ItemList[index].GetComponent<HistoryItem>().InitItem(HistoryList[_count - 1]);
        //    }
        //}

        public void OnClickCloseHistoryViewBtn()
        {
            Containser.SetActive(false);
        }

        public void Reset()
        {
            foreach (var item in ItemList)
            {
                item.gameObject.SetActive(false);
            }
            HistoryList.Clear();
            _curHistoryItemInfo = new HistoryItemInfo();
        }
    }


    public class HistoryItemInfo
    {
        public List<HistoryUserInfo> Users;
        public int RoundVal;
        public string Time;
        //public long Time;
        public List<int> PublicPokers;
    }

    public class HistoryUserInfo
    {
        public YxBaseUserInfo UserInfo;
        public int Seat;
        public List<int> Pokers;
        /// <summary>
        /// 输赢分数
        /// </summary>
        public int Score;
        public bool IsFold;
        /// <summary>
        /// 玩家是否参与了游戏
        /// </summary>
        public int PokerType;
    }
}
