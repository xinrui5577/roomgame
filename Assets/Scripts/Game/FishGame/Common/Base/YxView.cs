using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Common.Base
{
    public class YxView : MonoBehaviour
    {

        // Use this for initialization
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
	
        // Update is called once per frame
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
