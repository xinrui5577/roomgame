using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_TransformScaler : MonoBehaviour {
        public float ScaleTarget = 1.05F;
        public float ScaleStart = 0.95F;
        public float TimeElapse = 1F;
        void Start () {
            transform.localScale = Vector3.one * ScaleStart;
            iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one * ScaleTarget, "easeType", iTween.EaseType.easeInOutQuad, "loopType", iTween.LoopType.pingPong, "time", TimeElapse));
        }
 
    }
}
