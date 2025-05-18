using System.Collections.Generic;

namespace Assets.Scripts.Game.biji.Modle
{
    public class ColCntList : List<ColCntItem>
    {
        public ColCntList(){}

        public ColCntList(List<int> cards)
        {
            foreach (var t in cards)
            {
                AddCard(t);
            }
        }

        public ColCntItem FindItemByCard(int card)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Color == Tools.GetColor(card))
                {
                    return this[i];
                }
            }

            return null;
        }

        public void AddCard(int card)
        {
            ColCntItem item = FindItemByCard(card);
            if (item == null)
            {
                item = new ColCntItem(Tools.GetColor(card));
                Add(item);
            }

            item.AddCard(card);
        }

        private class ComparatorCnList : Comparer<ColCntItem> 
        {
            public int Cnt;

            public override int Compare(ColCntItem o1, ColCntItem o2)
            {
                for (int i = 0; i < Cnt; i++)
                {
                    int val1 = Tools.GetValue(o1.Cards[i]);
                    int val2 = Tools.GetValue(o2.Cards[i]);
                    if (val1 - val2 != 0)
                    {
                        return -(val1 - val2);
                    }
                }
                return 0;
            }
        }

        public void SortByVal(int cnt)
        {
            ComparatorCnList cmp = new ComparatorCnList();
            cmp.Cnt = cnt;
            Sort((a, b) => cmp.Compare(a, b));
        }

        public ColCntList GetMoreThan(int cnt)
        {
            ColCntList ret = new ColCntList();

            for (int i = 0; i < Count; i++)
            {
                if (this[i].Cnt() >= cnt)
                {
                    ret.Add(this[i]);
                }
            }

            return ret;

        }
    }
}
