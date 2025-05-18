using UnityEngine;

namespace Assets.Scripts.Game.FishGame.script
{
    public class DownFire : MonoBehaviour 
    {
        // Use this for initialization
        void Start ()
        {
            // MobileInterface.SetPlayerFireScore(10);
        }
        void OnMouseDown()
        { 
            GameMain.Singleton.Operation.ChangePriorGunStyle();
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
