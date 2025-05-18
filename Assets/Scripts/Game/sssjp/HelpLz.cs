using System.Collections.Generic;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace Assets.Scripts.Game.sssjp
{
    public class HelpLz //: MonoBehaviour
    {

        //void Update()
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

        /// <summary>
        /// 是否有特殊牌型的要求
        /// </summary>
        public static bool Special = true;

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
        public static void SortList(List<int> list, bool up = true)
        {
            int type = up ? 1 : -1;
            list.Sort((x, y) =>
            {
                int vx = GetValue(x);
                int vy = GetValue(y);
                if (vx == vy)
                {
                    return type*(x - y);
                }
                return type*(vx - vy);
            });
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
                if (node.Parent == null || node.Parent.Parent == null) return;
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
        /// <param name="cardsList"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public CardType CheckCardType(List<int> cardsList, bool sort)
        {
            var tempList = new List<int>(cardsList);
            int cardsCount = cardsList.Count >= 5 ? 5 : 3;
            
            if (sort)
                SortList(tempList, false);
            var magicList = SeparateMagicCard(tempList);
            VnList vnlist = new VnList(tempList);
            int magicCount = magicList.Count;
            
            if (IsWuTong(vnlist, magicCount))
            {
                return CardType.wutong;
            }

            bool tonghua = IsTonghua(new CnList(tempList));
            bool shunzi = IsShunZi(vnlist, magicCount, cardsCount);

            if (tonghua && shunzi)
            {
                return CardType.tonghuashun;
            }

            if (IsTiezhi(vnlist, magicCount))
            {
                return CardType.tiezhi;
            }
            if (IsHulu(vnlist, magicCount))
            {
                return CardType.hulu;
            }
            if (tonghua)
            {
                return CardType.tonghua;
            }
            if (shunzi)
            {
                return CardType.shunzi;
            }
            
            if (IsSantiao(vnlist, magicCount))
            {
                return CardType.santiao;
            }
            if (IsLiangdui(vnlist, magicCount))
            {
                return CardType.liangdui;
            }
            if (IsYidui(vnlist, magicCount))
            {
                return CardType.yidui;
            }
            return CardType.sanpai;
        }

        bool IsWuTong(VnList vnList, int magicCount)
        {
            return vnList.GetMoreThan(5 - magicCount, true).Count > 0;
        }

        private bool IsTiezhi(VnList vnList, int magicCount)
        {
            return vnList.GetMoreThan(4 - magicCount, true).Count > 0;
        }



        bool IsShunZi(VnList vnList, int magicCount,int cardsCount = 5)
        {
            var cnt1 = vnList.GetMoreThan(1, true);
            if (cnt1[0].Val == 14) cnt1.Add(new VnItem(1));
            int count = cnt1.Count;
            int checkCount = count + magicCount - cardsCount;
            if (checkCount < 0)
                return false;


            for (int i = 0; i < checkCount + 1; i++)
            {
                int lastVal = cnt1[i].Val;
                int tempMCount = magicCount;
                int shunziCounter = 1;
                for (int j = i + 1; j < count; j++)
                {
                    int thisVal = cnt1[j].Val;
                    thisVal = thisVal == 14 && lastVal < 10 ? 1 : thisVal;
                    if (lastVal - thisVal == 1)
                    {
                        lastVal = thisVal;
                        shunziCounter++;
                        if (shunziCounter >= cardsCount - magicCount)
                        {
                            if (cnt1[0].Val == 14)
                            {
                                cnt1.Remove(cnt1[count - 1]);
                            }
                            return true;
                        }
                    }
                    else if (tempMCount > 0)
                    {
                        j--;
                        lastVal--;
                        --tempMCount;
                    }
                    else
                    {
                        break;
                    }

                }
            }
            if (cnt1[0].Val == 14)
            {
                cnt1.Remove(cnt1[count - 1]);
            }
            return false;
        }

        bool IsTonghua(CnList cn)
        {
            return cn.Count < 2;
        }

        bool IsLiangdui(VnList vn, int magicCount)
        {
            int cnt2 = vn.GetMoreThan(2, true).Count;
            int cnt1 = vn.GetMoreThan(1, true).Count;
            return vn.GetMoreThan(2, true).Count + Mathf.Min(cnt1 - cnt2, magicCount) >= 2;
        }

        bool IsYidui(VnList vn, int magicCount)
        {
            return vn.GetMoreThan(2 - magicCount, true).Count > 0;
        }

        bool IsSantiao(VnList vn, int magicCount)
        {
            return vn.GetMoreThan(3 - magicCount, true).Count > 0;
        }

        bool IsHulu(VnList vn, int magicCount)
        {
            int cnt3 = vn.GetMoreThan(3, true).Count;
            int cnt2 = vn.GetMoreThan(2, true).Count;

            int score = cnt3 * 10 + cnt2;

            return magicCount > 0 ? score >= 2 : score >= 11;
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
                Cards.Add(card);
                Count++;
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

            public VnList GetMoreThan(int count, bool sort,bool up = true)
            {
                VnList list = new VnList();
                foreach (VnItem item in this)
                {
                    if (item.Count >= count)
                        list.Add(item);
                }
                if (sort && list.Count > 1)
                {
                    int type = up ? 1 : -1;
                    list.Sort((o1, o2) => type*(o2.Val - o1.Val));
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
                Count++;
                Cards.Add(card);
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
                    var cards1 = CardType == CardType.shunzi ? Cards.GetRange(1, Cards.Count - 2) : Cards;
                    var cards2 = CardType == CardType.shunzi ? dun.Cards.GetRange(1, dun.Cards.Count - 2) : dun.Cards;
                    VnList list1 = new VnList(cards1);
                    VnList list2 = new VnList(cards2);
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
                    var cards1 = CardType == CardType.shunzi ? Cards.GetRange(1, Cards.Count - 2) : Cards;
                    var cards2 = CardType == CardType.shunzi ? dun.Cards.GetRange(1, dun.Cards.Count - 2) : dun.Cards;
                    VnList list1 = new VnList(cards1);
                    VnList list2 = new VnList(cards2);
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
            public CardType SpecialType;
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
            var clone = new List<int>(cards);
            var laiziList = SeparateMagicCard(clone);
            int laiziCount = laiziList.Count;
            SortList(clone, false);

            bool hasA = GetValue(clone[0]) == 14;
            if (hasA) clone.Add(clone[0]);
            int cloneCount = clone.Count;
            int checkCount = cloneCount + laiziCount - 4;

            if (checkCount < 0)
            {
                return false;
            }

            for (int i = 0; i < checkCount + 1; i++)
            {
                int lastVal = clone[i];
                List<int> shunzi = new List<int> {lastVal};
                lastVal = GetValue(lastVal);

                int tempMCount = laiziCount;
                for (int j = i + 1; j < cloneCount; j++)
                {
                    int thisVal = GetValue(clone[j]);
                    thisVal = thisVal == 14 && lastVal < 10 ? 1 : thisVal;
                    if (lastVal == thisVal) continue;

                    if (lastVal - thisVal == 1)
                    {
                        lastVal = thisVal;
                        shunzi.Add(clone[j]);
                        if (shunzi.Count >= count - laiziCount)
                        {
                            SssDun dun = new SssDun
                            {
                                CardType = tonghua ? CardType.tonghuashun : CardType.shunzi,
                                Cards = shunzi
                            };
                            shunzi.AddRange(laiziList);
                            outList.Add(dun);
                            return true;
                        }
                    }
                    else if (tempMCount > 0)
                    {
                        j--;
                        lastVal--;
                        --tempMCount;
                    }
                    else
                    {
                        break;
                    }
                }
                if (shunzi.Count + tempMCount >= count)
                {
                    SssDun dun = new SssDun
                    {
                        CardType = tonghua ? CardType.tonghuashun : CardType.shunzi,
                        Cards = shunzi
                    };
                    dun.Cards.AddRange(laiziList.GetRange(0, count - shunzi.Count));
                    outList.Add(dun);
                    return true;
                }
            }



            if (hasA)
            {
                clone.Remove(clone[count - 1]);
            }
            return false;
        }

        /// <summary>
        /// 查看数组中的牌是否在vnlist中
        /// </summary>
        /// <param name="vnList"></param>
        /// <param name="dunList"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        bool CheckContainCard(VnList vnList, List<SssDun> dunList, int index)
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


        bool CheckLianXu(List<int> cardList,List<int> lzcardList,int checkCount,List<int> outList)
        {

            var checkList = new List<int>(cardList);
            var laiziList = new List<int>(lzcardList);

            //有A,加入到2之后
            SortList(checkList, false);
            if (GetValue(checkList[0]) == 14)
            {
                checkList.Add(checkList[0]);
            }

            int loopTime = checkList.Count;
            int laiziCount = laiziList.Count;
            var tempList = new List<int>();

            for (int i = 0; i < loopTime; i++)
            {
                tempList.Clear();
                int lastVal = checkList[i];
                tempList.Add(lastVal);
                int tempMcount = laiziCount;      //对癞子牌进行处理
                for (int j = i + 1; j < checkList.Count && tempList.Count < checkCount && tempMcount >= 0; j++)
                {
                    int thisVal = checkList[j];

                    int lval = GetValue(lastVal);
                    int tval = GetValue(thisVal);

                    if (lval == tval)
                    {
                    }
                    else if (lval - tval == 1)
                    {
                        tempList.Add(thisVal);
                        lastVal = thisVal;
                    }
                    else
                    {
                        if (tempMcount > 0)
                        {
                            tempList.Add(laiziList[laiziCount - tempMcount]);
                            tempMcount--;
                            lastVal--;
                        }
                    }
                }

                if (tempList.Count >= checkCount && !IsTonghua(new CnList(tempList)))
                {
                    outList.AddRange(tempList);
                    return true;
                }
            }


            return false;
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
        public void GetSpecialCardType(List<int> cards, List<PlayerDuns> dunsOut)       //特殊牌型无需太顾及癞子,所以只能用于ChoiseWay1
        {
            List<int> cloneCards = new List<int>(cards);
            List<int> magicList = SeparateMagicCard(cloneCards);
            int magicCount = magicList.Count;

            VnList vnList = new VnList(cloneCards);
            CnList cnList = new CnList(cloneCards);
            List<int> cardValues = vnList.GetValues();


            bool isShunzi = vnList.Count == cloneCards.Count;
            bool isTonghua = cnList.Count == 1;

            bool isJ_A = true;
            bool is8_A = true;
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
                    is8_A = false;
                }
                if (val > 8)
                {
                    is2_8 = false;
                }
                if (!isJ_A && !is8_A && !is2_8)
                {
                    break;
                }
            }


            List<int> valCountList = new List<int>();
            for (int i = 4; i > 1; i--)
            {
                valCountList.Add(vnList.GetMoreThan(i, false).Count);
            }
            valCountList.Add(vnList.GetValues().Count);     //1个的个数

            if (magicCount > 0)
            {
                int count = valCountList.Count;
                for (int i = 0; i < count - 1; i++)
                {
                    int mCount = magicCount;
                    int cntI = valCountList[i];
                    int shift = 1;

                    for (int j = i; j < count - 1; j++, shift++)
                    {
                        int shifCount = mCount / shift;
                        if (shifCount == 0) break;

                        int min = Mathf.Min(valCountList[j + 1] - valCountList[j], shifCount);
                        cntI += min;
                        mCount -= min * shift;
                    }

                    if (mCount >= count - i)
                    {
                        cntI += mCount / (count - i);
                    }
                    valCountList[i] = cntI;
                }
            }

            if (isShunzi && isTonghua)
            {
                PlayerDuns pDun = new PlayerDuns
                {
                    SpecialType = CardType.tonghuashisanshui,
                    Duns = dunsOut[0].Duns
                };
                dunsOut.Insert(0, pDun);
            }
            else if (isShunzi)
            {
                PlayerDuns pDun = new PlayerDuns
                {
                    SpecialType = CardType.shisanshui,
                    Duns = dunsOut[0].Duns
                };
                dunsOut.Insert(0, pDun);
            }
            else if (isJ_A)
            {
                PlayerDuns pDun = new PlayerDuns
                {
                    SpecialType = CardType.shierhuang,
                    Duns = dunsOut[0].Duns
                };
                dunsOut.Insert(0, pDun);
            }
            else
            {



                //PlayerDuns p = dunsOut[0];
                //CardType frist = CheckCardType(p.Duns[0].Cards, true);
                //if (p.Duns[1].CardType == CardType.tonghuashun && p.Duns[2].CardType == CardType.tonghuashun && frist == CardType.tonghuashun)
                //{
                //    PlayerDuns add = new PlayerDuns
                //    {
                //        SpecialType = CardType.santonghuashun,
                //        Duns = dunsOut[0].Duns
                //    };
                //    dunsOut.Insert(0, add);
                //}
                //else if (valCountList[0] >= 3)
                //{
                //    PlayerDuns add = new PlayerDuns
                //    {
                //        SpecialType = CardType.sanzhadan,
                //        Duns = dunsOut[0].Duns
                //    };
                //    dunsOut.Insert(0, add);
                //}
                //else if (is8_A)
                //{
                //    PlayerDuns add = new PlayerDuns
                //    {
                //        SpecialType = CardType.quanda,
                //        Duns = dunsOut[0].Duns
                //    };
                //    dunsOut.Insert(0, add);
                //}
                //else if (is2_8)
                //{
                //    PlayerDuns add = new PlayerDuns
                //    {
                //        SpecialType = CardType.quanxiao,
                //        Duns = dunsOut[0].Duns
                //    };
                //    dunsOut.Insert(0, add);
                //}
                //else if (cnList.Count == 2 && CheckSameColor(cnList[0].Color, cnList[1].Color))
                //{
                //    PlayerDuns add = new PlayerDuns
                //    {
                //        SpecialType = CardType.couyise,
                //        Duns = dunsOut[0].Duns
                //    };
                //    dunsOut.Insert(0, add);
                //}
                //else if (valCountList[1] >= 4)
                //{
                //    PlayerDuns add = new PlayerDuns
                //    {
                //        SpecialType = CardType.sitiaosan,
                //        Duns = dunsOut[0].Duns
                //    };
                //    dunsOut.Insert(0, add);
                //}
                //else if (valCountList[2] >= 7 && valCountList[1] >= 1)
                //{
                //    PlayerDuns add = new PlayerDuns
                //    {
                //        SpecialType = CardType.wuduisan,
                //        Duns = dunsOut[0].Duns
                //    };
                //    dunsOut.Insert(0, add);
                //}
                //else if (valCountList[2] >= 6)
                //{
                //    PlayerDuns add = new PlayerDuns
                //    {
                //        SpecialType = CardType.liuduiban,
                //        Duns = dunsOut[0].Duns
                //    };
                //    dunsOut.Insert(0, add);
                //}
                //else if (IsTongHua(p.Duns[1].CardType) && IsTongHua(p.Duns[2].CardType) && IsTongHua(frist))
                //{
                //    PlayerDuns add = new PlayerDuns
                //    {
                //        SpecialType = CardType.santonghua,
                //        Duns = dunsOut[0].Duns
                //    };
                //    dunsOut.Insert(0, add);
                //}
                //else if (IsShunZi(p.Duns[1].CardType) && IsShunZi(p.Duns[2].CardType) && IsShunZi(frist))
                //{
                //    PlayerDuns add = new PlayerDuns
                //    {
                //        SpecialType = CardType.sanshunzi,
                //        Duns = dunsOut[0].Duns
                //    };
                //    dunsOut.Insert(0, add);
                //}
            }
        }

        /// <summary>
        /// 判断两个颜色是不是同色(全为黑或者全为红)
        /// </summary>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <returns></returns>
        bool CheckSameColor(int color1, int color2)
        {
            int dif = Mathf.Abs(color1 - color2);
            int sum = color1 + color2;
            return dif == 0x10 && (sum == 0x30 || sum == 0x70);
        }


        bool IsTongHua(CardType cType)
        {
            return cType == CardType.tonghuashun || cType == CardType.tonghua;
        }

        /// <summary>
        /// 三顺子
        /// </summary>
        /// <param name="cardList"></param>
        /// <param name="outList"></param>
        /// <returns></returns>
        bool IsAllShunzi(List<int> cardList,List<int> outList)
        {
            var cloneList = new List<int>(cardList);
            var laiziList = SeparateMagicCard(cloneList);
            int count = cloneList.Count;
            var addList = new List<int>();
            for (int i = 0; i < count - 5 + 1; i++)
            {
                if (AllShunZi(cloneList, addList, laiziList, i))
                {
                    outList.AddRange(addList);
                    return true;
                }
                addList.Clear();
            }
            return false;
        }



        private bool AllShunZi(List<int> cardList, List<int> outCardList, List<int> lzcardList, int index,int checkCount = 5)
        {
            
            var checkList = new List<int>(cardList);
            var laiziList = new List<int>(lzcardList);
            int cardCount = checkList.Count;
            int laiziCount = laiziList.Count;
            int enoughCount = cardCount + laiziCount > 3 ? 5 : 3;
            

            //有A,加入到2之后
            SortList(checkList, false);
            if (GetValue(checkList[0]) == 14)
            {
                checkList.Add(checkList[0]);
            }
            
            var tempList = new List<int>();

            for (int i = index; i < cardCount; i++)
            {
                tempList.Clear();
                int lastVal = checkList[i];
                tempList.Add(lastVal);
                int tempMcount = laiziCount;      //对癞子牌进行处理
                for (int j = i + 1; j < checkList.Count && tempList.Count < enoughCount && tempMcount >= 0; j++)
                {
                    int thisVal = checkList[j];

                    int lval = GetValue(lastVal);
                    int tval = GetValue(thisVal);

                    if (lval == tval)
                    {
                    }
                    else if (lval - tval == 1)
                    {
                        tempList.Add(thisVal);
                        lastVal = thisVal;
                    }
                    else
                    {
                        if (tempMcount > 0)
                        {
                            int laizi = laiziList[0];
                            tempList.Add(laizi);
                            laiziList.Remove(laizi);
                            tempMcount--;
                            lastVal--;
                        }
                    }
                }

                if (tempList.Count >= enoughCount && !IsTonghua(new CnList(tempList)))
                {
                    outCardList.AddRange(tempList);
                    foreach (var item in tempList)
                    {
                        checkList.Remove(item);
                    }

                    if (checkList.Count <= 0)
                    {
                        outCardList.AddRange(laiziList);
                        return true;
                    }
                    else
                    {
                        //return IsSanShunZi(checkList);
                    }

                    
                }
            }

            return false;
        }


        bool IsShunZi(CardType cType)
        {
            return cType == CardType.tonghuashun || cType == CardType.shunzi;
        }

        public static List<int> SeparateMagicCard(List<int> cardsList)
        {
            List<int> magicList = new List<int>();
            for (int i = 0; i < cardsList.Count;)
            {
                int val = cardsList[i];
                if (val >= 0x51)
                {
                    magicList.Add(val);
                    cardsList.Remove(val);
                }
                else
                {
                    i++;
                }
            }

            return magicList;
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
            if (Special)
            {
                GetSpecialCardType(cards, dunsOut);
                if (dunsOut[0].SpecialType > CardType.none)
                    Debug.LogError(dunsOut[0].SpecialType);
            }
            return dunsOut;
        }

       

        public static List<int> SortLineList(List<int> cardList,CardType cardType)
        {
            var cloneList = new List<int>(cardList);
            var laiziList = SeparateMagicCard(cloneList);
            if (laiziList.Count > 1)
            {
                laiziList.Sort((o1, o2) => o2 - o1);
            }

            List<int> outList = new List<int>();
            switch (cardType)
            {
                //必然没有癞子
                case CardType.none:
                case CardType.sanpai:
                    outList = new List<int>(cloneList);
                    SortList(outList, false);
                    break;
                case CardType.liangdui:
                    MoveCards(outList, cloneList, 2);
                    MoveCards(outList, cloneList, 2);
                    outList.AddRange(cloneList);
                    break;
                
                //最多有一个癞子
                case CardType.yidui:
                    SortList(cloneList, false);
                    if (laiziList.Count > 0)
                    {
                        outList = new List<int>(cloneList);
                        SortList(laiziList);
                        outList.InsertRange(0, laiziList);
                    }
                    else
                    {
                        MoveCards(outList, cloneList, 2);
                        outList.AddRange(cloneList);
                    }
                    break;
                case CardType.hulu:
                    bool huluLaizi = laiziList.Count > 0;
                    if (huluLaizi)
                    {
                        MoveCards(outList, cloneList, 2);
                        MoveCards(outList, cloneList, 2);
                        outList.InsertRange(0,laiziList);
                    }
                    else
                    {
                        MoveCards(outList, cloneList, 3);
                        MoveCards(outList, cloneList, 2);
                    }
                    outList.AddRange(cloneList);

                    break;

                //多个癞子
                case CardType.santiao:
                    SortList(cloneList, false);
                    int laiziCount = laiziList.Count;
                    int check = 3 - laiziCount;
                    //cn2 = vnlist.GetMoreThan(3 - laiziCount, true, false);
                    MoveCards(outList, cloneList, check);
                    outList.AddRange(cloneList);
                    outList.InsertRange(0, laiziList);
                    break;

                case CardType.tonghuashun:
                case CardType.shunzi:
                    int shunziCount = cloneList.Count;
                    SortList(cloneList);
                    int lastVal = cloneList[shunziCount - 1];
                    int lastGetVal = GetValue(lastVal);
                    if (lastGetVal == 14 && GetValue(cloneList[shunziCount - 2]) < 7)
                    {
                        cloneList.Remove(lastVal);
                        cloneList.Insert(0, lastVal);
                        lastVal = cloneList[shunziCount - 1];
                        lastGetVal = GetValue(lastVal);
                    }

                    cloneList.Remove(lastVal);
                    outList.Add(lastVal);

                    for (int i = shunziCount - 2; i >= 0;)
                    {
                        int thisVal = cloneList[i];
                        int thisGetVal = GetValue(thisVal);
                        thisGetVal = thisGetVal == 14 ? 1 : thisGetVal;
                        if (lastGetVal == thisGetVal)
                        {
                            i--;
                        }
                        else if (lastGetVal - thisGetVal == 1)
                        {
                            i --;
                            lastGetVal = thisGetVal;
                            cloneList.Remove(thisVal);
                            outList.Add(thisVal);
                        }
                        else
                        {
                            if (laiziList.Count > 0)
                            {
                                int laizi = laiziList[0];
                                laiziList.Remove(laizi);
                                outList.Add(laizi);
                                lastGetVal--;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                  
                    outList.AddRange(cloneList);

                    if (laiziList.Count > 0)
                    {
                        int c = 14 - GetValue(outList[0]);
                        int laiziC = laiziList.Count;
                        c = c > laiziC ? laiziC : c;
                        outList.InsertRange(0, laiziList.GetRange(0, c));
                        outList.AddRange(laiziList.GetRange(c, laiziC - c));
                       
                    }
                    break;

                case CardType.tonghua:
                    outList = new List<int>(cloneList);
                    if (laiziList.Count > 0)
                    {
                        outList.AddRange(laiziList);
                    }
                    SortList(outList, false);
                    break;

                case CardType.tiezhi:
                    bool haveLaizi = laiziList.Count > 0;
                    int checkCount = haveLaizi ? 4 - laiziList.Count : 4;
                    MoveCards(outList, cloneList, checkCount);
                    
                    if (haveLaizi)
                    {
                        outList.InsertRange(0, laiziList);
                    }
                    outList.AddRange(cloneList);
                    break;
               
                case CardType.wutong:
                    if (laiziList.Count > 0)
                    {
                        outList.AddRange(laiziList);
                    }
                    outList.AddRange(cloneList);
                    break;
            }
            return outList;
        }

        /// <summary>
        /// 将指定张数的牌移动到另一个中
        /// </summary>
        /// <param name="tarList"></param>
        /// <param name="fromList"></param>
        /// <param name="checkCount"></param>
        protected static void MoveCards(List<int> tarList, List<int> fromList,int checkCount)
        {
            var vnlist = new VnList(fromList);
            var mtList = vnlist.GetMoreThan(checkCount, true);
            if (mtList.Count == 0) return;
            var cardList = mtList[0].Cards;
            int count = cardList.Count;
            for (int i = 0; i < count; i++)
            {
                int val = cardList[i];
                tarList.Add(val);
                fromList.Remove(val);
            }
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
        wutong = 9,

        none = 51,

        //特殊牌型
        sanshunzi = 100,
        santonghua = 110,
        liuduiban = 120,
        wuduisan = 130,
        sitiaosan = 140,
        couyise = 150,
        quanxiao = 160,
        quanda = 170,
        sanzhadan = 180,
        santonghuashun = 190,
        shierhuang = 200,
        shisanshui = 210,
        tonghuashisanshui = 220,
    }
}