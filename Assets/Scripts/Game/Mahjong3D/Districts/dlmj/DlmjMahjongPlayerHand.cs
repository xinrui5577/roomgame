using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class DlmjMahjongPlayerHand : MahjongPlayerHand
    {
        protected override void Start()
        {
            base.Start();
            AddActionToDic(HandcardStateTyps.ChooseNiuTing, SwitchChooseNiuTingState);
        }

        protected void SwitchChooseNiuTingState(params object[] args)
        {
            List<int> tingList = args[0] as List<int>;
            if (tingList == null || tingList.Count == 0) return;
            MahjongContainer item;
            MahjongContorl.ClearSelectCard();
            var list = mMahjongList;
            for (int i = 0; i < list.Count; i++)
            {
                item = list[i];
                item.ResetPos();
                if (!tingList.Contains(item.Value))
                {
                    item.Lock = true;
                    item.RemoveMahjongScript();
                }
            }
            for (int i = 0; i < tingList.Count; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    item = list[j];
                    if (item.Value == tingList[i])
                    {
                        item.SetMahjongScript();
                        item.SetThowOutCall(NiuTingClickEvent);
                    }
                }
            }
        }

        private void NiuTingClickEvent(Transform transf)
        {
            if (HasToken)
            {
                MahjongContainer item;
                var Mj = transf.GetComponent<MahjongContainer>();
                if (!Mj.Laizi && !Mj.Lock)
                {
                    HasToken = false;
                    DoSelectTdhNiuClick(Mj);
                    var list = mMahjongList;
                    for (int i = 0; i < list.Count; i++)
                    {
                        item = list[i];
                        item.SetMahjongScript();
                        item.SetThowOutCall(ThrowCardClickEvent);
                    }
                }
            }
        }

        #region 大连推到胡部分
        private void DoSelectTdhNiuClick(MahjongContainer mjItem)
        {
            var list = mMahjongList;
            List<int> cardValueList = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                cardValueList.Add(list[i].Value);
            }
            int index = 0;
            for (int i = 0; i < cardValueList.Count; i++)
            {
                if (mjItem.Value == cardValueList[i])
                {
                    index = i;
                    break;
                }
            }
            int[] handCard = RemoveListItem(cardValueList.ToArray(), index, 1);
            Array.Sort(handCard);
            int isQiDui = CheckQiDui(handCard);
            if (isQiDui > 0)
            {
                int[] finalNiu = { isQiDui };
                GameCenter.EventHandle.Dispatch<C2STingArgs>((int)EventKeys.C2SNiuting, (args) =>
                {
                    args.Card = mjItem.Value;
                    args.Prol = NetworkProls.Ting;
                    args.LiangCards = finalNiu;
                });              
                return;
            }
            else
            {
                int[] findNiu = FindAutoLiang(handCard);
                GameCenter.EventHandle.Dispatch<C2STingArgs>((int)EventKeys.C2SNiuting, (args) =>
                {
                    args.Card = mjItem.Value;
                    args.Prol = NetworkProls.Ting;
                    args.LiangCards = findNiu;
                });              
            }
            //如果打出的牌是抬起状态放下
            mjItem.ResetPos();
        }

        private int CheckQiDui(int[] list)
        {
            Array.Sort(list);
            if (list.Length == 13)
            {
                int single = 0;
                int card = 0;
                for (int i = 0; i < 12; i++)
                {
                    if (list[i] == list[i + 1])
                    {
                        i++;
                    }
                    else
                    {
                        card = list[i];
                        single++;
                    }
                    if (single > 1)
                    {
                        return 0;
                    }
                }
                return card == 0 ? list[list.Length - 1] : card;
            }
            return 0;
        }

        private int[] FindAutoLiang(int[] cards, bool jianGang = true)
        {
            int[] handCards = new int[cards.Length];
            Array.Copy(cards, handCards, cards.Length);
            Array.Sort(handCards);
            int[] huarr = GetHuList(handCards);

            int i;
            //移将
            for (i = 0; i < handCards.Length - 1; i++)
            {
                if (handCards[i] == handCards[i + 1])
                {
                    int[] temp_arr = RemoveListItem(handCards, i, 2);
                    int[] temp_hu = GetHuList(temp_arr);
                    if (EquitArr(temp_hu, huarr))
                    {
                        if (temp_hu.Length != 0)
                        {
                            handCards = temp_arr;
                        }
                        break;
                    }
                }
            }
            //AAA
            for (i = 0; i < handCards.Length - 2; i++)
            {
                if (handCards[i] == handCards[i + 2])
                {
                    int[] temp_arr = RemoveListItem(handCards, i, 3);
                    int[] temp_hu = GetHuList(temp_arr);
                    if (EquitArr(temp_hu, huarr))
                    {
                        handCards = temp_arr;
                        i--;
                    }
                }
            }

            // ABC
            //收集能亮的牌值
            List<int> collection = new List<int>();
            //现有手牌
            List<int> list = new List<int>(handCards);
            for (int j = 0; j < huarr.Length; j++)
            {
                FilterUncorrelated(huarr[j], list, collection);
            }
            //确定要亮得牌
            List<int> temp = new List<int>();
            for (int k = 0; k < collection.Count; k++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (collection[k] == list[j])
                    {
                        temp.Add(list[j]);
                    }
                }
            }
            return temp.ToArray();
        }

        /// <summary>
        /// 过滤 与胡牌不连续得牌
        /// </summary>
        private void FilterUncorrelated(int card, List<int> list, List<int> collection)
        {
            int count = list.Count;
            int temp = card;
            //手牌中是否有胡牌
            for (int i = 0; i < count; i++)
            {
                if (list[i] == temp)
                {
                    if (!collection.Contains(temp))
                    {
                        collection.Add(temp);
                    }
                }
            }
            //向后搜索
            temp = card - 1;
            for (int i = count - 1; i >= 0; i--)
            {
                //找到连续得牌
                if (list[i] == temp)
                {
                    if (!collection.Contains(temp))
                    {
                        collection.Add(temp);
                    }
                    //是否与下一张牌 相同
                    if (i > 0 && (list[i - 1] != temp))
                    {
                        temp--;
                    }
                }
            }
            temp = card + 1;
            //向前搜索
            for (int i = 0; i < count; i++)
            {
                //找到连续得牌
                if (list[i] == temp)
                {
                    if (!collection.Contains(temp))
                    {
                        collection.Add(temp);
                    }
                    //是否与下一张牌 相同
                    if (count - 1 > i && (list[i + 1] != temp))
                    {
                        temp++;
                    }
                }
            }
        }

        private int[] GetHuList(int[] cards)
        {
            List<int> hulist = new List<int>();
            int last = 0, last1 = 0;
            for (int i = 0; i < cards.Length; i++)
            {
                int testCard = cards[i];
                //这张牌就是上一张牌
                if (testCard == last)
                {
                    continue;
                }
                //这张牌不是上一张+1
                if (testCard - 1 != last1 && testCard - 1 != last)
                {
                    if (CheckCardValueRight(testCard - 1) && MakeArrToCheckHu(cards, testCard - 1))
                    {
                        hulist.Add(testCard - 1);
                    }
                }
                if (testCard != last1)
                {
                    if (CheckCardValueRight(testCard) && MakeArrToCheckHu(cards, testCard))
                    {
                        hulist.Add(testCard);
                    }
                }

                last1 = testCard + 1;
                if (CheckCardValueRight(last1) && MakeArrToCheckHu(cards, last1))
                {
                    hulist.Add(last1);
                }
                last = testCard;
            }
            return hulist.ToArray();
        }

        private bool CheckToHu(int[] list, bool checkJiang)
        {
            Array.Sort(list);
            int size = list.Length;
            if (size == 0)
            {
                return true;
            }
            int i;
            //将
            if (checkJiang)
            {
                for (i = 0; i < size - 1; i++)
                {
                    if (list[i] == list[i + 1])
                    {
                        if (CheckToHu(RemoveListItem(list, i, 2), false))
                        {
                            return true;
                        }
                        i++;
                    }
                }
            }
            //AAA
            for (i = 0; i < size - 2; i++)
            {
                if (list[i] == list[i + 2])
                {
                    if (CheckToHu(RemoveListItem(list, i, 3), false))
                    {
                        return true;
                    }
                }
            }
            //ABC
            for (i = 0; i < size - 2; i++)
            {
                if (list[i] == list[i + 1])
                {
                    continue;
                }
                for (int j = i + 2; j < size; j++)
                {
                    if (list[j] - list[i] > 2)
                    {
                        break;
                    }
                    if (list[j] - list[i] == 2 && list[j] != list[j - 1])
                    {
                        int[] cpList = RemoveListItem(list, j - 1, 2);
                        cpList = RemoveListItem(cpList, i, 1);
                        if (CheckToHu(cpList, false))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool CheckCardValueRight(int card)
        {
            return card % 16 > 0 && card % 16 < 10 || card == 74;
        }

        private bool MakeArrToCheckHu(int[] cards, int hu)
        {
            int[] m_cards = new int[cards.Length + 1];
            Array.Copy(cards, m_cards, cards.Length);
            m_cards[m_cards.Length - 1] = hu;
            return CheckToHu(m_cards, m_cards.Length % 3 == 2);
        }

        private bool FindOtherTwoSameCards(int[] handCards, int same)
        {
            int count = 0;
            for (int i = 0; i < handCards.Length; i++)
            {
                if (same == handCards[i])
                {
                    count++;
                }
            }
            return count > 2;
        }

        private bool FindFive(int card, int[] huarr)
        {
            if (!huarr.Contains(card) || card % 16 != 5)
            {
                return false;
            }
            foreach (var hucard in huarr)
            {
                if (hucard == card)
                {
                    return true;
                }
            }
            return false;
        }

        private bool EquitArr(int[] arr1, int[] arr2)
        {
            if (arr1.Length != arr2.Length)
            {
                return false;
            }
            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i])
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        private int[] RemoveListItem(int[] list, int index, int num)
        {
            int[] mList = new int[list.Length - num];
            for (int i = 0; i < index; i++)
            {
                mList[i] = list[i];
            }
            for (int i = index + num; i < list.Length; i++)
            {
                mList[i - num] = list[i];
            }
            return mList;
        }     
    }
}