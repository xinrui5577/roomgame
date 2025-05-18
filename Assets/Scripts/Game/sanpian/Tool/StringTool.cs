using System;
using System.Collections.Generic;

namespace Assets.Scripts.Game.sanpian.Tool
{
    public class StringTool {
        public static String CardsToString(int[] cards )
        {
            String str = "";
            int len = cards.Length;
            for (int i = 0; i < len; i++)
            {
                str += "[ " + cards[i] / 16 + ":" + cards[i] % 16 + " ]";
            }
            return str;
        }

        public static String CardsToString(List<int> cards)
        {
            String str = "";
            int len = cards.Count;
            for (int i = 0; i < len; i++)
            {
                str += "[ " + cards[i] / 16 + ":" + cards[i] % 16 + " ]";
            }
            return str;
        }
    }
}
