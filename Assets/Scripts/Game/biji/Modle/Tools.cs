namespace Assets.Scripts.Game.biji.Modle
{
    public class Tools
    {
        public static int GetValue(int card)
        {
            if (card == 81 || card == 97)
            {
                return card;
            }
            return card & 0xf;
        }

        public static int GetColor(int card)
        {
            return card & 0xf0;
        }

        public static bool IsLaizi(int card)
        {
            return card > 0x50;
        }

        public static int GetColorVal(int card)
        {
            int color = GetColor(card);
            if (color == 0x10)
            {
                return 1;
            }
            else if (color == 0x20)
            {
                return 3;
            }
            else if (color == 0x30)
            {
                return 2;
            }
            else if (color == 0x40)
            {
                return 4;
            }
            else
            {
                return 0;
            }
        }

        public static bool CheckHongHei(int card)
        {
            if (card == 0x61)
            {
                return true;
            }
            if (card == 0x51)
            {
                return false;
            }
            int color = GetColor(card);
            if (color < 0x30)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
