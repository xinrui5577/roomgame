using UnityEngine;

namespace Assets.Scripts.Game.jsys
{
    public class TurnImageManager : MonoBehaviour
    {

        public bool IsShowing = false;

       
        public void OnEnable()
        {
            if (IsShowing)
                Invoke("HideSelf", 0.12f);
        }
        private void HideSelf()
        {
            gameObject.SetActive(false);
            IsShowing = false;
        }

    }

}

