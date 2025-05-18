using UnityEngine;

namespace Assets.Scripts.Game.bjlb.bjlb_skin02
{
    public class CardGroup02 : MonoBehaviour
    {

        public UISprite CardPrefab;

        public UISprite CoverCard;

        public TweenPosition TweenPos;

        public void SetGroup(int cardVal,int depth,Vector3 targetPos)
        {
            CardPrefab.spriteName = string.Format("0x{0}", cardVal.ToString("X"));
            CardPrefab.depth = depth;
            CoverCard.depth = depth + 1;

            TweenPos.to = targetPos;
        }

        public void Play()
        {
            TweenPlayForward(TweenPos);

            var tweens = GetComponentsInChildren<UITweener>();

            //播放牌的移动动画
            if (tweens == null) return;
            int len = tweens.Length;
            for (int i = 0; i < len; i++)
            {
                TweenPlayForward(tweens[i]);
            }
        }


        void TweenPlayForward(UITweener tween)
        {
            tween.ResetToBeginning();
            tween.PlayForward();
        }

    }
}
