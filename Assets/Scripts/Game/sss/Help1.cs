using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;

// ReSharper disable InconsistentNaming
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace Assets.Scripts.Game.sss
{
    public class Help1 : MonoBehaviour
    {

        //void OnEnable()
        //{
        //    // int[] array = new int[] { 0x11, 0x21, 0x31, 0x22, 0x32, 0x12, 0x33, 0x23, 0x13, 0x14, 0x34, 0x24, 0x25 };  //三分天下
        //    //int[] array = new int[] { 0x11, 0x21, 0x31, 0x32, 0x1d, 0x2d, 0x3d, 0x4d, 0x33, 0x14, 0x25, 0x16, 0x22 };
        //    int[] array = new int[] { 54, 74, 23, 73, 40, 76, 29, 57, 70, 20, 38, 42, 59, };  
        //    List<int> testList = new List<int>();
        //    foreach (int item in array)
        //    {
        //        testList.Add(item);
        //    }
        //    List<PlayerDuns> pd = getPlayerDuns(testList);
        //    Debug.Log("end");
        //}
        


        public TreeNode Root;

        public static int GetValue(int card)
        {
            if ((card & 0x0f) == 1)
            {
                return 14;
            }
            return card & 0x0f;
        }

        public static int GetColor(int card)
        {
            return card & 0xf0;
        }


        /// <summary>
        /// 按牌面牌值排列
        /// </summary>
        /// <param name="list">要排列的列表</param>
        /// <param name="up">是否是升序排列</param>
        public static void SortList(List<int> list,bool up = true)
        {
            int type = up ? 1 : -1;
            list.Sort((x, y) => type * (GetValue(x) - GetValue(y)));
        }


        public class TreeNode
        {
            public SssDun Dun;
            public TreeNode Parent;
            public LinkedList<TreeNode> Children;

            public TreeNode(TreeNode parent, SssDun dun)
            {
                Parent = parent;
                Dun = dun;
                Children = new LinkedList<TreeNode>();
            }

            public TreeNode AddChild(SssDun dun)
            {
                if (Dun.Compare(dun) >= 0)
                {
                    TreeNode node = new TreeNode(this, dun);
                    Children.AddLast(node);
                    return node;
                }
                return null;
            }

            public List<SssDun> GetChildDuns()
            {
                List<SssDun> ret = new List<SssDun>();
                foreach (TreeNode node in Children)
                {
                    ret.Add(node.Dun);
                }
                return ret;
            }
        }

        public void GetTreeDuns(List<PlayerDuns> outList, TreeNode node)
        {
            if (node.Children.Count > 0)
            {
                foreach (TreeNode nodeC in node.Children)
                {
                    GetTreeDuns(outList, nodeC);
                }
            }
            else
            {
                PlayerDuns pDun = new PlayerDuns();

                pDun.AddDun(node.Dun);
                pDun.AddDun(node.Parent.Dun);
                pDun.AddDun(node.Parent.Parent.Dun);

                outList.Add(pDun);
            }
        }

        /// <summary>
        /// 检验五张牌的牌型
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public CardType CheckCardType(List<int> cards, bool sort)
        {
            //先排序
            if (sort)
                SortList(cards, false);

            bool isTongHua = true;
            bool isShunZi = true;
            int samecount = 1;
            List<int> sameNum = new List<int>();
            int color = GetColor(cards[0]);
            int value = GetValue(cards[0]);
            for (int i = 1; i < cards.Count; i++)
            {
                int card = cards[i];
                int cardColor = GetColor(card);
                int cardValue = GetValue(card);
                if (cardColor != color)
                {
                    isTongHua = false;
                }
                if (value - cardValue != 1)
                {
                    isShunZi = false;
                }

                if (cardValue == value)
                {
                    samecount++;
                }
                else
                {
                    if (samecount > 0)
                    {
                        sameNum.Add(samecount);
                        samecount = 1;
                    }
                }

                value = cardValue;
            }

            if (isTongHua && isShunZi)
            {
                return CardType.tonghuashun;
            }
            else if (isTongHua)
            {
                return CardType.tonghua;
            }
            else if (isShunZi)
            {
                return CardType.shunzi;
            }
            else if (sameNum.Count > 0)
            {
                if (sameNum.Count == 1)
                {
                    samecount = sameNum[0];
                    if (samecount == 2)
                    {
                        return CardType.yidui;
                    }
                    else if (samecount == 3)
                    {
                        return CardType.santiao;
                    }
                    else if (samecount == 4)
                    {
                        return CardType.tiezhi;
                    }
                }
                else if (sameNum.Count == 2)
                {
                    samecount = sameNum[0];
                    int samecount1 = sameNum[1];
                    if (samecount == 2 && samecount1 == 3)
                    {
                        return CardType.hulu;
                    }
                    else if (samecount == 2 && samecount1 == 2)
                    {
                        return CardType.liangdui;
                    }
                }
            }

            return CardType.sanpai;
        }


        public class VnItem
        {
            public int Val;
            public List<int> Cards;
            public int Count;

            public VnItem(int card)
            {
                Cards = new List<int> { card };
                Val = GetValue(card);
                Count = 1;
            }

            public void AddCard(int card)
            {
                if (GetValue(card) != Val)
                {
                    return;
                }

                if (!Cards.Contains(card))
                {
                    Cards.Add(card);
                    Count++;
                }
            }

            public int Compare(VnItem item)
            {
                int res = Count - item.Count;
                if (res != 0)
                {
                    return res;
                }
                return Val - item.Val;

            }
        }


        /// <summary>
        /// 将牌按照牌面值分类列表,牌总数不变
        /// </summary>
        public class VnList : List<VnItem>
        {

            public VnList() { }
            public VnList(List<int> cards)
            {
                foreach (int card in cards)
                {
                    PutCard(card);
                }

                SortByNum();
            }


            public VnItem FindItemByCard(int card)
            {
                foreach (VnItem item in this)
                {
                    if (item.Val == GetValue(card))
                    {
                        return item;
                    }
                }
                return null;
            }

            public void PutCard(int card)
            {
                VnItem find = FindItemByCard(card);
                if (find != null)
                {
                    find.AddCard(card);
                }
                else
                {
                    find = new VnItem(card);
                    Add(find);
                }
            }

            /// <summary>
            /// 将数组按同面值牌的个数降序排序
            /// </summary>
            public void SortByNum()
            {
                Sort((x, y) => -x.Compare(y));
            }



            public int Compare(VnList list1)
            {
                if (list1.Count != Count)
                    return 0;

                for (int i = 0; i < Count; i++)
                {
                    int comp = this[i].Compare(list1[i]);
                    if (comp != 0)
                        return comp;
                    if (this[i].Val != list1[i].Val)
                    {
                        //if (i > 0)
                        //{
                        //    if (this[0].count > 2)
                        //    {
                        //        return -this[i].val.CompareTo(list1[i].val);
                        //    }
                        //    else
                        //        return this[i].val.CompareTo(list1[i].val);
                        //}
                        return this[i].Val.CompareTo(list1[i].Val);
                    }
                }
                return 0;
            }


            public int CompareLine(VnList list1)
            {
                if (list1.Count != Count)
                    return 0;

                for (int i = 0; i < Count; i++)
                {
                    int comp = this[i].Compare(list1[i]);
                    if (comp != 0)
                        return comp;
                    else if (this[i].Val - list1[i].Val != 0)
                    {
                        int val1 = this[i].Val == 1 ? 14 : this[i].Val;     //如果是A,改为14
                        int val2 = list1[i].Val == 1 ? 14 : list1[i].Val;   //如果是A,改为14

                        return val1 - val2;
                    }
                }
                return 0;
            }

            public VnList GetEqual(int count, bool sort)
            {
                VnList list = new VnList();
                foreach (VnItem item in this)
                {
                    if (item.Count == count)
                    {
                        list.Add(item);
                    }
                }
                if (sort && list.Count > 0)
                {
                    list.SortByNum();
                }

                return list;
            }

            public VnList GetMoreThan(int count, bool sort)
            {
                VnList list = new VnList();
                foreach (VnItem item in this)
                {
                    if (item.Count >= count)
                        list.Add(item);
                }
                if (sort && list.Count > 1)
                {
                    list.Sort((o1, o2) => o2.Val - o1.Val);
                }
                return list;
            }

            public bool ContainsValue(int card)
            {
                foreach (VnItem item in this)
                {
                    if (item.Val == GetValue(card))
                    {
                        return true;
                    }
                }
                return false;
            }

            public int ContainsValues(List<int> cards)
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    if (ContainsValue(cards[i]))
                    {
                        return i;
                    }
                }
                return -1;
            }

            public List<int> GetValues()
            {
                List<int> cards = new List<int>();
                foreach (VnItem item in this)
                {
                    cards.Add(item.Val);
                }
                return cards;
            }
        }

        public class CnItem
        {
            public int Color;
            public int Count;
            public List<int> Cards;

            public CnItem(int card)
            {
                Cards = new List<int> { card };
                Count = 1;
                Color = GetColor(card);
            }

            public void AddCard(int card)
            {
                if (GetColor(card) != Color)
                {
                    return;
                }
                if (!Cards.Contains(card))
                {
                    Count++;
                    Cards.Add(card);
                }
            }

            /// <summary>
            /// 降序排列cards列表
            /// </summary>
            public void SortCards()
            {
                SortList(Cards, false);
            }

            public void ResetCount()
            {
                Count = Cards.Count;
            }
        }

        /// <summary>
        /// 花色
        /// </summary>
        public class CnList : List<CnItem>
        {

            public CnList() { }
            public CnList(List<int> cards)
            {
                foreach (int card in cards)
                {
                    PutCard(card);
                }
            }

            public CnItem FindItemByCard(int card)
            {
                foreach (CnItem item in this)
                {
                    if (item.Color == GetColor(card))
                    {
                        return item;
                    }
                }
                return null;
            }


            public void PutCard(int card)
            {
                CnItem item = FindItemByCard(card);
                if (item == null)
                {
                    item = new CnItem(card);
                    Add(item);
                }
                else
                {
                    item.AddCard(card);
                }
            }

            /// <summary>
            /// 获取个数多于count的值的CNList
            /// </summary>
            /// <param name="count">获取的个数</param>
            /// <param name="sort">是否排序</param>
            /// <param name="isResetCount"></param>
            /// <returns></returns>
            public CnList GetMoreThan(int count, bool sort, bool isResetCount)
            {
                CnList ret = new CnList();
                foreach (CnItem item in this)
                {
                    if (isResetCount)
                    {
                        item.ResetCount();
                    }

                    if (item.Count >= count)
                    {
                        //排序
                        if (sort)
                            item.SortCards();

                        ret.Add(item);
                    }
                }

                if (sort && ret.Count > 1)
                {
                    ret.Sort((i1, i2) =>
                    {
                        for (int i = 0; i < count; i++)
                        {
                            int val1 = GetValue(i1.Cards[i]);
                            int val2 = GetValue(i2.Cards[i]);
                            if (val1 - val2 != 0)
                                return -(val1 - val2);
                        }
                        return 0;
                    });
                }
                return ret;

            }

            public CnList GetEqualTo(int count, bool sort, bool isResetCount)
            {
                CnList ret = new CnList();
                foreach (CnItem item in this)
                {
                    if (isResetCount)
                    {
                        item.ResetCount();
                    }

                    if (item.Count == count)
                    {
                        //排序
                        if (sort)
                            item.SortCards();

                        ret.Add(item);
                    }
                }

                if (sort && ret.Count > 1)
                {
                    ret.Sort((i1, i2) =>
                    {
                        for (int i = 0; i < count; i++)
                        {
                            int val1 = GetValue(i1.Cards[i]);
                            int val2 = GetValue(i2.Cards[i]);
                            if (val1 - val2 != 0)
                                return -(val1 - val2);
                        }
                        return 0;
                    });
                }
                return ret;

            }
        }


        public class SssDun
        {
            public CardType CardType;
            public List<int> Cards;

            /// <summary>
            /// 比较自身两行的大小
            /// </summary>
            /// <param name="dun"></param>
            /// <returns></returns>
            public int Compare(SssDun dun)
            {
                if (CardType != dun.CardType)
                {
                    return CardType - dun.CardType;
                }
                else
                {
                    VnList list1 = new VnList(Cards);
                    VnList list2 = new VnList(dun.Cards);
                    return list1.Compare(list2);
                }
            }

            /// <summary>
            /// 与其他SssDun比较大小
            /// </summary>
            /// <param name="dun"></param>
            /// <returns></returns>
            public int CompareLine(SssDun dun)
            {
                if (CardType != dun.CardType)
                {
                    return CardType - dun.CardType;
                }
                else
                {
                    VnList list1 = new VnList(Cards);
                    VnList list2 = new VnList(dun.Cards);
                    return list1.CompareLine(list2);
                }
            }

            public SssDun()
            {
                Cards = null;
                CardType = CardType.none;
            }
        }

        public class PlayerDuns
        {
            public SssDun[] Duns = new SssDun[3];
            public CardType Special;
            public int Index = -1;

            public bool IsEffectDun(SssDun dun)
            {
                if (IsEmpty())
                {
                    return true;
                }
                else
                {
                    return Duns[Index].Compare(dun) <= 0;
                }
            }

            public bool AddDun(SssDun dun)
            {
                if (IsFullDun() || !IsEffectDun(dun))
                {
                    return false;
                }

                Duns[++Index] = dun;
                return true;
            }

            public bool IsFullDun()
            {
                return Index >= 2;
            }

            public bool IsEmpty()
            {
                return Index == -1;
            }

            public int CheckAllBig(PlayerDuns pdun)
            {
                if (!IsFullDun() || !pdun.IsFullDun())
                {
                    return 0;
                }
                for (int i = 0; i < Duns.Length; i++)
                {
                    var ret = Duns[i].CompareLine(pdun.Duns[i]);
                    if (ret > 0)
                    {
                        return 1;
                    }
                }

                return -1;
            }
        }

        public bool CheckShunZi(List<int> cards, List<SssDun> outList, int count, bool tonghua)
        {
            //cards.Sort((x, y) => -(GetValue(x) - GetValue(y)));
            SortList(cards, false);

            bool isShunzi = false;

            for (int i = 0; i < cards.Count - count + 1; i++)
            {
                int cardVi = GetValue(cards[i]);
                List<int> shunzi = new List<int> { cards[i] };
                for (int j = i + 1; j < cards.Count; j++)
                {
                    int cardVj = GetValue(cards[j]);
                    if (cardVj == cardVi)
                    {
                        continue;
                    }

                    if (cardVi - 1 == cardVj)
                    {
                        shunzi.Add(cards[j]);
                        cardVi = cardVj;
                    }
                    else
                    {
                        break;
                    }

                    //如果够数了
                    if (shunzi.Count >= count)
                    {
                        SssDun dun = new SssDun
                        {
                            CardType = tonghua ? CardType.tonghuashun : CardType.shunzi,
                            Cards = shunzi
                        };
                        outList.Add(dun);
                        isShunzi = true;
                        break;
                    }
                }
            }

            return isShunzi;
        }

        /// <summary>
        /// 查看数组中的牌是否在vnlist中
        /// </summary>
        /// <param name="vnList"></param>
        /// <param name="dunList"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        bool CheckContainCard(VnList vnList,List<SssDun> dunList,int index)
        {
            var list = dunList[index].Cards;
            int cIndex = vnList.ContainsValues(list);
            return cIndex != -1;
        }


        public bool CheckLianXu(VnList list, int card)
        {
            for (int i = card - 2; i <= card + 2; i++)
            {
                if (!list.ContainsValue(i))
                {
                    return false;
                }
            }

            return true;
        }

        //help
        public void SetTreeNode(List<int> cards, TreeNode treeNode, int deep)
        {

            if (deep > 3)
            {
                return;
            }

            int cardNum = deep == 3 ? 3 : 5;


            CnList cnList = new CnList(cards);
            VnList vnList = new VnList(cards);

            CnList cn5 = cnList.GetMoreThan(5, true, false);
            VnList vn2 = vnList.GetMoreThan(2, true);

            //如果没有关系的牌（有多个 或者 连续） 能组成任何牌型 后面的就不需要去确认别的牌
            bool isCheckType = true;
            //bool isTongHuaShun = false;
            //同花顺
            if (treeNode.Dun.CardType - CardType.tonghuashun >= 0 && deep <= 2)
            {
                if (cn5.Count > 0)
                {
                    foreach (CnItem item in cn5)
                    {
                        List<SssDun> shunzi = new List<SssDun>();
                        if (CheckShunZi(item.Cards, shunzi, 5, true) && !CheckContainCard(vn2, shunzi, shunzi.Count - 1))
                        {
                            isCheckType = false;
                        }
                        if (shunzi.Count > 0)
                        {
                            foreach (SssDun dun in shunzi)
                            {
                                AddDunToTree(cards, treeNode, dun, deep + 1);
                            }
                            break;
                        }
                    }
                }
            }

            //铁支
            if (isCheckType && treeNode.Dun.CardType - CardType.tiezhi >= 0 && deep <= 2)
            {
                VnList vn4 = vnList.GetEqual(4, true);
                if (vn4.Count > 0)
                {
                    foreach (VnItem item in vn4)
                    {
                        SssDun dun = new SssDun
                        {
                            CardType = CardType.tiezhi,
                            Cards = vn4[0].Cards
                        };
                        dun.Cards.Add(0);
                        AddDunToTree(cards, treeNode, dun, deep + 1);

                        if (!CheckLianXu(vnList, item.Cards[0]))
                        {
                            isCheckType = false;
                            break;
                        }
                    }
                }
            }

            VnList vn3 = vnList.GetMoreThan(3, true);

            //葫芦
            if (isCheckType && treeNode.Dun.CardType - CardType.hulu >= 0 && deep <= 2)
            {
                if (vn3.Count > 0 && vn2.Count > 0)
                {
                    foreach (VnItem item3 in vn3)
                    {
                        foreach (VnItem item2 in vn2)
                        {
                            if (item3.Val == item2.Val)
                            {
                                continue;
                            }
                            SssDun dun = new SssDun
                            {
                                CardType = CardType.hulu,
                                Cards = new List<int>(item3.Cards.GetRange(0, 3))
                            };
                            dun.Cards.AddRange(item2.Cards.GetRange(0, 2));

                            AddDunToTree(cards, treeNode, dun, deep + 1);
                        }
                    }
                }
            }

            //同花
            if (isCheckType && treeNode.Dun.CardType - CardType.tonghua >= 0 && deep <= 2)
            {
                if (cn5.Count > 0)
                {
                    //截取最前面的5个位最大的5个
                    //int loopcount
                    while (cn5.Count > 0)
                    {
                        List<int> subCards = cn5[0].Cards.GetRange(0, 5);
                        SssDun dun = new SssDun
                        {
                            CardType = CardType.tonghua,
                            Cards = new List<int>(subCards)
                        };

                        //如果当前牌型不是同花顺
                        if (CheckCardType(dun.Cards, false) != CardType.tonghuashun)
                        {
                            AddDunToTree(cards, treeNode, dun, deep + 1);
                            int index = vn2.ContainsValues(dun.Cards);
                            if (index != -1)
                            {
                                cn5[0].Cards.RemoveRange(0, 5);
                                cn5 = cn5.GetMoreThan(5, true, true);
                            }
                            else
                            {
                                isCheckType = false;
                                break;
                            }
                        }
                        else
                        {
                            //移除 牌中最小的那个牌 然后重新确认cn5
                            cn5[0].Cards.Remove(dun.Cards[dun.Cards.Count - 1]);
                            cn5 = cn5.GetMoreThan(5, true, true);
                        }
                    }
                }
            }

            //顺子
            if (isCheckType && treeNode.Dun.CardType - CardType.shunzi >= 0 && deep <= 2)
            {
                List<SssDun> shunzi = new List<SssDun>();

                if (CheckShunZi(cards, shunzi, 5, false) && !CheckContainCard(vn2, shunzi, shunzi.Count - 1))
                {
                    isCheckType = false;
                }

                if (shunzi.Count > 0)
                {
                    foreach (SssDun dun in shunzi)
                    {
                        if (CheckCardType(dun.Cards, false) != CardType.tonghuashun)
                        {
                            AddDunToTree(cards, treeNode, dun, deep + 1);
                        }
                    }
                }
            }

            //三条
            if (isCheckType && treeNode.Dun.CardType - CardType.shunzi >= 0)
            {
                if (vn3.Count > 0)
                {
                    SssDun dun = new SssDun
                    {
                        CardType = CardType.santiao,
                        Cards = new List<int>(vn3[0].Cards.GetRange(0, 3))
                    };
                    if (cardNum == 5)
                    {
                        dun.Cards.Add(0);
                        dun.Cards.Add(0);
                    }
                    AddDunToTree(cards, treeNode, dun, deep + 1);

                    isCheckType = false;
                }
            }

            //两对
            if (isCheckType && treeNode.Dun.CardType - CardType.liangdui >= 0 && deep <= 2)
            {
                if (vn2.Count > 1)
                {
                    SssDun dun = new SssDun
                    {
                        CardType = CardType.liangdui,
                        Cards = vn2[0].Cards.GetRange(0, 2)
                    };
                    dun.Cards.AddRange(vn2[1].Cards.GetRange(0, 2));
                    dun.Cards.Add(0);
                    AddDunToTree(cards, treeNode, dun, deep + 1);
                }
            }

            //对子
            if (isCheckType && treeNode.Dun.CardType - CardType.yidui >= 0)
            {
                if (vn2.Count > 0)
                {
                    SssDun dun = new SssDun
                    {
                        CardType = CardType.yidui,
                        Cards = vn2[0].Cards.GetRange(0, vn2[0].Cards.Count)
                    };
                    if (cardNum == 5)
                    {
                        dun.Cards.Add(0);
                        dun.Cards.Add(0);
                    }
                    dun.Cards.Add(0);
                    AddDunToTree(cards, treeNode, dun, deep + 1);

                    isCheckType = false;
                }
            }

            //散牌
            if (isCheckType)
            {
                cardNum = cardNum > cards.Count ? cards.Count : cardNum;
                SssDun dun = new SssDun
                {
                    CardType = CardType.sanpai,
                    Cards = new List<int>(cards.GetRange(0, cardNum))
                };

                AddDunToTree(cards, treeNode, dun, deep + 1);
            }
        }






        public void AddDunToTree(List<int> cards, TreeNode treeNode, SssDun dun, int deep)
        {
            TreeNode node = treeNode.AddChild(dun);
            if (node != null)
            {
                List<int> cardClone = cards.GetRange(0, cards.Count);
                //删除已使用过的牌
                foreach (int card in dun.Cards)
                {
                    if (card != 0)
                    {
                        cardClone.Remove(card);
                    }
                }

                //将剩余的牌继续检测
                SetTreeNode(cardClone, node, deep);
            }
            else
            {
                treeNode.Parent.Children.Remove(treeNode);
            }

        }


        /// <summary>
        /// 获取特殊牌型
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="dunsOut"></param>
        public void GetSpecialCardType(List<int> cards, List<PlayerDuns> dunsOut)
        {

            CardType type = CheckCardType(cards, true);
            VnList vnList = new VnList(cards);
            CnList cnList = new CnList(cards);
            List<int> cardValues = vnList.GetValues();

            SssGameData gData = App.GetGameData<SssGameData>();
                

            bool isJ_A = true;
            bool is8_k = true;
            bool is2_8 = true;
            foreach (int card in cardValues)
            {
                int val = GetValue(card);
                if (val < 11)
                {
                    isJ_A = false;
                }
                if (val < 8)
                {
                    is8_k = false;
                }
                if (val > 8)
                {
                    is2_8 = false;
                }
                if (!isJ_A && !is8_k && !is2_8)
                {
                    break;
                }
            }
            PlayerDuns pDun = new PlayerDuns
            {
                Special = CardType.none,
                Duns = dunsOut[0].Duns
            };
            if (type == CardType.tonghuashun 
                && gData.IsAllowSpecialType((int) CardType.tonghuashisanshui))
            {
                pDun.Special = CardType.tonghuashisanshui;
                dunsOut.Insert(0, pDun);
            }
            else if ((type == CardType.shunzi || type ==CardType.tonghuashun)
                && gData.IsAllowSpecialType((int)CardType.shisanshui))
            {
                pDun.Special = CardType.shisanshui;
                dunsOut.Insert(0, pDun);
            }
            else if (isJ_A
                && gData.IsAllowSpecialType((int)CardType.shierhuang))
            {
                pDun.Special = CardType.shierhuang;
                dunsOut.Insert(0, pDun);
            }
            else
            {
                PlayerDuns p = dunsOut[0];
                CardType frist = CheckCardType(p.Duns[0].Cards, true);
                if (p.Duns[1].CardType == CardType.tonghuashun && p.Duns[2].CardType == CardType.tonghuashun && frist == CardType.tonghuashun
                    && gData.IsAllowSpecialType((int)CardType.santonghuashun))
                {
                    pDun.Special = CardType.santonghuashun;
                    dunsOut.Insert(0, pDun);
                }
                else if (vnList.GetMoreThan(4, false).Count >= 3
                    && gData.IsAllowSpecialType((int)CardType.santonghuashun))
                {
                    pDun.Special = CardType.santonghuashun;
                    dunsOut.Insert(0, pDun);
                }
                else if (is8_k
                    && gData.IsAllowSpecialType((int)CardType.quanda))
                {
                    pDun.Special = CardType.quanda;
                    dunsOut.Insert(0, pDun);
                }
                else if (is2_8
                    && gData.IsAllowSpecialType((int)CardType.quanxiao))
                {
                    pDun.Special = CardType.quanxiao;
                    dunsOut.Insert(0, pDun);
                }
                else if (cnList.Count == 2 && Mathf.Abs(cnList[0].Color - cnList[1].Color) == 0x10
                    && gData.IsAllowSpecialType((int)CardType.couyise))
                {
                    pDun.Special = CardType.couyise;
                    dunsOut.Insert(0, pDun);
                }
                else if (vnList.GetMoreThan(3, false).Count >= 4
                    && gData.IsAllowSpecialType((int)CardType.sitiaosan))
                {
                    pDun.Special = CardType.sitiaosan;
                    dunsOut.Insert(0, pDun);
                }
                else if (vnList.GetMoreThan(2, false).Count >= 6 && vnList.GetMoreThan(3, false).Count >= 1
                    && gData.IsAllowSpecialType((int)CardType.wuduisan))
                {

                    pDun.Special = CardType.wuduisan;
                    dunsOut.Insert(0, pDun);
                }
                else if (vnList.GetMoreThan(2, false).Count >= 6
                    && gData.IsAllowSpecialType((int)CardType.liuduiban))
                {
                    pDun.Special = CardType.liuduiban;
                    dunsOut.Insert(0, pDun);
                }
                else if (gData.IsAllowSpecialType((int)CardType.santonghua)&&checkSanTongHua(pDun, cards))
                {
                    pDun.Special = CardType.santonghua;
                    dunsOut.Insert(0, pDun);
                    
                }
                else if (gData.IsAllowSpecialType((int)CardType.sanshunzi)&&checkSanShunZi(pDun,cards))
                {
                    pDun.Special = CardType.sanshunzi;
                    dunsOut.Insert(0, pDun);
                }
            }
        }

        protected bool checkSanShunZi(PlayerDuns pDun,List<int> allCards){
		//移除前
		int[][] checkCnt = {
		    new[] {3,5,5}, new[] {5,3,5}, new [] {5,5,3}
		};

		
		foreach(int[] check1 in checkCnt){
			List<int> check = new List<int>(allCards);
			List<List<int>> removeArr = new List<List<int>>();
			bool is3 = false;
			foreach(int cnt in check1){
				List<int> remove = new List<int>();
				if(!removeLianXu(check, cnt, remove)){
					break;
				}
				if(cnt==3){
					is3 = true;
					removeArr.Insert(0,remove);
				}else{
					if(is3){
						removeArr.Insert(1,remove);						
					}else{
						removeArr.Insert(0,remove);
					}
				}
			}
			if(check.Count==0){
				//成功重置 牌数据
				pDun.Duns[0].Cards.Clear();
				pDun.Duns[0].Cards.AddRange(removeArr[0]);
				pDun.Duns[1].Cards.Clear();
				pDun.Duns[1].Cards.AddRange(removeArr[1]); 
				pDun.Duns[2].Cards.Clear();
				pDun.Duns[2].Cards.AddRange(removeArr[2]); 
				return true;
			}
		}
		
		
		return false;
	}
	
	protected bool removeLianXu(List<int> check,int cnt,List<int> remove){
		if(cnt<2){
			return false;
		}
		
		List<int> delete = new List<int>();
		for(int i = 0;i<check.Count-1;){
			int iVal = GetValue(check[i]);
			int iJian1Val = GetValue(check[i+1]);
			if(delete.Count==0){
				delete.Add(check[i]);				
			}
			if(iVal-iJian1Val==1){
				if(delete.Count>=cnt){
					break;
				}else{					
					delete.Add(check[i+1]);						
				}
				i++;
			}else if(iVal-iJian1Val==0){
				i++;
			}else{
				break;
			}
		}
		
		if(delete.Count>=cnt){
		    foreach (int i in delete)
		    {
		        check.Remove(i);
		    }
			
			remove.AddRange(delete);
			return true;
		}
		
		return false;
	}
	
	protected bool checkSanTongHua(PlayerDuns pDun,List<int> allCards){
		
		CnList cnList = new CnList(allCards);
		if(cnList.Count==3){
			CnList list3 = cnList.GetEqualTo(3, true, true);
			CnList list5 = cnList.GetEqualTo(5, true, true);
			
			if(list3.Count==1&&list5.Count==2){
				pDun.Duns[0].Cards.Clear();
				pDun.Duns[0].Cards.AddRange(list3[0].Cards);
				pDun.Duns[1].Cards.Clear();
				pDun.Duns[1].Cards.AddRange(list5[0].Cards);
				pDun.Duns[2].Cards.Clear();
				pDun.Duns[2].Cards.AddRange(list5[1].Cards);
				return true;
			}	
		}else if(cnList.Count==2){
			CnList list3 = cnList.GetEqualTo(3, true, true);
			if(list3.Count==1){
				CnList list10 = cnList.GetEqualTo(10, true, true);
				if(list10.Count==1){
					pDun.Duns[0].Cards.Clear();
					pDun.Duns[0].Cards.AddRange(list3[0].Cards);
					pDun.Duns[1].Cards.Clear();
					pDun.Duns[1].Cards.AddRange(list10[0].Cards.GetRange(5, 5));
					pDun.Duns[2].Cards.Clear();
					pDun.Duns[2].Cards.AddRange(list10[0].Cards.GetRange(0, 5));
					return true;
				}
			}else if(list3.Count==0){
				CnList list5 = cnList.GetEqualTo(5, true, true);
				CnList list8 = cnList.GetEqualTo(8, true, true);
				if(list5.Count==1&&list8.Count==1){
					pDun.Duns[0].Cards.Clear();
					pDun.Duns[1].Cards.Clear();
					pDun.Duns[2].Cards.Clear();
					pDun.Duns[0].Cards.AddRange(list8[0].Cards.GetRange(0, 3));
					List<int> list8_5 = list8[0].Cards.GetRange(3, 5);
					List<List<int>> sort = new List<List<int>>();
					sort.Add(list8_5);
					sort.Add(list5[0].Cards);
					sort.Sort(( o1, o2)=>
					{
						for(int i = 0;i<o1.Count;i++){
							int o1val = GetValue(o1[i]);
							int o2val = GetValue(o2[i]);
							if(o1val-o2val!=0){
								return o1val-o2val;
							}
						}
						for(int i = 0;i<o1.Count;i++){
							int o1val = GetColor(o1[i]);
							int o2val = GetColor(o2[i]);
							if(o1val-o2val!=0){
								return o1val-o2val;
							}
						}
						return 0;
					});
					pDun.Duns[1].Cards.AddRange(sort[0]);
					pDun.Duns[2].Cards.AddRange(sort[1]);
					return true;
				}
			}
		}else if(cnList.Count==1){
			return true;
		}
		
		return false;
	}


        //获取玩家的dun
        public List<PlayerDuns> getPlayerDuns(List<int> cards)
        {

            //筛选出来合适的牌
            List<PlayerDuns> dunsOut = new List<PlayerDuns>();

            SssDun dun = new SssDun
            {
                Cards = cards,
                CardType = CardType.none
            };
            Root = new TreeNode(null, dun);
            SetTreeNode(cards, Root, 1);
            List<PlayerDuns> duns = new List<PlayerDuns>();
            GetTreeDuns(duns, Root);
            dunsOut.Add(duns[0]);
            if (duns.Count > 1)
            {
                for (int i = 1; i < duns.Count; i++)
                {
                    PlayerDuns dunTemp = duns[i];
                    bool isAdd = true;
                    foreach (PlayerDuns outDun in dunsOut)
                    {
                        if (dunTemp.CheckAllBig(outDun) != 1)
                        {
                            isAdd = false;
                        }
                    }
                    if (isAdd)
                    {
                        dunsOut.Add(dunTemp);
                    }
                }
            }

            //将牌型补全
            for (int i = 0; i < dunsOut.Count; i++)
            {
                List<int> temp = new List<int>(cards);
                PlayerDuns playerDun = dunsOut[i];
                for (int j = 0; j < 3; j++)
                {
                    SssDun pdun = playerDun.Duns[j];
                    if (pdun == null)
                        continue;
                    int cardCount = j == 0 ? 3 : 5;
                    if (pdun.Cards.Count > cardCount)
                        pdun.Cards = pdun.Cards.GetRange(0, cardCount);
                    for (int z = 0; z < pdun.Cards.Count; z++)
                    {
                        if (pdun.Cards[z] == 0)
                            continue;

                        temp.Remove(pdun.Cards[z]);
                    }
                }

                if (temp.Count <= 0)
                    continue;
                int count = 0;
                for (int j = 0; j < 3; j++)
                {
                    SssDun pdun = playerDun.Duns[j];
                    if (pdun == null)
                    {
                        pdun = new SssDun { Cards = temp.GetRange(0, j == 0 ? 3 : 5) };
                        pdun.CardType = CheckCardType(pdun.Cards, true);
                        playerDun.Duns[j] = pdun;
                    }

                    for (int z = 0; z < pdun.Cards.Count; z++)
                    {
                        if (pdun.Cards[z] == 0)
                            pdun.Cards[z] = temp[count++];
                    }
                }
            }

            //加入特殊牌型
            GetSpecialCardType(cards, dunsOut);

            return dunsOut;
        }


    }



    public enum CardType
    {
        //普通牌型
        sanpai = 0,
        yidui = 1,
        liangdui = 2,
        santiao = 3,
        shunzi = 4,
        tonghua = 5,
        hulu = 6,
        tiezhi = 7,
        tonghuashun = 8,

        none = tonghuashun + 1,

        //特殊牌型
        sanshunzi = 10,
        santonghua = 11,
        liuduiban = 12,
        wuduisan = 13,
        sitiaosan = 14,
        couyise = 15,
        quanxiao = 16,
        quanda = 17,
        sanzhadan = 18,
        santonghuashun = 19,
        shierhuang = 20,
        shisanshui = 21,
        tonghuashisanshui = 22,
    }


}