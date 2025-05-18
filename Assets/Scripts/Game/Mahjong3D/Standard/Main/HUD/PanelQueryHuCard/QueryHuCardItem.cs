using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class QueryHuCardItem : MonoBehaviour
    {
        /// <summary>
        /// 任意牌
        /// </summary>
        public Image Any;
        /// <summary>
        /// 牌
        /// </summary>
        public Image CardImg;
        /// <summary>
        /// 倍
        /// </summary>
        public Text RateValue;
        /// <summary>
        /// 数量标记
        /// </summary>
        public MahjongSign Sign;

        public virtual void SetData(int card, int num)
        {
            var flag = card != 0;
            Any.gameObject.SetActive(!flag);
            CardImg.gameObject.SetActive(flag);

            if (flag)
            {
                CardImg.sprite = GameCenter.Assets.GetMahjongSprite(card);
                CardImg.SetNativeSize();
                Sign.SetNumberSign(num);
            }
            else
            {
                Sign.OnReset();
            }

            //控制牌颜色
            var colorCtrl = gameObject.GetComponent<UIImageColorCtrl>();
            if (colorCtrl != null && card > 0)
            {
                if (num == 0)
                {
                    colorCtrl.SetImagesColor(new Color(150 / 255f, 150 / 255f, 150 / 255f));
                }
                else
                {
                    colorCtrl.SetImagesColor(Color.white);
                }
            }
        }

        public virtual int Rate
        {
            set
            {
                var text = value != 0 ? value : 1;
                RateValue.gameObject.SetActive(true);
                RateValue.text = text + "倍";
            }
        }
    }
}