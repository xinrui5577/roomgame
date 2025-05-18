using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_PlayAnimationWhenEatFish : MonoBehaviour {
        public string AnimationToPlayName;
        // Use this for initialization
        private tk2dSpriteAnimationClip mOriginClip;
        private Fish mFish;
        public bool IsContPlay = false;
        void Awake () {
            FishEx_FishEatter fe = GetComponent<FishEx_FishEatter>();
            mFish = fe._Fish;
            mFish.AniSprite.AnimationCompleted += Handle_FishAniStop; 
            if (fe != null)
                fe.EvtBeforeEatFish += Handle_BeforeEatFish;

            mOriginClip = mFish.AniSprite.DefaultClip;
        }

        void Handle_BeforeEatFish(Fish eatter, Fish beEattedFish)
        {
            if (eatter.AniSprite.IsPlaying(AnimationToPlayName)
                && !IsContPlay)
                return;

            eatter.AniSprite.Play(AnimationToPlayName);
        }

        void Handle_FishAniStop(tk2dSpriteAnimator sprAni, tk2dSpriteAnimationClip aniClip)
        {
            if (mFish != null && mFish.Attackable)
            {
                mFish.AniSprite.PlayFrom(mOriginClip,0F);
            }
        
        }
    }
}
