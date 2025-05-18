using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ZhaniaoItem : MonoBehaviour
    {
        /// <summary>
        /// 麻将牌背景
        /// </summary>
        public Image BgImage;
        /// <summary>
        /// 麻将牌
        /// </summary>
        public Image CardImage;
        /// <summary>
        /// 动画
        /// </summary>
        public Animator Animator;
        /// <summary>
        /// 中码特效
        /// </summary>
        public ParticleSystem PrizeEffer;
        /// <summary>
        /// 未中码
        /// </summary>
        public GameObject Black;

        public int Card { get; set; }

        public void OnInit(int card)
        {
            gameObject.SetActive(true);
            Animator.gameObject.SetActive(true);

            Animator.enabled = false;
            Animator.GetComponent<Image>().sprite = BgImage.sprite;

            Card = card;
            CardImage.sprite = GameCenter.Assets.GetMahjongSprite(card);
            CardImage.SetNativeSize();
        }

        public void OnReset()
        {
            Card = 0;
            Black.gameObject.SetActive(false);
            Animator.gameObject.SetActive(false);
            CardImage.gameObject.SetActive(false);
            PrizeEffer.gameObject.SetActive(false);
        }

        public void PlayShowAnimation()
        {
            Animator.enabled = true;
        }

        private void OnDisable()
        {
            OnReset();
        }

        public void PlayZhongmaEffect(bool isOn)
        {
            Black.gameObject.SetActive(!isOn);
            PrizeEffer.gameObject.SetActive(isOn);

            if (isOn)
            {
                PrizeEffer.Play();
            }
        }
    }
}