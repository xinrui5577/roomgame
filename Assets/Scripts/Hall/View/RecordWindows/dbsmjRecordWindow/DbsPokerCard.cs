using UnityEngine;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Hall.View.RecordWindows
{
    public class DbsPokerCard : MonoBehaviour
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
        /// 牌ID = 花色 + 面值
        /// </summary>
        public int Id;
        /// <summary>
        /// 扑克显示对象
        /// </summary>
        public UISprite Sprite;
        /// <summary>
        /// 显示牌型的线
        /// </summary>
        public UISprite Line;
        /// <summary>
        /// 显示牌值
        /// </summary>
        public UISprite Num;
        /// <summary>
        /// 小花色显示
        /// </summary>
        public UISprite SmallColor;
        /// <summary>
        /// 大花色显示
        /// </summary>
        public UISprite BigColor;

        /// <summary>
        /// 公共牌标记
        /// </summary>
        public UISprite PublicMark = null;

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

        /// <summary>
        /// 设置扑克显示成扑克
        /// </summary>
        public void SetCardFront()
        {
            Sprite.spriteName = GetCardValueStr();
            if (Id == 81 || Id == 97 || Id == 113)
            {
                Num.gameObject.SetActive(false);
                SmallColor.gameObject.SetActive(false);
                BigColor.gameObject.SetActive(false);
            }
            else
            {
                Num.gameObject.SetActive(true);
                SmallColor.gameObject.SetActive(true);
                BigColor.gameObject.SetActive(true);
                Num.spriteName = GetCardShowNumStr();
                Num.MakePixelPerfect();
                SmallColor.spriteName = "s_" + Colour + "_0";
                SmallColor.MakePixelPerfect();
                BigColor.spriteName = GetBigColorStr();
                BigColor.MakePixelPerfect();
            }
        }
    }
}
