using System;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelOtherHuTip), UIPanelhierarchy.Popup)]
    public class PanelOtherHuTip : UIPanelBase
    {
        public LianghuGrid[] LianghuGrid;

        private Dictionary<int, int[]> mHulist = new Dictionary<int, int[]>();

        public void Open(int[] cards, int chair)
        {
            base.Open();
            var huCards = getHuList(cards);
            var tChair = chair.ExChairC2T();

            mHulist[tChair] = huCards;
            LianghuGrid[tChair].SetHuCard(huCards);
        }

        public override void OnEndGameUpdate()
        {
            Close();
            mHulist.Clear();
            for (int i = 0; i < LianghuGrid.Length; i++)
            {
                LianghuGrid[i].OnReset();
            }
        }

        public void OnTipClick(int tChair)
        {
            LianghuGrid[tChair].FrameCtrl();
        }

        private int[] getHuList(int[] cards)
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
    }
}
