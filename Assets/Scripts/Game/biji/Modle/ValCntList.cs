using System.Collections.Generic;

namespace Assets.Scripts.Game.biji.Modle
{
    public class ValCntList : List<ValCntItem>
    {
        public ValCntList(){}

        public ValCntList(List<int> cards)
        {
            foreach (var t in cards)
            {
                AddCard(t);
            }

            SortByNum();
        }

        public ValCntItem FindItemByValue(int val)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Value == val)
                {
                    return this[i];
                }
            }

            return null;
        }

        public void AddCard(int card)
        {
            int val =Tools.GetValue(card);
            ValCntItem find = FindItemByValue(val);
            if (find == null)
            {
                find = new ValCntItem(val);
                Add(find);
            }

            find.AddCard(card);
        }

        public void SortByNum()
        {
            Sort((a, b) => -a.Compare(b));
        }

        public int Compare(ValCntList list1)
        {
            if (list1.Count != Count)
            {
                return 0;
            }

            for (int i = 0; i < Count; i++)
            {
                int comp = this[i].Compare(list1[i]);
                if (comp != 0)
                {
                    return comp;
                }
            }

            return 0;
        }

        public ValCntList GetEqualCnt(int cnt, bool sort)
        {
            ValCntList list = new ValCntList();
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Cnt() == cnt)
                {
                    list.Add(this[i]);
                }
            }

            if (sort && list.Count > 0)
            {
                list.SortByNum();
            }

            return list;
        }

        public void SortByVale()
        {
            Sort((a, b) => b.Value - a.Value);
            }

        public ValCntList GetMoreThan(int cnt)
        {
            ValCntList list = new ValCntList();

            for (int i = 0; i < Count; i++)
            {
                if (this[i].Cnt() >= cnt)
                {
                    list.Add(this[i]);
                }
            }

            return list;
        }

        public bool ContainsValue(int val)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Value == val)
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

            for (int i = 0; i < Count; i++)
            {
                if (this[i].Cnt() > 0)
                {
                    cards.Add(this[i].Value);
                }
            }

            return cards;
        }
    }
}

      
