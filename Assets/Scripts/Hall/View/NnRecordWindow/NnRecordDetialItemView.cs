using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.Utiles;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.NnRecordWindow
{
    public class NnRecordDetialItemView : YxView
    {
        public UISprite Bg;
        public UILabel Round;
        public UISprite BtnPullDown;
        public UILabel GoldItem;
        public UIGrid GoldGrid;
        public UIGrid CardsGrid;
        public NnRecordDetialItem NnRecordDetialItem;

        public void OnFreshGold(List<int> golds)
        {
            foreach (var gold in golds)
            {
                var item = YxWindowUtils.CreateItem(GoldItem, GoldGrid.transform);
                item.text = gold.ToString(CultureInfo.InvariantCulture);
            }
            GoldGrid.repositionNow = true;
        }

        public void OnFreshCards(bool isHaveRob,bool isNoRob,int rob,int niuType,int ante,List<object> cardsValue,bool isBaker)
        {
            var item = YxWindowUtils.CreateItem(NnRecordDetialItem, CardsGrid.transform);
            if (isHaveRob)
            {
                item.Rob.gameObject.SetActive(true);
                item.Rob.GetComponentInChildren<UILabel>().text = "x" + rob;
            }
            if (isNoRob)
            {
                item.NoRob.gameObject.SetActive(true);
            }
            string niuValue="";
            if (niuType < 11)
            {
                niuValue = "n" + niuType;
            }
            else
            {
                switch (niuType)
                {
                    case 11:
                        niuValue = "nshn";
                        break;
                    case 12:
                        niuValue = "nszn";
                        break;
                    case 13:
                        niuValue = "nwhn";
                        break;
                    case 14:
                        niuValue = "nthn";
                        break;
                    case 15:
                        niuValue = "nhln";
                        break;
                    case 16:
                        niuValue = "nzdn";
                        break;
                    case 17:
                        niuValue = "nwxn";
                        break;
                    case 18:
                        niuValue = "nkln";
                        break;
                }
            }
            item.NiuType.spriteName = niuValue;
            if (isBaker)
            {
                 item.BankSprite.gameObject.SetActive(true);
            }
            else
            {
                 item.BetBg.gameObject.SetActive(true);
                 item.BetBg.GetComponentInChildren<UILabel>().text = ante.ToString(CultureInfo.InvariantCulture);
            }
            item.InitCardsValue(cardsValue);

        }

        public void OnPullDown()
        {
            BtnPullDown.GetComponent<UIButton>().normalSprite = BtnPullDown.spriteName == "pullDown" ? "pullUp" : "pullDown";
        }
    }
}
