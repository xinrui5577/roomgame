using UnityEngine;

namespace Assets.Scripts.Game.FishGame.script
{
    public class UpFire : MonoBehaviour
    {
        public int FireLevel;
        // Use this for initialization
        void Start ()
        {
        }
        void OnMouseDown()
        {
            GameMain.Singleton.Operation.ChangeNextGunStyle();
        }

        void OnMouseUp()
        {
        }
        // Update is called once per frame
        void Update ()
        {
	
        }
    }
}
