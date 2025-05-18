using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_tk2dAniSprFramePos : MonoBehaviour {
        public Vector3[] FramePositions;
        public GameObject goToChangePos;
        private Transform mTsGoToChangePos;
        // Use this for initialization
        void Awake () {
            tk2dSpriteAnimator aniSpr = GetComponent<tk2dSpriteAnimator>();
            aniSpr.AnimationEventTriggered += Handle_AnimationFrameEvent;
            mTsGoToChangePos = goToChangePos.transform;
        }

        //void Handle_AnimationFrameEvent(tk2dAnimatedSprite sprite, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame, int frameNum)
        void Handle_AnimationFrameEvent(tk2dSpriteAnimator sprAnimator,tk2dSpriteAnimationClip clip,int frameNum)
        {
            //Debug.Log("Handle_AnimationFrameEvent" + frameNum);
            mTsGoToChangePos.localPosition = FramePositions[frameNum % FramePositions.Length];
        } 
    }
}
