using System.Collections.Generic;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.NnRecordWindow
{
    public class NnRecordDetialItem : YxView
    {
        public List<UISprite> Cards;
        public UISprite NiuType;
        public UISprite NoRob;
        public UISprite Rob;
        public UISprite BetBg;
        public UISprite BankSprite;

       
        public void InitCardsValue(List<object> cardsValue)
        {
            for (int i = 0; i < cardsValue.Count; i++)
            {
                Cards[i].spriteName = "0x" + int.Parse(cardsValue[i].ToString()).ToString("X");
            }
        }
    }
}
