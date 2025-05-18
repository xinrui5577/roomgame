using System;

namespace Assets.Scripts.Game.sanpian.item
{
    [Serializable]
    public class PoKerCard
    {
        /// <summary>
        /// 花色
        /// </summary>
        public int Colour;
        /// <summary>
        /// 面值
        /// </summary>
        public int Value;
        /// <summary>
        /// 传输值
        /// </summary>
        public int Id;

        public PoKerCard()
        {
        }

        public PoKerCard(int id)
        {
            SetCardId(id);
        }

        private int GetColor(int id)
        {
            return id >> 4;
        }

        private int GetValue(int id)
        {
            id = id & 0xf;
            return id;
        }

        public void SetCardId(int id)
        {
            Id = id;
            Colour = GetColor(id);
            Value = GetValue(id);
        }

        public string GetCardValueStr()
        {
            string result = Id == 0 ? "back" : "front";
            result = (Id == 81 || Id == 97 || Id == 113) ? "0x" + Id.ToString("x") : result;
            return result;
        }

        public string GetCardShowNumStr()
        {
            string result = (Colour == 1 || Colour == 2) ? "red_" + Value.ToString("x") : "black_" + Value.ToString("x");
            return result;
        }

        public string GetBigColorStr()
        {
            string result = "b_" + Colour + "_";
            result += (Value > 10 && Value < 14) ? Value.ToString("x") : "0";
            return result;
        }
    }
    
}