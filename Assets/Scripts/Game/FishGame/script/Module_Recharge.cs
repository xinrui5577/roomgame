using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.FishGame.script
{
    public class Module_Recharge : MonoBehaviour {

        public GameObject Prefab_Recharge;
        public GameObject go_Recharge;

        float ScaleX=1;
        float ScaleY=1;

        public int select = 0;

        private float startTime = 0;

        int create = 1;
        // Use this for initialization
        void Start () {
            Moudle_main.EvtRecharge += Handle_GameRecharge;
        }
        void Handle_GameRecharge()
        {
            if (create == 1)
            {
                go_Recharge = Instantiate(Prefab_Recharge) as GameObject;
                create = 0;
            }
            else
            {
                go_Recharge.SetActive(true);
            }

            go_Recharge.transform.FindChild("fanhui").GetComponent<tk2dUIItem>().OnClick += Back_Click;
            go_Recharge.transform.FindChild("确定").GetComponent<tk2dUIItem>().OnClick += OK_Click;

            go_Recharge.transform.FindChild("话费充值").GetComponent<tk2dUIItem>().OnClick += Huafei_Recharge;
            go_Recharge.transform.FindChild("信用卡充值").GetComponent<tk2dUIItem>().OnClick += Xinyongka_Recharge;
            go_Recharge.transform.FindChild("支付宝").GetComponent<tk2dUIItem>().OnClick += Zhifubao_Recharge;
            go_Recharge.transform.FindChild("其他支付").GetComponent<tk2dUIItem>().OnClick += Qita_Recharge;
        }
        void reset()
        {
            ScaleX = ScaleY = 1;
            go_Recharge.transform.FindChild("话费充值").GetComponent<Transform>().localScale = new Vector3(ScaleX, ScaleY, 0);
            go_Recharge.transform.FindChild("信用卡充值").GetComponent<Transform>().localScale = new Vector3(ScaleX, ScaleY, 0);
            go_Recharge.transform.FindChild("支付宝").GetComponent<Transform>().localScale = new Vector3(ScaleX, ScaleY, 0);
            go_Recharge.transform.FindChild("其他支付").GetComponent<Transform>().localScale = new Vector3(ScaleX, ScaleY, 0);
        }
        IEnumerator change(string son)
        {
            reset();
            //  YxDebug.Log(go_Recharge.transform.FindChild(son).GetComponent<Transform>().localScale.x);
            // YxDebug.Log(go_Recharge.transform.FindChild(son).GetComponent<Transform>().localScale.y);

            Transform tr=go_Recharge.transform.FindChild(son).GetComponent<Transform>();
        
            while (startTime + 0.2f > Time.time)
            {
                tr.localScale=new Vector3(ScaleX,ScaleY,0);
                ScaleX += 0.2f / (0.2f / Time.deltaTime);
                ScaleY += 0.2f / (0.2f / Time.deltaTime);
                yield return 0;
            }
            tr.localScale = new Vector3(1.2f, 1.2f, 0);
        }
        void Huafei_Recharge()
        {
            startTime = Time.time;
            select = 1;
            StopCoroutine("change");
        
            StartCoroutine("change","话费充值");
        }
        void Xinyongka_Recharge()
        {
            startTime = Time.time;
            select = 2;
            StopCoroutine("change");
            StartCoroutine("change","信用卡充值");
        }
        void Zhifubao_Recharge()
        {
            startTime = Time.time;
            select = 3;
            StopCoroutine("change");
            StartCoroutine("change","支付宝");
        }
        void Qita_Recharge()
        {
            startTime = Time.time;
            select = 4;
            StopCoroutine("change");
            StartCoroutine("change", "其他支付");
        }
        void Back_Click()
        {
            reset();
            Moudle_main.Singlton.go_Black.SetActive(false);
            go_Recharge.SetActive(false);
            select = 0; 
        }
        void OK_Click()
        {
            reset();
            if (select != 0)
            {
                go_Recharge.SetActive(false);
                select = 0;
                if (Moudle_main.EvtShop != null)
                    Moudle_main.EvtShop();
            }
            //chongzhi_select.get
        }
        // Update is called once per frame
        void Update () {
	
        }
    }
}
