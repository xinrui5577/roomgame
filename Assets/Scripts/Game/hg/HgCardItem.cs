using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.hg
{
    public class HgCardItem : MonoBehaviour
    {
        public UISprite CardBg;
        public UISprite CardMan;
        public UISprite CardKing;
        public UISprite CardValue;
        public UISprite CardColorTop;
        public UISprite CardColorCenter;
        /// <summary>
        /// 是否显示下面的花
        /// </summary>
        public bool NoShowCardCenter;

        private string _cardColor;
        private string _cardMan;
        private bool _isHasMan;

        private HgGameManager _gmanager
        {
            get { return App.GetGameManager<HgGameManager>(); }
        }

        public void SetDepth(int depth)
        {
            depth = depth * 4;
            CardBg.depth = depth+1;
            CardMan.depth = depth + 2;
            CardValue.depth = depth + 2;
            CardColorTop.depth = depth + 2;
            CardColorCenter.depth = depth + 2;
            if (CardKing)
            {
                CardKing.depth = depth + 2;
            }
        }

        public void RotateCardBg(int value,EventDelegate.Callback callback=null, bool isBig = false)
        {
            CardBg.GetComponent<TweenRotation>().PlayForward();

            if (isBig&& _gmanager.ResponseQueue.Count > 0)
            {
                gameObject.GetComponent<TweenScale>().PlayForward();
            }

            CardBg.gameObject.GetComponent<TweenRotation>().AddOnFinished(() =>
            {
                Facade.Instance<MusicManager>().Play("flipcard");
                if (gameObject.GetComponent<TweenScale>())
                {
                    gameObject.GetComponent<TweenScale>().PlayReverse();
                    gameObject.GetComponent<TweenScale>().AddOnFinished(callback);
                }

                SetCardValue(value);
            });
        }

        public void SetCardValue(int value)
        {
            CardShow(CardBg, "cardfront");
            if (CardKing != null)
            {
                if (value == 81 || value == 94)
                {
                    CardKing.spriteName = "smallKing";
                    CardKing.gameObject.SetActive(true);
                    return;
                }

                if (value == 95 || value == 97)
                {
                    CardKing.spriteName = "bigKing";
                    CardKing.gameObject.SetActive(true);
                    return;
                }
            }
           
            CardShow(CardValue, GetCardValue(value));

            if (_isHasMan)
            {
                CardShow(CardMan, _cardMan);
            }
            else
            {
                CardShow(CardColorCenter, _cardColor + "c");
            }

            if (!NoShowCardCenter)
            {
                CardShow(CardColorCenter, _cardColor + "c");
            }

            CardShow(CardColorTop, _cardColor + "t");
        }


        private void CardShow(UISprite sprite, string value)
        {
            sprite.spriteName = value;
            if (NoShowCardCenter)
            {
                sprite.MakePixelPerfect();
            }
            sprite.gameObject.SetActive(true);
        }

        public string GetCardValue(int cardValue)
        {
            var color = cardValue & 0xF0;
            var value = cardValue & 0x0F;

            var cardName = "";
            _cardColor = "";
            _cardMan = "";
            switch (color)
            {
                case 0x10:
                    cardName += "red_";
                    _cardColor += "f_";
                    _cardMan += "f_";
                    break;
                case 0x20:
                    cardName += "red_";
                    _cardColor += "r_";
                    _cardMan += "r_";
                    break;
                case 0x30:
                    cardName += "black_";
                    _cardColor += "m_";
                    _cardMan += "m_";
                    break;
                case 0x40:
                    cardName += "black_";
                    _cardColor += "b_";
                    _cardMan += "b_";
                    break;
            }

            if (value > 10)
            {
                if (value != 14 && value != 15)
                {
                    _isHasMan = true;
                }
                else
                {
                    _isHasMan = false;
                }
            }
            else
            {
                _isHasMan = false;
            }
            cardName += string.Format("{0:D2}", value);
            _cardMan += string.Format("{0:D2}", value);
            return cardName;
        }

        public void Clear()
        {
            CardBg.GetComponent<TweenRotation>().PlayReverse();
            CardBg.gameObject.GetComponent<TweenRotation>().onFinished.Clear();
            CardBg.spriteName = "cardback";
            CardMan.gameObject.SetActive(false);
            CardValue.gameObject.SetActive(false);
            CardColorTop.gameObject.SetActive(false);
            CardColorCenter.gameObject.SetActive(false);
        }
    }
}
