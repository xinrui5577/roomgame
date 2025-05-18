using UnityEngine;

namespace Assets.Scripts.Game.FishGame.script
{
    public class Moudle_Setting : MonoBehaviour 
    {
        public GameObject Prefab_Setting;
        public GameObject go_Setting;
     
        // Use this for initialization
        void Start ()
        {
            Moudle_main.EvtSetting += Handle_Setting;
        }
        void Handle_Setting()
        {
            if (go_Setting == null)
            {
                go_Setting = Instantiate(Prefab_Setting); 
            }
            else
            {
                go_Setting.SetActive(true);
            }
            go_Setting.transform.FindChild("button/fanhui").GetComponent<tk2dUIItem>().OnClick += Back_Click; 
        }

        void Back_Click()
        {
            Moudle_main.Singlton.go_Black.SetActive(false);
            if (go_Setting == null) return;
            Destroy(go_Setting.gameObject); 
        }  
    }
}
