using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_AutoRotate : MonoBehaviour {
        public float Speed = 180F;
        private Transform mTs; 
        // Use this for initialization
        void Start () {
            mTs = transform; 
        }
	 
        void Update () {
            mTs.RotateAroundLocal(Vector3.forward, Speed *Mathf.Deg2Rad* Time.deltaTime); 
        }
    }
}
