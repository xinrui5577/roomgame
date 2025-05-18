using UnityEngine;

namespace Assets.Scripts.Game.FishGame.script
{
    public class Moudle_Help : MonoBehaviour
    {
        public GameObject Prefab_HelpUI;
        public static GameObject go_Help;

        public GameObject Prefab_BaikeUI;
        public GameObject go_Baike;

        public GameObject Prefab_Jiaocheng1;
        public GameObject go_Jiaocheng1;

        public GameObject Prefab_Jiaocheng2;
        public GameObject go_Jiaocheng2;

        public GameObject Prefab_Jiaocheng3;
        public GameObject go_Jiaocheng3;

        public GameObject Prefab_Jiaocheng4;
        public GameObject go_Jiaocheng4;

        private int createOnce = 1;
        // Use this for initialization
        void Start()
        {
            Moudle_main.EvtHelp += Handle_GameHelp;
        }
        void init_Baike()
        {
            //if (Moudle_main.EvtWikipedia != null)
            // Moudle_main.EvtWikipedia();
            go_Baike = Instantiate(Prefab_BaikeUI) as GameObject;

            go_Baike.SetActive(false);

            go_Baike.transform.FindChild("button/fanhui").GetComponent<tk2dUIItem>().OnClick += Back_BaikeClick;
        }
        void Back_BaikeClick()
        {
            go_Baike.SetActive(false);
            go_Help.SetActive(true);


        }
        void Handle_GameHelp()
        {
            if (createOnce == 1)
            {
                go_Help = Instantiate(Prefab_HelpUI) as GameObject;
                init_Baike();

                createOnce = 0;
            }
            else
            {
                go_Help.SetActive(true);
            }
            go_Help.transform.FindChild("button/fanhui").GetComponent<tk2dUIItem>().OnClick += Back_Click; 
        }
     
        void Back_Click()
        {
            switch (Moudle_main.iState)
            {
                case 0:
                    {
                        go_Help.SetActive(false);
                        if (Moudle_main.EvtBackStart != null)
                            Moudle_main.EvtBackStart();
                    }
                    break;
                case 1:
                    {
                        go_Help.SetActive(false);
                    }
                    break;
            }

            go_Help.transform.FindChild("button/fanhui").GetComponent<tk2dUIItem>().OnClick -= Back_Click;
           
            Moudle_main.Singlton.go_Black.SetActive(false);
        } 

        void next1()
        {
            go_Jiaocheng1.SetActive(false);
            if (go_Jiaocheng2 == null)
                go_Jiaocheng2 = Instantiate(Prefab_Jiaocheng2) as GameObject;
            else
                go_Jiaocheng2.SetActive(true);

            go_Jiaocheng2.transform.FindChild("next").GetComponent<tk2dUIItem>().OnClick += next2;
        }
        void next2()
        {
            go_Jiaocheng2.SetActive(false);
            if (go_Jiaocheng3 == null)
                go_Jiaocheng3 = Instantiate(Prefab_Jiaocheng3) as GameObject;
            else
                go_Jiaocheng3.SetActive(true);
            go_Jiaocheng3.transform.FindChild("next").GetComponent<tk2dUIItem>().OnClick += next3;
        }
        void next3()
        {
            go_Jiaocheng3.SetActive(false);
            if (go_Jiaocheng4 == null)
                go_Jiaocheng4 = Instantiate(Prefab_Jiaocheng4) as GameObject;
            else
                go_Jiaocheng4.SetActive(true);
            go_Jiaocheng4.transform.FindChild("next").GetComponent<tk2dUIItem>().OnClick += next4;
        }
        void next4()
        {
            switch (Moudle_main.iState)
            { 
                case 0:
                    if (Moudle_main.EvtBackStart != null)
                        Moudle_main.EvtBackStart();
                    break;
            }
            go_Jiaocheng4.SetActive(false);
            Moudle_main.Singlton.go_Black.SetActive(false);
            go_Help.transform.FindChild("button/fanhui").GetComponent<tk2dUIItem>().OnClick -= Back_Click; 
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}
