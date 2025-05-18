using System.Collections.Generic;

namespace Assets.Scripts.Game.sanpian.Tool
{
    public class SortCardsTool  {

        public static void SortCards(List<int> cards)
        {
            cards.Sort();
            cards.Sort((a, b) =>
            {
                if (a % 16 > b % 16) return -1;
                if (a % 16 < b % 16) return 1;
                return 0;
            }
                );
        }
    }
}
