using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_DengAnimationFlash : MonoBehaviour {
 
        void Start () {
            tk2dAnimatedSprite ani = GetComponent<tk2dAnimatedSprite>();
            ani.animationEventDelegate += Handle_AniSpriteFrameTrigger;
        }

        void Handle_AniSpriteFrameTrigger(tk2dAnimatedSprite sprite, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame, int frameNum)
        {
            Color c = sprite.color;
            c.a = frame.eventFloat;
            sprite.color = c;
        }
 
    }
}
