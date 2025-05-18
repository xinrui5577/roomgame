using YxFramwork.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class PlayerInfoItem : YxBaseGamePlayer
    {
        public Transform TuoweiEffect;
        public Transform Banker;
        public Transform Owner;
        public Transform Ting;

        public Image OtherImage;

        public void SetHeadOtherImage(bool flag, Sprite sprite)
        {
            if (OtherImage == null) return;

            OtherImage.gameObject.SetActive(flag);
            if (flag)
            {
                OtherImage.sprite = sprite;
            }
        }

        public void AddGlod(long score)
        {
            Coin += score;
        }

        public void SetGlod(long score)
        {
            Coin = score;
        }

        public bool IsCurrOp { set { TuoweiEffect.gameObject.SetActive(value); } }
        public bool IsBanker { set { Banker.gameObject.SetActive(value); } }
        public bool IsOwner { set { Owner.gameObject.SetActive(value); } }
        public bool IsTing { set { Ting.gameObject.SetActive(value); } }

        public void Reset()
        {
            Ting.ExCompHide();
            Banker.ExCompHide();
            TuoweiEffect.ExCompHide();

            if (OtherImage != null)
            {
                OtherImage.gameObject.SetActive(false);
            }
        }
    }
}