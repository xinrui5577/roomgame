using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    [ExecuteInEditMode]
    public class Ef_PixelLocaltor : MonoBehaviour {
        public float PixelX;
        public float PixelY;
        // Use this for initialization
#if !UNITY_EDITOR
	void Awake () {

        Destroy(this);

	}
#endif

#if UNITY_EDITOR
        // Update is called once per frame
        void Update () {
            Vector3 localPos = transform.localPosition;
            localPos.x = GameMain.WorldWidth * PixelX / 1600F;
            localPos.y = GameMain.WorldHeight * PixelY / 900F;
            transform.localPosition = localPos;
        }
#endif
    }
}
