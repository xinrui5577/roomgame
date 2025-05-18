using UnityEngine;

namespace Assets.Scripts.Game.FishGame.script
{
    public class Moudle_MainStart : MonoBehaviour
    {
        public GameObject Prefab_startUI;
        public GameObject go_start;

        public GameObject Prefab_Quit;
        public GameObject go_Quit;

        // Use this for initialization
        void Awake()
        {
            //Moudle_main.Singlton.go_Black.SetActive(false);
        }
        void Start()
        {
            go_start = Instantiate(Prefab_startUI) as GameObject; 
            go_start.transform.FindChild("buttoms/function").GetComponent<tk2dUIItem>().OnClick += Help_Click; 

            Moudle_main.EvtBackStart += Handle_GameBackStart;
            if (Moudle_main.EvtBackStart!=null)
                Moudle_main.EvtBackStart();

            go_Quit = Instantiate(Prefab_Quit) as GameObject;
            go_Quit.SetActive(false);

            go_Quit.transform.FindChild("yes").GetComponent<tk2dUIItem>().OnClick += YES_Click;
            go_Quit.transform.FindChild("no").GetComponent<tk2dUIItem>().OnClick += NO_Click;
            // go_start.transform.localScale = new Vector3(0.1f, 1, 1);
        }
        void Handle_GameBackStart()
        {
            switch (Moudle_main.iState)
            {
                case 0:
                    {
                        go_start.SetActive(true);
                    }
                    break;
            }
        }
        void Help_Click()
        { 
            Moudle_main.Singlton.go_Black.SetActive(true);
            switch (Moudle_main.iState)
            {
                case 0:
                    {
                        go_start.SetActive(false);
                    }
                    break;
            }
            if (Moudle_main.EvtHelp != null)
                Moudle_main.EvtHelp();
        }
      
        void OnDisable()
        {

        }
        void OnEnable()
        {

        }
        void start_Click()
        {
            switch (Moudle_main.iState)
            {
                case 0:
                    {
                        go_start.SetActive(false);
                        Moudle_main.iState = 1;
                    }
                    break;
            }
            if (Moudle_main.EvtSceneSelect != null)
                Moudle_main.EvtSceneSelect();
        }

        void YES_Click()
        {
            Application.Quit();
        }

        void NO_Click()
        {
            go_Quit.SetActive(false);
            if (Moudle_main.iState==0)
            {
                go_start.SetActive(true);
            }
            Moudle_main.Singlton.go_Black.SetActive(false);
        }
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Application.Quit();
                Moudle_main.Singlton.go_Black.SetActive(true);
                go_start.SetActive(false);
                go_Quit.SetActive(true);
            }
        }
    }
}
