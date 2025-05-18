using Assets.Scripts.Game.jh.EventII;
using UnityEngine;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhHelpView : MonoBehaviour
    {
        public GameObject View;

        public void Show()
        {
            View.SetActive(true);
        }

        public void Hide()
        {
            View.SetActive(false);
        }

        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "Show":
                    Show();
                    break;
            }
        }
    }
}
