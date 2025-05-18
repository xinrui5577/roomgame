using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.sanpian.DataStore;
using Assets.Scripts.Game.sanpian.item;
using Assets.Scripts.Game.sanpian.Tool;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.sanpian.server
{
    public class MyPlayerCtrl : PlayerCtrl
    {
        void Start()
        {
            base.Start();
            ThisCardDiatance = CardDiatance / 1334 * Screen.width;//屏幕宽度适配
        }
        public List<int> MyCardsValueList;


        public int TiShiIndex = -1;

        //拖拽第一个下标
        private int headIndex = -1;

        public bool IsOut;

        public int MyFriendIndex;

        List<CardGroup> TiShiList = new List<CardGroup>();

        /// <summary>
        /// 连片标记
        /// </summary>
        protected bool IsLianPian
        {
            get { return App.GetGameData<SanPianGameData>().IsLianPian; }
        }

        public override void SetCardValue(int[] cards)
        {
            MyCardsValueList = new List<int>(cards);
            SortHandList(ref MyCardsValueList);
            StartCoroutine(CardValueForStart());
        }

        public override void Reset()
        {
            base.Reset();
            TiShiIndex = -1;
            IsOut = false;
            TiShiList = new List<CardGroup>();
        }

        #region 手牌排序
        public void SortHandList(ref List<int> list)
        {
            list.Sort();
            List<CardGroup> groupList = new List<CardGroup>();
            int samCount = 1;
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i] == list[i + 1])
                {
                    samCount++;
                }
                else
                {
                    samCount = 1;
                }
                if (samCount == 3)
                {
                    samCount = 1;
                    CardGroup group = new CardGroup();
                    if (list[i] == 81)
                    {
                        group.Gtype = 17;
                    }
                    else if (list[i] == 97)
                    {
                        group.Gtype = 18;
                    }
                    else
                    {
                        group.Gtype = list[i] % 16;
                    }
                    for (int j = 0; j <= 2; j++)
                    {
                        group.AddMember(list[i - 1]);
                        list.RemoveAt(i - 1);
                    }
                    group.GroupNum += 500;
                    groupList.Add(group);
                    i -= 2;
                }
            }
            while (list.Count > 0)
            {
                CardGroup group = new CardGroup();
                int temp = list[0];
                group.Gtype = temp % 16;
                if (temp == 81)
                {
                    group.Gtype = 17;
                }
                else if (temp == 97)
                {
                    group.Gtype = 18;
                }
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] % 16 == temp % 16)
                    {
                        group.AddMember(list[i]);
                        group.GroupNum += 10;
                        list.RemoveAt(i);
                        i--;
                    }
                }
                if (temp % 16 == 1)
                {
                    switch (group.Member.Count)
                    {
                        case 1:
                            groupList.Add(group);
                            break;
                        case 2:
                            if (group.Member[0] == 97)
                            {
                                group.Gtype = 18;
                                groupList.Add(group);
                            }
                            else if (group.Member[0] != group.Member[1])
                            {
                                CardGroup SingleGroup = new CardGroup();
                                SingleGroup.AddMember(group.Member[1]);
                                SingleGroup.Gtype = 18;
                                SingleGroup.GroupNum = 10;
                                groupList.Add(SingleGroup);
                                group.Member.RemoveAt(1);
                                group.GroupNum = 10;
                                group.Gtype = 17;
                                groupList.Add(group);
                            }
                            else
                            {
                                group.Gtype = 17;
                                groupList.Add(group);
                            }
                            break;
                        case 3:
                            CardGroup newGroup = new CardGroup();
                            if (group.Member[1] == 81)
                            {

                                newGroup.AddMember(group.Member[2]);
                                newGroup.GroupNum += 10;
                                newGroup.Gtype = 18;
                                group.Member.RemoveAt(2);
                                group.GroupNum = 20;
                                group.Gtype = 17;
                            }
                            else
                            {
                                newGroup.AddMember(group.Member[0]);
                                newGroup.GroupNum += 10;
                                newGroup.Gtype = 17;
                                group.Member.RemoveAt(0);
                                group.GroupNum = 20;
                                group.Gtype = 18;
                            }
                            groupList.Add(newGroup);
                            groupList.Add(group);
                            break;
                        case 4:
                            CardGroup TempGroup = new CardGroup();
                            TempGroup.AddMember(group.Member[2]);
                            TempGroup.AddMember(group.Member[3]);
                            TempGroup.Gtype = 18;
                            TempGroup.GroupNum = 20;
                            groupList.Add(TempGroup);
                            group.Member.Remove(97);
                            group.Member.Remove(97);
                            group.GroupNum = 20;
                            group.Gtype = 17;
                            groupList.Add(group);
                            break;
                    }
                }
                else
                {
                    groupList.Add(group);
                }

            }

            groupList.Sort((a, b) =>
            {
                if (a.GroupNum > b.GroupNum) return -1;
                if (a.GroupNum < b.GroupNum) return 1;
                if (a.GroupNum == b.GroupNum)
                {
                    if (a.Gtype > b.Gtype)
                    {
                        return -1;
                    }
                    return 1;
                }
                return 0;
            });
            foreach (CardGroup group in groupList)
            {
                List<int> MemberList = group.Member;
                MemberList.Sort((a, b) =>
                {
                    if (a > b) return -1;
                    if (a < b) return 1;
                    return 0;
                }
                    );

                for (int i = 0; i < MemberList.Count; i++)
                {
                    list.Add(MemberList[i]);
                }
            }
            if (list.Count > 27)
            {
                int moreLen = list.Count - 27;
                for (int i = 0; i < moreLen; i++)
                {
                    int temp = list[0];
                    list.Add(temp);
                    list.RemoveAt(0);
                }
            }
        }
        #endregion

        #region 提示牌
        public void TiShi()
        {
            DownAllCards();
            if (TiShiIndex < 0)
            {
                TiShiList = GetTiShiList();
                TiShiIndex = 0;
            }
            if (TiShiList.Count < 1)
            {
                //不出
                App.GetGameManager<SanPianGameManager>().UIButtonCtrl.ClickPass();
                return;
            }
            int len = TiShiList.Count;
            UpSomeCards(TiShiList[TiShiIndex].Member);
            TiShiIndex++;
            TiShiIndex = TiShiIndex >= len ? 0 : TiShiIndex;
        }

        void UpSomeCards(List<int> cards)
        {
            int len = cards.Count;
            int listLen = CardItemList.Count;
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < listLen; j++)
                {
                    if (cards[i] == CardItemList[j].Value && !CardItemList[j].IsStatus())
                    {
                        CardItemList[j].UpCard();
                        break;
                    }
                }
            }
        }

        void DownAllCards()
        {
            foreach (CardItem item in CardItemList)
            {
                item.DownCard();
            }
        }

        /// <summary>
        /// 连片
        /// </summary>
        /// <param name="list"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        List<CardGroup> GetLianPianList(List<CardGroup> list, int len)
        {
            List<CardGroup> GroupList = new List<CardGroup>();
            for (int i = 0; i < list.Count - len + 1; i++)
            {
                CardGroup group = new CardGroup();
                group.AddMemeberList(list[i].Member);
                int firstType = list[i].Gtype;
                group.Gtype = firstType;
                bool flag = true;
                int index = 1;
                for (int j = i + 1; j < i + len; j++)
                {
                    if (list[j].Gtype != list[i].Gtype + index)
                    {
                        flag = false;
                        break;
                    }
                    index++;
                    group.AddMemeberList(list[j].Member);
                    group.GroupNum += 500;
                }
                if (flag)
                {
                    GroupList.Add(group);
                }
            }
            return GroupList;
        }

        List<CardGroup> GetLianZhaList(List<CardGroup> list, int len, int minLen)
        {
            List<CardGroup> GroupList = new List<CardGroup>();
            for (int i = 0; i < list.Count - len + 1; i++)
            {
                CardGroup group = new CardGroup();
                for (int k = 0; k < minLen; k++)
                {
                    group.AddMember(list[i].Member[k]);
                }
                int firstType = list[i].Gtype;
                group.Gtype = firstType;
                bool flag = true;
                int index = 1;
                for (int j = i + 1; j < i + len; j++)
                {
                    if (list[j].Gtype != list[i].Gtype + index)
                    {
                        flag = false;
                        break;
                    }
                    index++;
                    for (int k = 0; k < minLen; k++)
                    {
                        group.AddMember(list[j].Member[k]);
                    }
                    group.GroupNum += list[j].GroupNum;
                }
                if (flag)
                {
                    GroupList.Add(group);
                }
            }
            return GroupList;
        }

        List<CardGroup> GetTiShiList()
        {
            int lastValue = App.GetGameManager<SanPianGameManager>().lastV;
            List<CardGroup> GroupList = TiShiCards(new List<int>(MyCardsValueList));
            int len = GroupList.Count;
            List<CardGroup> newGroupList = new List<CardGroup>();
            if (lastValue == 0)
            {
                newGroupList = GroupList;
            }
            else if (lastValue < 40000 && lastValue > 10000)
            {
                for (int i = 0; i < len; i++)
                {
                    var value = GroupList[i].GValue;
                    if (value >= 550000)
                    {
                        if (!IsLianPian)
                        {
                            continue;
                        }
                    }
                    if (value < 100000 && value > 40000)
                    {
                        continue;
                    }
                    if (!CompareCards(value, lastValue))
                    {
                        continue;
                    }
                    newGroupList.Add(GroupList[i]);
                }
            }
            //else if (lastValue < 300000 && lastValue > 20000)
            //{
            //    for (int i = 0; i < len; i++)
            //    {
            //        if (!CompareCards(GroupList[i].GValue, lastValue))
            //        {
            //            continue;
            //        }
            //        newGroupList.Add(GroupList[i]);
            //    }
            //}

            //判断顺子
            //else if (lastValue < 50000 && lastValue > 40000)
            //{
            //    int minCard = lastValue%10000/100%16;
            //    int cardLen = lastValue%100;
            //    List<int> ABCList = new List<int>(); //把炸以下的组全装 在这里面，来组顺子
            //    foreach (CardGroup m_group in GroupList)
            //    {
            //        if (m_group.GValue > 40000)
            //        {
            //            break;
            //        }
            //        foreach (int card in m_group.Member)
            //        {
            //            if (card%16 == 15 || card%16 == 1)
            //            {
            //                break;
            //            }
            //            ABCList.Add(card);
            //        }
            //    }
            //    ABCList.Sort((a, b) =>
            //    {
            //        if (a%16 > b%16) return 1;
            //        if (a%16 < b%16) return -1;
            //        return 0;
            //    });
            //    int ABCListLen = ABCList.Count;
            //    if (ABCListLen >= cardLen)
            //    {
            //        List<int> m_ABCListLen = new List<int>(); //准备一个list，把里面的对子变成单;
            //        m_ABCListLen.AddRange(ABCList);
            //        for (int i = 0; i < m_ABCListLen.Count - 1; i++)
            //        {
            //            if (m_ABCListLen[i]%16 == m_ABCListLen[i + 1]%16)
            //            {
            //                m_ABCListLen.RemoveAt(i);
            //                i--;
            //            }
            //        }
            //        int newLen = m_ABCListLen.Count;
            //        for (int i = 0; i < newLen; i++)
            //        {
            //            if (m_ABCListLen[i]%16 <= minCard)
            //            {
            //                continue;
            //            }
            //            if (newLen - i < cardLen)
            //            {
            //                break;
            //            }
            //            bool flag = true;
            //            CardGroup group = new CardGroup();
            //            for (int j = i; j < i + cardLen - 1; j++)
            //            {
            //                if (m_ABCListLen[j]%16 + 1 != m_ABCListLen[j + 1]%16)
            //                {
            //                    flag = false;
            //                    break;
            //                }
            //                group.AddMember(m_ABCListLen[j]);
            //                if (j == i + cardLen - 2)
            //                {
            //                    group.AddMember(m_ABCListLen[j + 1]);
            //                }
            //            }
            //            if (flag)
            //            {
            //                newGroupList.Add(group);
            //            }
            //        }
            //    }
            //}

            //    //骑顺结束
            //    foreach (CardGroup m_group in GroupList)
            //    {
            //        if (m_group.GValue > 30000 && m_group.GValue < 300000)
            //        {
            //            newGroupList.Add(m_group);
            //        }
            //    }
            //}
            //连对
            else if (lastValue > 40000 && lastValue < 70000)
            {
                int SameLen = lastValue / 10000 - 3;
                int minCard = lastValue % 10000 / 100 % 16;
                int cardLen = lastValue % 100;
                List<int> ABCList = new List<int>(); //把炸以下的对全装在这里面,只装一张牌
                foreach (CardGroup m_group in GroupList)
                {
                    if (m_group.GValue >= 550000)
                    {
                        if (!IsLianPian)
                        {
                            continue;
                        }
                    }
                    if (m_group.GValue < SameLen * 10000)
                    {
                        continue;
                    }
                    if (m_group.GValue > 40000)
                    {
                        break;
                    }
                    for (int i = 0; i < SameLen; i++)
                    {
                        int card = m_group.Member[i];
                        if (card % 16 == 15 || card % 16 == 1)
                        {
                            break;
                        }
                        ABCList.Add(card);
                    }

                }
                ABCList.Sort((a, b) =>
                {
                    if (a % 16 > b % 16) return 1;
                    if (a % 16 < b % 16) return -1;
                    return 0;
                });

                int newLen = ABCList.Count;
                for (int i = 0; i < newLen; i++)
                {
                    if (ABCList[i] % 16 <= minCard)
                    {
                        continue;
                    }
                    if (newLen - i < cardLen)
                    {
                        break;
                    }
                    bool flag = true;
                    CardGroup group = new CardGroup();
                    int AbcIndex = 0;
                    for (int j = i; j < i + cardLen-1; j++)
                    {
                        if (AbcIndex++ % SameLen == SameLen-1)
                        {
                            if (ABCList[j] % 16+1 != ABCList[j + 1] % 16)
                            {
                                flag = false;
                                break;
                            }

                        }
                        else
                        {
                            if (ABCList[j] % 16 != ABCList[j + 1] % 16)
                            {
                                flag = false;
                                break;
                            }
                        }

                        //for (int k = 0; k < SameLen; k++)
                        //{
                        //    group.AddMember(ABCList[j + k]);
                        //}
                        //if (j == i + cardLen - 2)
                        //{
                        //    for (int k = 0; k < SameLen; k++)
                        //    {
                        //        group.AddMember(ABCList[j + k]);
                        //    }
                        //}
                    }
                    if (flag)
                    {
                        for (int j = i; j < cardLen+i; j++)
                        {
                            group.AddMember(ABCList[j]);
                        }
                        group.GValue = GroupValue(group.Member.ToArray());
                        newGroupList.Add(group);
                    }


                }
                foreach (CardGroup m_group in GroupList)
                {
                    if (m_group.GValue > 100000)
                    {
                        newGroupList.Add(m_group);
                    }
                }
            }
             //三带二
            else if (lastValue > 70000 && lastValue < 80000)
            {
                List<CardGroup> twoList = new List<CardGroup>();
                List<CardGroup> threeList = new List<CardGroup>();
                foreach (CardGroup m_group in GroupList)
                {
                    if (m_group.GValue >= 550000)
                    {
                        if (!IsLianPian)
                        {
                            continue;
                        }
                    }
                    if (m_group.GValue > 20000 && m_group.GValue < 30000)
                    {
                        twoList.Add(m_group);
                    }
                    else if (m_group.GValue > 30000 && m_group.GValue < 40000&&m_group.GValue%10000/100>lastValue%10000/100)
                    {
                        threeList.Add(m_group);
                    }
                }
                foreach (var three_group in threeList)
                {
                    foreach (var two_group in twoList)
                    {
                        CardGroup tempGroup=new CardGroup();
                        for (int i = 0; i < 3; i++)
                        {
                            tempGroup.AddMember(three_group.Member[i]);
                        }
                        for (int i = 0; i < 2; i++)
                        {
                            tempGroup.AddMember(two_group.Member[i]);
                        }
                        tempGroup.GValue = GroupValue(tempGroup.Member.ToArray());
                        if (tempGroup.GValue > 70000 && tempGroup.GValue<80000)
                        {
                            newGroupList.Add(tempGroup);
                        }
                    }
                }
                foreach (var cardGroup in GroupList)
                {
                    if (cardGroup.GValue >= 550000)
                    {
                        if (!IsLianPian)
                        {
                            continue;
                        }
                    }
                    if (cardGroup.GValue>100000)
                    {
                        newGroupList.Add(cardGroup);
                    }
                }
                if (newGroupList.Count<1&&threeList.Count>1)
                {
                    for (int i = 0; i < threeList.Count; i++)
                    {
                        for (int j = 0; j < threeList.Count; j++)
                        {
                            if (i==j)
                            {
                                continue;
                            }
                            if (threeList[i].GValue % 10000 / 100 > lastValue % 10000 / 100)
                            {
                                CardGroup tempGroup = new CardGroup();
                                for (int k = 0; k < 3; k++)
                                {
                                    tempGroup.AddMember(threeList[i].Member[k]);
                                }
                                for (int k = 0; k < 2; k++)
                                {
                                    tempGroup.AddMember(threeList[j].Member[k]);
                                }
                                tempGroup.GValue = GroupValue(tempGroup.Member.ToArray());
                                if (tempGroup.GValue > 70000 && tempGroup.GValue < 80000)
                                {
                                    newGroupList.Add(tempGroup);
                                }
                            }
                        }
                    }
                }
            }
            //蝴蝶
            else if (lastValue > 80000 && lastValue < 90000)
            {
                //List<CardGroup> twoList = new List<CardGroup>();
                //List<CardGroup> threeList = new List<CardGroup>();
                //foreach (CardGroup m_group in GroupList)
                //{
                //    if (m_group.GValue > 20000 && m_group.GValue < 30000)
                //    {
                //        twoList.Add(m_group);
                //    }
                //    else if (m_group.GValue > 30000 && m_group.GValue < 40000 && m_group.GValue % 10000 / 100 > lastValue % 10000 / 100)
                //    {
                //        threeList.Add(m_group);
                //    }
                //}
                foreach (var cardGroup in GroupList)
                {
                    if (cardGroup.GValue >= 550000)
                    {
                        if (!IsLianPian)
                        {
                            continue;
                        }
                    }
                    if (cardGroup.GValue > 100000)
                    {
                        newGroupList.Add(cardGroup);
                    }
                }
                if (newGroupList.Count<1)
                {
                    CardGroup tempGroup = new CardGroup();
                    newGroupList.Add(tempGroup); 
                }
            }
            else if (lastValue>100000)
            {
                foreach (var cardGroup in GroupList)
                {
                    if (cardGroup.GValue >= 550000)
                    {
                        if (!IsLianPian)
                        {
                            continue;
                        }
                    }
                    if (cardGroup.GValue >lastValue)
                    {
                        newGroupList.Add(cardGroup);
                    }
                }
            }
            return newGroupList;
        }

        /// <summary>
        /// 获取三片牌组
        /// </summary>
        /// <param name="list"></param>
        /// <param name="groupList"></param>
        private void GetGroup(List<int> list,List<CardGroup> groupList)
        {
            int samCount = 1;
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i] == list[i + 1])
                {
                    samCount++;
                }
                else
                {
                    samCount = 1;
                }
                if (samCount == 3)
                {
                    samCount = 1;
                    CardGroup group = new CardGroup();
                    if (list[i] == 81)
                    {
                        group.Gtype = 17;
                    }
                    else if (list[i] == 97)
                    {
                        group.Gtype = 18;
                    }
                    else
                    {
                        group.Gtype = list[i] % 16;
                    }
                    for (int j = 0; j <= 2; j++)
                    {
                        group.AddMember(list[i - 1]);
                        list.RemoveAt(i - 1);
                    }
                    group.GroupNum += 500;
                    groupList.Add(group);
                    i -= 2;
                }
            }
        }

        /// <summary>
        /// 处理三片牌组
        /// </summary>
        /// <param name="groupList"></param>
        private void DealPianCards(List<CardGroup> groupList)
        {
            if (groupList.Count > 1)
            {
                groupList.Sort((a, b) => a.Gtype - b.Gtype);
                int thisLen = groupList.Count;
                List<CardGroup> CopyList = groupList.ToList();
                int groupSam = 0;
                int firstIndex = 0;
                for (int i = 0; i < thisLen - 1; i++)
                {
                    if (groupList[i].Gtype == groupList[i + 1].Gtype)
                    {
                        groupSam++;
                    }
                    else
                    {
                        if (groupSam > 0)
                        {
                            for (int j = 0; j < groupSam; j++)
                            {
                                CardGroup tempGroup = new CardGroup();
                                for (int k = firstIndex; k < firstIndex + 2 + j; k++)
                                {
                                    tempGroup.AddMemeberList(groupList[k].Member);
                                    tempGroup.Gtype += 500;
                                }
                                tempGroup.Gtype = groupList[i].Gtype;
                                groupList.Add(tempGroup);
                            }
                        }
                        groupSam = 0;
                        firstIndex = i + 1;
                    }
                    if (i == thisLen - 2 && groupSam > 0)
                    {
                        for (int j = 0; j < groupSam; j++)
                        {
                            CardGroup tempGroup = new CardGroup();
                            for (int k = firstIndex; k < firstIndex + 2 + j; k++)
                            {
                                tempGroup.AddMemeberList(groupList[k].Member);
                                tempGroup.Gtype += 500;
                            }
                            tempGroup.Gtype = groupList[i].Gtype;
                            groupList.Add(tempGroup);
                        }
                    }
                }

                for (int i = 2; i < CopyList.Count + 1; i++)
                {
                    List<CardGroup> NewList = GetLianPianList(CopyList, i);
                    if (NewList.Count > 0)
                    {
                        groupList.AddRange(NewList);
                    }
                }

            }
        }

        List<CardGroup> TiShiCards(List<int> list)
        {
            List<CardGroup> groupList = new List<CardGroup>();

            GetGroup(list, groupList);
            DealPianCards(groupList);
            while (list.Count > 0)
            {
                CardGroup group = new CardGroup();
                int temp = list[0];
                for (int i = 0; i < list.Count; i++)
                {
                    switch (list[i])
                    {
                        case 81:
                            if (temp==list[i])
                            {
                                group.Gtype = 17;
                                group.AddMember(list[i]);
                                group.GroupNum += 10;
                                list.RemoveAt(i);
                                i--;
                            }
                            break;
                        case 97:
                            if (temp == list[i])
                            {
                                group.Gtype = 18;
                                group.AddMember(list[i]);
                                group.GroupNum += 10;
                                list.RemoveAt(i);
                                i--;
                            }
                            break;
                        default:
                            if (temp % 16 == list[i] % 16)
                            {
                                group.Gtype = temp % 16;
                                group.AddMember(list[i]);
                                group.GroupNum += 10;
                                list.RemoveAt(i);
                                i--;
                            }
                            break;
                    }
                }
                groupList.Add(group);
            }
            SortList(groupList);
            groupList =SortNoticeList(groupList);
            return groupList;
        }

        /// <summary>
        /// list 排序
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        private List<CardGroup> SortList(List<CardGroup> groups)
        {
            var list=new List<CardGroup>();
            foreach (CardGroup group in groups)
            {
                group.GValue = GroupValue(group.Member.ToArray());
            }
            groups.Sort((a, b) =>
            {
                if (a.GValue > b.GValue) return 1;
                if (a.GValue < b.GValue) return -1;
                if (a.GValue == b.GValue)
                {
                    if (a.Gtype > b.Gtype)
                    {
                        return -1;
                    }
                    return 1;
                }
                return 0;
            });
            list = groups.ToList();
            return list;
        }

        /// <summary>
        /// 提示列表排序
        /// </summary>
        /// <param name="groupList"></param>
        private List<CardGroup> SortNoticeList(List<CardGroup> groupList)
        {
            var lastValue = App.GetGameManager<SanPianGameManager>().lastV;
            List<CardGroup> TempList = new List<CardGroup>();
            if (lastValue > 10000 && lastValue < 20000)
            {
                var listNum2=new List<CardGroup>();
                var listNum3 = new List<CardGroup>();
                for (int i = 0; i < groupList.Count; i++)
                {
                    if (groupList[i].GroupNum == 20)
                    {
                        int card = groupList[i].Member[0];
                        CardGroup singleGroup = new CardGroup();
                        singleGroup.AddMember(card);
                        singleGroup.Gtype = groupList[i].Gtype;
                        singleGroup.GroupNum = 10;
                        listNum2.Add(singleGroup);
                    }
                    else
                    {
                        if (groupList[i].GroupNum == 30&& groupList[i].GValue<100000)
                        {
                            int card = groupList[i].Member[0];
                            CardGroup singleGroup = new CardGroup();
                            singleGroup.AddMember(card);
                            singleGroup.Gtype = card % 16;
                            singleGroup.GroupNum = 10;
                            listNum3.Add(singleGroup);
                        }
                    }
                }
                SortList(listNum2);
                SortList(listNum3);
                TempList.AddRange(listNum2);
                TempList.AddRange(listNum3);
            }
            if (lastValue > 20000 && lastValue < 30000)
            {
                for (int i = 0; i < groupList.Count; i++)
                {
                    if (groupList[i].GroupNum == 30 && groupList[i].GValue < 100000)
                    {
                        int card = groupList[i].Member[0];
                        CardGroup singleGroup = new CardGroup();
                        singleGroup.AddMember(groupList[i].Member[0]);
                        singleGroup.AddMember(groupList[i].Member[1]);
                        singleGroup.Gtype = card % 16;
                        singleGroup.GroupNum = 20;
                        TempList.Add(singleGroup);
                    }
                }
                SortList(TempList);
            }
            var mulList=new List<CardGroup>();
            for (int i = 40; i <= 80; i += 10)
            {
                var cacheList=new List<CardGroup>();
                for (int j = 0; j < groupList.Count; j++)
                {
                    if (groupList[j].GroupNum >= i && groupList[j].GroupNum < 500 && groupList[j].Gtype != 15)
                    {
                        cacheList.Add(groupList[j]);
                    }
                }
                if (cacheList.Count > 1)
                {
                    cacheList.Sort((a, b) =>
                    {
                        return a.Gtype - b.Gtype;
                    });
                    for (int k = 2; k < cacheList.Count + 1; k++)
                    {
                        List<CardGroup> NewList = GetLianZhaList(cacheList, k, i / 10);
                        if (NewList.Count > 0)
                        {
                            mulList.AddRange(NewList);
                        }
                    }
                }
            }
            groupList.AddRange(mulList);
            SortList(groupList);
            var list=AddTempList(groupList, TempList, lastValue);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].GValue < 10000)
                {
                    list.RemoveAt(i);
                    i--;
                }
                else
                {
                    break;
                }
            }
            return list.ToList();
        }

        /// <summary>
        /// 添加临时列表
        /// </summary>
        /// <param name="cardGroups"></param>
        /// <param name="tempGroups"></param>
        /// <param name="lastValue"></param>
        private List<CardGroup> AddTempList(List<CardGroup> cardGroups,List<CardGroup> tempGroups,int lastValue)
        {
            if (lastValue > 10000 && lastValue < 20000)
            {
                cardGroups=AddTempToList(cardGroups,tempGroups,10);
            }
            else if (lastValue > 20000 && lastValue < 30000)
            {
                cardGroups=AddTempToList(cardGroups, tempGroups, 20);
            }
            return cardGroups.ToList();
        }

        /// <summary>
        /// 添加指定组数列表
        /// </summary>
        /// <param name="cardGroups"></param>
        /// <param name="tempGroups"></param>
        /// <param name="groupNum"></param>
        private List<CardGroup> AddTempToList(List<CardGroup> cardGroups, List<CardGroup> tempGroups, int groupNum)
        {
            var index = cardGroups.FindIndex(item => item.GroupNum > groupNum);
            if (index <= -1)
            {
                cardGroups.AddRange(tempGroups);
            }
            else
            {
                var firstList = new List<CardGroup>();
                var endList = new List<CardGroup>();
                if (index == 0)
                {
                    endList = cardGroups.ToList();
                }
                else
                {
                    firstList = cardGroups.GetRange(0, index).ToList();
                    endList = cardGroups.GetRange(index, cardGroups.Count - index).ToList();
                }
                firstList.AddRange(tempGroups);
                firstList.AddRange(endList);
                cardGroups = firstList.ToList();
            }
            return cardGroups.ToList();
        }

        #endregion

        #region 出牌
        public override void OutCards(int[] cards)
        {
            //if (cards.Length > 0 && cards[0] != 0)
            //{
            //    AutoCards(cards);
            //    return;
            //}

            //for (int i = 0; i < CardItemList.Count; i++)
            //{
            //    if (CardItemList[i].IsStatus())
            //    {
            //        OutCardItemList.Add(CardItemList[i]);
            //        CardItemList[i].transform.SetParent(UIInfo.OutCardsArea);
            //        CardItemList[i].transform.localScale = Vector3.one * 0.5f;
            //        MyCardsValueList.RemoveAt(i);
            //        CardItemList.RemoveAt(i);
            //        i--;
            //    }
            //}
            //UIInfo.OutCardsArea.GetComponent<UIGrid>().Reposition();
            //if (MyCardsValueList.Count != CardItemList.Count)
            //{
            //    YxDebug.LogError("手牌与手牌值长度不相等，手牌数为：" + CardItemList.Count + "，值的数量为：" + MyCardsValueList.Count + "手牌值为：" + StringTool.CardsToString(MyCardsValueList));
            //}
            //SortHandList(ref MyCardsValueList);
            //UpdateCardItemValue();

            //TiShiIndex = -1;
            //AmIOut();
            AutoCards(cards);
        }

        public void ClickOutCards()
        {
            var gmanager = App.GetGameManager<SanPianGameManager>();
            int lastValue = gmanager.lastV;
            List<int> TempList = new List<int>();
            for (int i = 0; i < CardItemList.Count; i++)
            {
                if (CardItemList[i].IsStatus())
                {
                    TempList.Add(CardItemList[i].Value);
                }
            }
            int[] cardsToCheck = TempList.ToArray();
            int thisValue = GroupValue(cardsToCheck);
            //CardsMusicPlay(TempList.ToArray());
            //return;
            if (thisValue < 0)
            {
                YxDebug.Log("打出的牌：" + StringTool.CardsToString(TempList) + "，無法打出");
                DownAllCards();
                return;
            }
            if (!CompareCards(thisValue, lastValue))
            {
                YxDebug.Log("打出的牌：" + StringTool.CardsToString(TempList) + "，目标牌：" + StringTool.CardsToString(gmanager.lastCards) + "，判定为管不上");
                DownAllCards();
                return;
            }
            ISFSObject param = new SFSObject();
            param.PutInt("realSeat", userInfo.Seat);
            param.PutIntArray("cards", TempList.ToArray());
            param.PutInt("cardsv", thisValue);
            param.PutInt(RequestKey.KeyType, (int)LandRequestData.GameRequestType.TypeChuPai);
            YxDebug.Log("发送出牌,牌值为:" + StringTool.CardsToString(TempList));
            App.GetRServer<SanPianGameServer>().SendGameRequest(param);
        }

        private void AutoCards(int[] cards)
        {
            List<int> OutList = new List<int>(cards);
            SortCardsTool.SortCards(OutList);
            for (int j = 0; j < OutList.Count; j++)
            {
                for (int i = 0; i < CardItemList.Count; i++)
                {
                    if (CardItemList[i].Value == OutList[j])
                    {
                        OutCardItemList.Add(CardItemList[i]);
                        CardItemList[i].transform.SetParent(UIInfo.OutCardsArea);
                        CardItemList[i].MyCardFlag = false;
                        CardItemList[i].transform.localScale = Vector3.one * 0.5f;
                        CardItemList[i].SetCardDepth(j);
                        MyCardsValueList.RemoveAt(i);
                        CardItemList.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }
            UIInfo.OutCardsArea.GetComponent<UIGrid>().Reposition();
            if (MyCardsValueList.Count != CardItemList.Count)
            {
                YxDebug.LogError("手牌与手牌值长度不相等，手牌数为：" + CardItemList.Count + "，值的数量为：" + MyCardsValueList.Count + "手牌值为：" + StringTool.CardsToString(MyCardsValueList));
            }
            SortHandList(ref MyCardsValueList);
            UpdateCardItemValue();
            AmIOut();
            TiShiIndex = -1;
        }

        void AmIOut()
        {
            if (MyCardsValueList.Count == 0)
            {
                IsOut = true;
            }
        }

        #endregion

        #region 比大小
        bool CompareCards(int myCards, int target)
        {
            if (target == 0)
            {
                return true;
            }
            if (target < 40000 && target > 10000)
            {
                if (myCards > target / 10000 * 10000 && myCards < (target / 10000 + 1) * 10000)
                {
                    return myCards > target;
                }
                if (myCards > 100000)
                {
                    return true;
                }
            }
            else if (target > 40000 && target < 100000)
            {
                if ((myCards > target / 10000 * 10000 && myCards < (target / 10000 + 1) * 10000) && myCards % 100 == target % 100)
                {
                    return myCards > target;
                }
                if (myCards > 100000)
                {
                    return true;
                }
            }
            else if (target > 100000)
            {
                return myCards > target;
            }
            
            return false;
        }


        int GroupValue(int[] cards)
        {
            int len = cards.Length;
            if (len<1)
            {
                return -1;
            }
            Array.Sort(cards);
            //先查三片
            if (len == 3)
            {
                if (cards[0] == cards[1] && cards[1] == cards[2])
                {
                    int cardv = cards[0] == 81 || cards[0] == 97 ? cards[0] : cards[0] % 16;
                    return 510000 + cardv * 100 + len;
                }
            }
            if (len % 3 == 0)
            {
                bool pian = true;
                for (int i = 0; i < len - 1; i++)
                {
                    if (i % 3 == 2)
                    {
                        continue;
                    }
                    if (cards[i] != cards[i + 1])
                    {
                        pian = false;
                        break;
                    }
                }
                //去花
                for (int i = 0; i < len; i++)
                {
                    if (cards[i] == 81)
                    {
                        cards[i] = 17;
                    }
                    else if (cards[i] == 97)
                    {
                        cards[i] = 18;
                    }
                    else
                    {
                        cards[i] = cards[i] % 16;
                    }
                }
                Array.Sort(cards);
                if (pian)
                {
                    bool hengpian = false;
                    bool lianpian = false;
                    for (int i = 2; i < len - 1; i += 3)
                    {
                        if (hengpian)
                        {
                            if (cards[i] != cards[i + 1])
                            {
                                hengpian = false;
                                break;
                            }
                        }
                        else if (lianpian)
                        {
                            if (cards[i] + 1 != cards[i + 1])
                            {
                                lianpian = false;
                                break;
                            }
                        }
                        else
                        {
                            if (cards[i] == cards[i + 1])
                            {
                                hengpian = true;
                            }
                            else if (cards[i] + 1 == cards[i + 1])
                            {
                                lianpian = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (cards[0] == 17 && lianpian)
                    {
                        return len * 100000 + cards[0] * 100 + len;
                    }
                    if (lianpian)
                    {
                        return len * 100000 - 50000 + cards[0] * 100 + len;
                    }
                    if (hengpian)
                    {
                        return len * 100000 + cards[0] * 100 + len;
                    }
                }
            }
            //去花
            for (int i = 0; i < len; i++)
            {
                if (cards[i] == 81)
                {
                    cards[i] = 17;
                }
                else if (cards[i] == 97)
                {
                    cards[i] = 18;
                }
                else
                {
                    cards[i] = cards[i] % 16;
                }
            }
            Array.Sort(cards);


            bool flag = true;

            //单-------------------------------------

            if (len == 1)
            {
                return 10000 + cards[0] * 100 + len;
            }

            //对--------------------------------------

            if (len == 2 && cards[0] == cards[1])
            {
                return 20000 + cards[0] * 100 + len;
            }

            //三个 3万------------------------------------

            if (len == 3 && cards[0] == cards[1] && cards[1] == cards[2])
            {
                return len * 10000 + cards[0] * 100 + len;
            }
            //炸
            if (len > 3 && len < 12)
            {
                flag = true;

                for (int i = 0; i < len - 1; i++)
                {
                    if (cards[i] != cards[i + 1] || cards[0] == 1)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    return (len + 10) * 10000 + cards[0] * 100 + len;
                }
            }

            //连炸
            if (len > 2)
            {
                flag = true;
                List<List<int>> list = new List<List<int>>();
                int firstCard = cards[0];
                List<int> temp = new List<int>();
                temp.Add(firstCard);
                for (int i = 1; i < len; i++)
                {
                    if (cards[i] > 14)
                    {
                        flag = false;
                        break;
                    }
                    if (cards[i] != firstCard)
                    {
                        firstCard = cards[i];
                        list.Add(temp);
                        temp = new List<int>();
                        temp.Add(cards[i]);
                    }
                    else
                    {
                        temp.Add(cards[i]);
                    }
                }
                list.Add(temp);
                if (flag)
                {
                    for (int i = 0; i < list.Count - 1; i++)
                    {
                        if (list[i].Count != list[i + 1].Count || list[i][0] + 1 != list[i + 1][0])
                        {
                            flag = false;
                            break;
                        }
                    }
                }
                
                if (flag)
                {
                    switch (list[0].Count)
                    {
                        case 1:
                            if (len > 4)
                            {
                                return 40000 + cards[0] * 100 + len;
                            }
                            break;
                        case 2:
                            return 50000 + cards[0] * 100 + len;
                        case 3:
                            return 60000 + cards[0] * 100 + len;
                        default:
                            return IsLianPian ? (len + 10)*10000 + (10 - list.Count)*100 + cards[0] : -1;
                    }
                }
            }
            var sanNum = 0;
            var sanCount=CheckSanDaiEr(cards, out sanNum);
            if(sanCount!=0)
            {
                if (sanCount==1)
                {
                    return 70000 + sanNum * 100 + len;
                }
                else
                {
                    return 80000 +sanNum * 100 + len;
                }
            }
            return -1;
        }
        #endregion

        #region 更新牌值
        IEnumerator CardValueForStart()
        {
            if (MyCardsValueList.Count != CardItemList.Count)
            {
                YxDebug.LogError("手牌与手牌值长度不相等，手牌数为：" + CardItemList.Count + "，值的数量为：" + MyCardsValueList.Count + "手牌值为：" + StringTool.CardsToString(MyCardsValueList));
            }
            int DepthShang = 0;
            int SanPianCount = 0;
            int ColorIndex = 0;
            for (int i = 0; i < MyCardsValueList.Count; i++)
            {
                DepthShang = i > 26 ? 0 : 100;
                CardItemList[i].Value = MyCardsValueList[i];
                CardItemList[i].SetCardDepth(i + DepthShang);
                CardItemList[i].SetCardColor(Color.white);
                if (SanPianCount > 0)
                {
                    SanPianCount--;
                    CardItemList[i].SetCardColor(SanPianColor[ColorIndex / 3 % SanPianColor.Length]);
                    ColorIndex++;
                }
                else if (MyCardsValueList.Count > i + 2 && MyCardsValueList[i] == MyCardsValueList[i + 1] && MyCardsValueList[i + 1] == MyCardsValueList[i + 2])
                {
                    CardItemList[i].SetCardColor(SanPianColor[ColorIndex / 3 % SanPianColor.Length]);
                    ColorIndex++;
                    SanPianCount = 2;
                }
                yield return new WaitForSeconds(0.005f);
            }
            grid.Reposition();
            SetMyCardsPos();
        }

        void UpdateCardItemValue()
        {
            if (MyCardsValueList.Count != CardItemList.Count)
            {
                YxDebug.LogError("手牌与手牌值长度不相等，手牌数为：" + CardItemList.Count + "，值的数量为：" + MyCardsValueList.Count + "手牌值为：" + StringTool.CardsToString(MyCardsValueList));
            }
            int DepthShang = 0;
            int SanPianCount = 0;
            int ColorIndex = 0;
            for (int i = 0; i < MyCardsValueList.Count; i++)
            {
                DepthShang = i > 26 ? 0 : 100;
                CardItemList[i].Value = MyCardsValueList[i];
                CardItemList[i].SetCardColor(Color.white);
                if (SanPianCount > 0)
                {
                    SanPianCount--;
                    CardItemList[i].SetCardColor(SanPianColor[ColorIndex / 3 % SanPianColor.Length]);
                    ColorIndex++;
                }
                else if (MyCardsValueList.Count > i + 2 && MyCardsValueList[i] == MyCardsValueList[i + 1] && MyCardsValueList[i + 1] == MyCardsValueList[i + 2])
                {
                    CardItemList[i].SetCardColor(SanPianColor[ColorIndex / 3 % SanPianColor.Length]);
                    ColorIndex++;
                    SanPianCount = 2;
                }
                CardItemList[i].SetCardDepth(i + DepthShang);
            }
            grid.Reposition();
            SetMyCardsPos();
        }
        #endregion

        #region 划动选牌

        private float _dregDistance = 0;
        public void ChooseHead()
        {
            int len = CardItemList.Count;
            for (int i = 0; i < len; i++)
            {
                if (CardItemList[i].IsHead)
                {
                    headIndex = i;
                    CardItemList[i].IsHead = false;
                    CardItemList[i].ShowShade();
                    _dregDistance = 0;
                    break;
                }
            }
        }
        public float CardDiatance = 46;
        private float cardWidth = 140;
        private float ThisCardDiatance = 0;
        public void DragCards(float x)
        {

            _dregDistance += x;
            int temp = (int)(_dregDistance / ThisCardDiatance);
            int endIndex = 0;
            HideAllShade(headIndex);
            if (temp != 0)
            {
                endIndex = headIndex + temp;
                endIndex = endIndex < 0 ? 0 : endIndex > CardItemList.Count - 1 ? CardItemList.Count - 1 : endIndex;
                if (endIndex < headIndex)
                {
                    for (int i = endIndex; i < headIndex; i++)
                    {
                        CardItemList[i].ShowShade();
                    }
                }
                else
                {
                    for (int i = headIndex + 1; i < 1 + endIndex; i++)
                    {
                        CardItemList[i].ShowShade();
                    }
                }
            }
            else
            {
                CardItemList[headIndex].ShowShade();
            }

        }

        public void HideAllShade(int index)
        {
            int len = CardItemList.Count;
            for (int i = 0; i < len; i++)
            {
                if (i == index)
                {
                    continue;
                }
                if (index == -1 && CardItemList[i].IsShade)
                {
                    if (CardItemList[i].IsStatus())
                    {
                        CardItemList[i].DownCard();
                    }
                    else
                    {
                        CardItemList[i].UpCard();
                    }
                }
                CardItemList[i].HideShade();
            }
        }
        #endregion

        #region 获取牌
        //按数组获取牌，Bool标识是自己手牌还是队友手牌

        public Color[] SanPianColor;
        public void GetCardsFromArr(int[] cards, bool IsMe)
        {
            if (!IsMe)
            {
                App.GetGameManager<SanPianGameManager>().UIButtonCtrl.FriendCardsIcon.SetActive(true);
                IsOut = true;
            }
            MyCardsValueList = new List<int>(cards);
            SortHandList(ref MyCardsValueList);

            CardItemList.Clear();
            int len = MyCardsValueList.Count;
            int DepthShang = 0;
            int SanPianCount = 0;
            int ColorIndex = 0;
            for (int i = 0; i < len; i++)
            {
                DepthShang = i > 26 ? 0 : 100;
                CardItem card = (CardItem)Instantiate(App.GetGameManager<SanPianGameManager>().cardItem);
                if (SanPianCount > 0)
                {
                    SanPianCount--;
                    card.SetCardColor(SanPianColor[ColorIndex / 3 % SanPianColor.Length]);
                    ColorIndex++;
                }
                else if (MyCardsValueList.Count > i + 2 && MyCardsValueList[i] == MyCardsValueList[i + 1] && MyCardsValueList[i + 1] == MyCardsValueList[i + 2])
                {
                    card.SetCardColor(SanPianColor[ColorIndex / 3 % SanPianColor.Length]);
                    ColorIndex++;
                    SanPianCount = 2;
                }
                card.transform.SetParent(UIInfo.CardsArea);
                card.SetCardDepth(i + DepthShang);
                card.transform.localScale = Vector3.one * 0.8f;
                CardItemList.Add(card);
                card.Value = MyCardsValueList[i];
                if (IsMe)
                {
                    card.MyCardFlag = true;
                }
                else
                {
                    card.ShowShade();
                }
                App.GetGameManager<SanPianGameManager>().XiaoHuiCardList.Add(card);
            }
            grid.Reposition();
            SetMyCardsPos();

        }

        public void SetMyCardsPos()
        {
            for (int i = 0; i < CardItemList.Count; i++)
            {
                CardItemList[i].SetPos();
            }
        }


        public void FriendOutCard(int[] cards)
        {
            int len = cards.Length;
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < MyCardsValueList.Count; j++)
                {
                    if (cards[i] == MyCardsValueList[j])
                    {
                        MyCardsValueList.RemoveAt(j);
                        Destroy(CardItemList[CardItemList.Count-1].gameObject);
                        CardItemList.RemoveAt(CardItemList.Count - 1);
                        break;
                    }
                }
            }
            SortHandList(ref MyCardsValueList);
            UpdateCardItemValue();
        }
        #endregion
    }
}
