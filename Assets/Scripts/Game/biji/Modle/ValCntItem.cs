using System.Collections.Generic;

namespace Assets.Scripts.Game.biji.Modle
{
    public class ValCntItem 
    {
        public int Value;
        public List<int> Cards;

        public ValCntItem(int value)
        {
            Cards = new List<int>();
            Value = value;
        }

        public void AddCard(int card)
        {
            if (Tools.GetValue(card) != Value)
            {
                return;
            }

            Cards.Add(card);
        }

        public int RemoveCard(int card)
        {
            if (Cards.Contains(card))
            {
                Cards.Remove(card);
                return card;
            }
            return -1;
        }

        public int RemoveOneCard()
        {
            if (Cards.Count > 0)
            {
                int c = Cards[0];
                Cards.RemoveAt(0);
                return c;
            }

            return -1;
        }

        public int Cnt()
        {
            return Cards.Count;
        }

        public int Compare(ValCntItem item)
        {
            if (Cnt() - item.Cnt() != 0)
            {
                return Cnt() - item.Cnt();
            }
            else
            {
                return Value - item.Value;
            }
        }
    }
}
