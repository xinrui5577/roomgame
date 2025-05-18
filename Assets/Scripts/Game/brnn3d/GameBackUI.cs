using UnityEngine;

namespace Assets.Scripts.Game.brnn3d
{
    public class GameBackUI : MonoBehaviour
    {
        public Transform GameBackUibg;

        public void ShowSelf()
        {
            if (!GameBackUibg.gameObject.activeSelf)
                GameBackUibg.gameObject.SetActive(true);
        }

        public void HideSelf()
        {
            if (GameBackUibg.gameObject.activeSelf)
                GameBackUibg.gameObject.SetActive(false);
        }

    }
}

