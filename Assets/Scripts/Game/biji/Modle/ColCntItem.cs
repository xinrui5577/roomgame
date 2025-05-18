using System.Collections.Generic;

namespace Assets.Scripts.Game.biji.Modle
{
    public class ColCntItem
    {
        public int Color;
        public List<int> Cards;

        public ColCntItem(int color)
        {
            Cards = new List<int>();
            Color = color;
        }

        public void AddCard(int card)
        {
            if (Tools.GetColor(card) != Color)
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

        public void SortCards()
        {
            Cards.Sort((a, b) => -(Tools.GetValue(a) - Tools.GetValue(b)));
        }

        public int Cnt()
        {
            return Cards.Count;
        }
    }
}
