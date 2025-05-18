using UnityEngine;

namespace Assets.Scripts.Game.bjlb
{
    public class HelpCtrl : MonoBehaviour
    {
        public GameObject HelpPanel;
        public void ToggleHelp()
        {
            HelpPanel.SetActive(!HelpPanel.activeSelf);
        }
    }
}
