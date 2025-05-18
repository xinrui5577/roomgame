using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.sssjp
{
    public class HandLineInfoSk1 : LineInfo
    {

        public UISprite[] BgSprites;

        public GameObject ColorBar;

        public override void Show()
        {
            if (ColorBar != null) ColorBar.SetActive(true);          

            //SetBgSprite();
            ShowLabels();
        }


        public override void Hide()
        {
            
        }

        public override void Reset()
        {
            if (ColorBar != null) ColorBar.SetActive(false);

            NormalScore = 0;
            NormalScoreLabel.gameObject.SetActive(false);
            AddScore = 0;
            CardTypeLabel.gameObject.SetActive(false);
            foreach (var sp in BgSprites)
            {
                sp.gameObject.SetActive(false);
            }
        }

       
        private void ShowLabels()
        {
            NormalScoreLabel.gameObject.SetActive(true);
            CardTypeLabel.gameObject.SetActive(true);
        }

        void SetBgSprite()
        {
            bool addType = IsAddType();
            string spriteName = addType ? "bg2" : "bg1";

            int len = BgSprites.Length;
            for (int i = 0; i < len; i++)
            {
                var sprite = BgSprites[i];
                sprite.gameObject.SetActive(true);
                sprite.spriteName = spriteName;
                sprite.fillAmount = 0;
            }

            StartCoroutine(SpreadBg());     //伸展背景动画
        }

        //伸展动画
        private IEnumerator SpreadBg()
        {
            float amount = 0;
            while (amount < 1f)
            {
                amount += Time.deltaTime * 3;
                amount = amount > 1 ? 1 : amount;
                int len = BgSprites.Length;
                for (int i = 0; i < len; i++)
                {
                    var sprite = BgSprites[i];
                    sprite.fillAmount = amount;
                }
                yield return null;
            }
        }


        bool IsAddType()
        {
            return Type > CardType.hulu || (Type == CardType.santiao && Line == 0);
        }
    }
}
