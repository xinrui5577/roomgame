using Assets.Scripts.Game.ddz2.InheritCommon;
using UnityEngine;
using YxFramwork.Common;


namespace Assets.Scripts.Game.ddz2.DDzGameListener.AnimationsScritp
{
    public class StartAnim : Anim
    {

        public UISprite[] RoundNumSprites;

        public ParticleSystem Particl;
   

        private void PlayTween()
        {
            var tweens = GetComponentsInChildren<UITweener>();
            PlayTweenAnim(tweens);
            gameObject.SetActive(true);
        }

        private void PlayTweenAnim(UITweener[] tweens)
        {
            int len = tweens.Length;
            for (int i = 0; i < len; i++)
            {
                var tw = tweens[i];
                tw.ResetToBeginning();
                tw.PlayForward();
            }
        }

        /// <summary>
        /// 设置局数贴图
        /// </summary>
        private void SetNumSprite()
        {
            int curRound = App.GetGameData<DdzGameData>().CurrentRound;
            int len = RoundNumSprites.Length;
            //从个位到高位设置数字
            for (int i = 0; i < len; i++)
            {
                var sp = RoundNumSprites[i];
                sp.spriteName = (curRound % 10).ToString();
                curRound /= 10;
            }
        }

        void PlayEffect()
        {
            Particl.Play();
        }



        public override void Play()
        {
            gameObject.SetActive(true);
            SetNumSprite();
            PlayTween();
            PlayEffect();
        }

        public override void Stop()
        {
            gameObject.SetActive(false);
        }
    }
}