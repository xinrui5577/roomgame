using UnityEngine;

namespace Assets.Scripts.Hall.View.AlllShowRecordWindow
{
    public class OpenOtherWindow : MonoBehaviour
    {
        public GameObject Bg;

        public void OnOpenOther()
        {
           Bg.SetActive(!Bg.activeSelf);
        }
    }
}
