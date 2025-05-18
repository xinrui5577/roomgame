using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jsys
{
    public class HaidiManager : MonoBehaviour
    {
        public Camera CameraLand;
        public Camera CameraSky;
        public Sprite[] Sprites;

        public Image Bg;
        public GameObject LandBg;
        public GameObject Senlinbg;
        public GameObject Senlin;
        public GameObject Banjiangtai;

        public void SetBgSprite(int no)
        {
            Bg.sprite = Sprites[no];
            if (no == 0)
            {
                CameraLand.gameObject.SetActive(true);
                CameraSky.gameObject.SetActive(false);
                LandBg.SetActive(true);
            }
            else if (no==1)
            {
                Banjiangtai.SetActive(false);
                CameraLand.gameObject.SetActive(false);
                CameraSky.gameObject.SetActive(true);
                LandBg.SetActive(false);
            }
            else 
            {
                Banjiangtai.SetActive(false);
                CameraLand.gameObject.SetActive(false);
                CameraSky.gameObject.SetActive(false);
            }
        }
    }

}

