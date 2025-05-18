using UnityEngine;
using System.Collections;


namespace Assets.Scripts.Game.ddz2.DDzGameListener.OneRoundResultPanel
{
    public class RecordItem : MonoBehaviour
    {

        /// <summary>
        /// 牌的面值(去掉花色后的值)
        /// </summary>
        public int CardVal = 0;

        /// <summary>
        /// 牌的剩余个数
        /// </summary>
        public int CardCount = 0;

        public UILabel CardMarkLabel;

        public UILabel CardCountLabel;

        public UISprite KingMarkSprite;


        void Start()
        {
            if (KingMarkSprite != null)
                KingMarkSprite.MakePixelPerfect();
        }

        public void SetSpriteMark(string spriteName)
        {
            if (KingMarkSprite == null) return;

            KingMarkSprite.spriteName = spriteName;
            KingMarkSprite.gameObject.SetActive(true);
        }

        /// <summary>
        /// 设置个数label的数值和颜色
        /// </summary>
        /// <param name="cardCount"></param>
        /// <param name="color"></param>
        public void SetCardCountLabel(int cardCount, Color color)
        {
            if (CardCountLabel == null) return;
            CardCountLabel.text = cardCount.ToString();
            CardCountLabel.color = color;
            CardCountLabel.gameObject.SetActive(true);
        }

        public void SetCardMarkLabel(string text, Color color)
        {
            if (CardMarkLabel == null) return;
            CardMarkLabel.text = text;
            CardMarkLabel.color = color;
            CardMarkLabel.gameObject.SetActive(true);
        }

        public void RemoveOne(Color color)
        {
            SetCardCountLabel(--CardCount, color);
        }

    }
}