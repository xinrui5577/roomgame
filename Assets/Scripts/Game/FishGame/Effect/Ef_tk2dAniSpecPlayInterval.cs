using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_tk2dAniSpecPlayInterval : MonoBehaviour {
        public float Interval = 15F;
        public string AniName = "刺猬鱼特殊动作0";

        private int mOriClipidx = 0;
        private tk2dSpriteAnimationClip mOriClip;
        private tk2dSpriteAnimator mAnispr;
        private bool mIsPlaySpecAni;//是否在播放指定动画

        private float mElapse;
        private float mSpecAniLength;
        void Awake()
        {
            mAnispr = GetComponent<tk2dSpriteAnimator>();
            if (mAnispr == null)
                return;
            mOriClip = mAnispr.DefaultClip;
            //mOriClipidx = mAnispr.DefaultClipId;
            tk2dSpriteAnimationClip aniClip = mAnispr.Library.clips[mAnispr.Library.GetClipIdByName(AniName)];
            mSpecAniLength = aniClip.frames.Length / aniClip.fps;

        }
        void Update()
        {
            if (mIsPlaySpecAni)
            {
                if (mElapse > mSpecAniLength)//转为播放原动画
                {
                    mElapse = 0F;
                    mIsPlaySpecAni = false;
                    //mAnispr.DefaultClipId = mOriClipidx;
                    mAnispr.PlayFrom(mOriClip,0F);
                }
            }
            else
            {
                if (mElapse > Interval)
                {
                    mElapse = 0F;
                    mIsPlaySpecAni = true;
                    mAnispr.PlayFrom(AniName, 0F);
                }
            }
            mElapse += Time.deltaTime;
        }

        void OnDisable()
        {
            if (GameMain.IsEditorShutdown)
                return;

            if (mAnispr == null)
                return;
            mElapse = 0F;
            mIsPlaySpecAni = false;
            //mAnispr.DefaultClipId = mOriClipidx;
            //mAnispr.Play(0F);
            mAnispr.PlayFrom(mOriClip, 0F);
        }

    
    }
}
