using UnityEngine;
using Assets.Scripts.Game.jh.EventII;
using Sfs2X.Entities.Data;
using System.Collections.Generic;
using Assets.Scripts.Game.jh.Public;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhHupUpView : MonoBehaviour
    {
        public GameObject View;

        public UILabel TotalTime;
        public UILabel LeaveTime;
        public UILabel PlayerName;

        public UIGrid Grid;

        public GameObject Prefce;

        public EventObject EventObj;

        public List<JhHupUpItem> ItemList;

        public GameObject Btns;


        protected bool StartTimer;
        protected long StartTime;
        protected int HupTime;
        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "HupUp":
                    OnHupUp(data.Data);
                    break;
                case "Close":
                    OnClose();
                    break;
            }
        }

        protected void OnHupUp(object data)
        {
            View.SetActive(true);

            ISFSObject obj = (ISFSObject)data;
            ISFSArray arr = obj.GetSFSArray("Users");
            string hupName = "";

            for(int i = 0;i<arr.Size();i++){

                ISFSObject arrObj = arr.GetSFSObject(i);
                int status = arrObj.GetInt("Status");

                JhHupUpItem item;
                if(ItemList.Count>i){
                    item = ItemList[i];
                    item.SetIcon(status);
                }else{
                    GameObject gobj = Instantiate(Prefce);
                    gobj.SetActive(true);
                    Grid.AddChild(gobj.transform);
                    gobj.transform.localScale = Vector3.one;
                    item = gobj.GetComponent<JhHupUpItem>();
                    
                    string uname = arrObj.GetUtfString("Name");
                    int sex = arrObj.GetInt("Sex");
                    string head = arrObj.GetUtfString("Head");
                    item.SetInfo(head, uname, sex, status);
                    ItemList.Add(item);
                }

                if (status == 2)
                {
                    hupName = arrObj.GetUtfString("Name");
                }
            }


            if (obj.ContainsKey("HupTime"))
            {
                StartTime = obj.GetLong("StartTime");
                HupTime = obj.GetInt("HupTime");

                TotalTime.text = string.Format("(超过{0}秒未做选着，则默认同意。)", HupTime);

                LeaveTime.text = "" + (HupTime - (JhFunc.GetTimeStamp() - StartTime));

                PlayerName.text = string.Format("玩家 {0} 申请解散房间,请等待其他玩家选着。", hupName);
            }

            bool isShowBtn = !obj.GetBool("SelfChoose");
            Btns.SetActive(isShowBtn);

            if (!StartTimer)
            {
                StartTimer = true; 
            }

        }

        void Update()
        {
            if (StartTimer)
            {
                long time = (HupTime - (JhFunc.GetTimeStamp() - StartTime));
                if(time<=0){
                    time = 0;
                    StartTimer = false;

                }
                LeaveTime.text = "" + time;
            }
        }

        protected void OnClose()
        {
            StartTimer = false;
            View.SetActive(false);

            foreach (var cItem in ItemList)
            {
                Destroy(cItem.gameObject);
            }
            ItemList.Clear();

            Grid.repositionNow = true;
        }

        public void OnTongYi()
        {
            Btns.SetActive(false);
            EventObj.SendEvent("ServerEvent","HupReq",3);
        }

        public void OnJuJue()
        {
            Btns.SetActive(false);
            EventObj.SendEvent("ServerEvent", "HupReq", -1);
        }
    }
}

