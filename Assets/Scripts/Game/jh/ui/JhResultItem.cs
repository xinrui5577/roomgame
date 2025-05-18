using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhResultItem : MonoBehaviour
    {

        public UILabel Name;

        public UILabel Gold;

        public UISprite Icon;

        public List<JhCard> Cards;

        public void SetInfo(string uname,int gold,int icon,int[] cards)
        {
            Name.text = uname;
            string tt = "";
            Color fColor = Color.white;
            if (gold > 0)
            {
                tt = "+" + YxUtiles.ReduceNumber(gold);
                fColor = Color.green;
            }
            else
            {
                tt = "" + YxUtiles.ReduceNumber(gold);
                fColor = Color.red;
            }

            Gold.text = tt;
            Gold.color = fColor;

            if (icon == 0)
            {
                Icon.spriteName = "icon_win";
            }
            else if (icon == 1)
            {
                Icon.spriteName = "icon_fold";
            }
            else
            {
                Icon.spriteName = "icon_lose";
            }
            Icon.MakePixelPerfect();

            if (cards != null)
            {
                for (int i = 0; i < cards.Length; i++)
                {
                    Cards[i].SetValue(cards[i]);
                    Cards[i].ShowFront();
                }
            }

        }
    }
}
