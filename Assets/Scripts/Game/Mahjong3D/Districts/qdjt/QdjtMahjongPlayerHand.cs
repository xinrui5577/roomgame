using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class QdjtMahjongPlayerHand : MahjongPlayerHand
    {
        private MahjongContainer mChooseTingItem;
        private int[] mLiangCardsSave;

        protected override void SwitchChooseTingState(params object[] args)
        {
            MahjongContainer item;
            List<int> tingList = args[0] as List<int>;
            if (tingList == null || tingList.Count == 0) return;
            var list = MahjongList;
            for (int i = 0; i < list.Count; i++)
            {
                item = list[i];
                item.ResetPos();
                if (!tingList.Contains(item.Value))
                {
                    item.Lock = true;
                    item.RemoveMahjongScript();
                }
                else
                {
                    item.SetMahjongScript();
                    item.SetThowOutCall(TingpaiClickEvent);
                }
            }
            MahjongContorl.ClearSelectCard();
        }

        /// <summary>
        /// 听牌点击事件
        /// </summary>      
        private void TingpaiClickEvent(Transform transf)
        {
            if (HasToken)
            {
                var Mj = transf.GetComponent<MahjongContainer>();
                if (!Mj.Laizi && !Mj.Lock)
                {
                    mChooseTingItem = Mj;
                    ChooseLiangPai();
                }
            }
        }

        public void ChooseLiangPai()
        {
            List<int> cardsList = new List<int>();
            for (int i = 0; i < MahjongList.Count; i++)
            {
                if (mChooseTingItem != MahjongList[i])
                {
                    cardsList.Add(MahjongList[i].Value);
                }
            }
            int diaoCard = CheckQiDui(cardsList.ToArray());
            if (diaoCard > 0)
            {
                GameCenter.EventHandle.Dispatch<C2STingArgs>((int)EventKeys.C2SLiangdaoTing, (args) =>
                {
                    args.Card = mChooseTingItem.Value;
                    args.Prol = NetworkProls.Ting;
                    args.LiangCards = new[] { diaoCard };
                });
            }
            else
            {
                List<int[]> LiangAndGang = FindAutoLiang(cardsList.ToArray());
                int[] LiangCards = LiangAndGang[0];
                int[] GangCards = LiangAndGang[1];

                if (LiangCards.Length == 0) return;
                if (GangCards.Length == 0)
                {
                    GameCenter.EventHandle.Dispatch<C2STingArgs>((int)EventKeys.C2SLiangdaoTing, (args) =>
                    {
                        args.Card = mChooseTingItem.Value;
                        args.Prol = NetworkProls.Ting;
                        args.LiangCards = LiangCards;
                    });
                }
                else
                {
                    mLiangCardsSave = LiangCards;
                    var panel = GameCenter.Hud.GetPanel<PanelLiangdao>();
                    panel.Open(GangCards);
                }
            }
        }

        public void SendLiangPai(int[] ChooseGang)
        {
            List<int> LiangList = new List<int>(mLiangCardsSave);
            if (mLiangCardsSave == null || ChooseGang == null)
            {
                return;
            }
            for (int i = 0; i < ChooseGang.Length; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    LiangList.Add(ChooseGang[i]);
                }
            }
            List<int[]> LiangAndGang = FindAutoLiang(LiangList.ToArray(), false);
            int[] SendLiang = LiangAndGang[0]; 
            GameCenter.EventHandle.Dispatch<C2STingArgs>((int)EventKeys.C2SLiangdaoTing, (args) =>
            {
                args.Card = mChooseTingItem.Value;
                args.Prol = NetworkProls.Ting;
                args.LiangCards = SendLiang;
            });
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

        private List<int[]> FindAutoLiang(int[] cards, bool jianGang = true)
        {
            int[] handCards = new int[cards.Length];
            Array.Copy(cards, handCards, cards.Length);
            Array.Sort(handCards);
            //bool jiang = false;
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
                        handCards = temp_arr;
                        //jiang = true;
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
            for (i = 0; i < handCards.Length - 2; i++)
            {
                if (handCards[i] == handCards[i + 1])
                {
                    continue;
                }
                for (int j = i + 2; j < handCards.Length; j++)
                {
                    if (handCards[j] - handCards[i] > 2)
                    {
                        break;
                    }
                    if (handCards[j] - handCards[i] == 2 && handCards[j] != handCards[j - 1])
                    {
                        if (FindOtherTwoSameCards(handCards, handCards[j - 1]) || FindOtherTwoSameCards(handCards, handCards[j]) || FindOtherTwoSameCards(handCards, handCards[i]))
                        {
                            break;
                        }
                        if (FindFive(handCards[j - 1], huarr) || FindFive(handCards[i], huarr) || FindFive(handCards[j], huarr))
                        {
                            break;
                        }
                        int[] temp_arr = RemoveListItem(handCards, j - 1, 2);
                        temp_arr = RemoveListItem(temp_arr, i, 1);
                        int[] temp_hu = GetHuList(temp_arr);
                        if (EquitArr(temp_hu, huarr))
                        {
                            handCards = temp_arr;
                            i = -1;
                            break;
                        }
                    }
                }
            }

            //拣杠
            List<int> GangList = new List<int>();
            if (jianGang)
            {
                for (i = 0; i < handCards.Length - 2; i++)
                {
                    if (handCards[i] == handCards[i + 2])
                    {
                        bool flag = false;
                        for (int j = 0; j < huarr.Length; j++)
                        {
                            if (handCards[i] == huarr[j])
                            {
                                flag = true;
                            }
                        }
                        if (flag)
                        {
                            break;
                        }
                        int[] temp_arr = RemoveListItem(handCards, i, 3);
                        int[] temp_hu = GetHuList(temp_arr);
                        if (temp_hu.Length > 0)
                        {
                            GangList.Add(handCards[i]);
                            handCards = temp_arr;
                        }
                    }
                }
            }
            //亮和杠的数组，0是亮牌，1是杠牌
            List<int[]> LiangAndGangArr = new List<int[]>();
            LiangAndGangArr.Add(handCards);
            LiangAndGangArr.Add(GangList.ToArray());
            return LiangAndGangArr;
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


        private int[] RomoveArrFromArr(int[] cards, int[] RomoveCards)
        {
            if (cards.Length <= RomoveCards.Length)
            {
                return null;
            }
            int[] m_cards = new int[cards.Length - RomoveCards.Length];
            int index = 0;
            int m_cards_index = 0;
            bool flag = true;
            for (int i = 0; i < cards.Length; i++)
            {
                if (flag && RomoveCards[index] == cards[i])
                {
                    index++;
                    if (index == RomoveCards.Length)
                    {
                        flag = false;
                    }
                    continue;
                }
                m_cards[m_cards_index] = cards[i];
                m_cards_index++;
            }
            return m_cards;
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
    }
}
