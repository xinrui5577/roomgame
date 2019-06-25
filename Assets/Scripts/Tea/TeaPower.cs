using System.Collections.Generic;
using System.Security.Permissions;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Tea
{
    public class TeaPower : YxNguiWindow
    {

        public GameObject DangQianUp;
        public GameObject DangQianDown;
        public GameObject YiJingUp;
        public GameObject YiJingDown;
        public GameObject InfoItem;
        public GameObject InfoItemHasIn;
        public UIGrid grid;

        public bool DangQianUpBool = true;

        public string TeaId="";

        void Start()
        {
            getApplyList();
        }

        public void getApplyList()
        {
            if (TeaId == "")
            {
                return;
            }
            grid.transform.DestroyChildren();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            object obj1 = TeaId;
            dic["id"] = obj1;
            Facade.Instance<TwManger>().SendAction("group.teaApplyList", dic, BackList);
        }

        private void BackList(object msg)
        {
            Dictionary<string, object> dic = (Dictionary<string, object>)msg;
            object obj = dic["applyList"];
            List<object> list = (List<object>)obj;
            foreach (object o in list)
            {
                Dictionary<string, object> dic2 = (Dictionary<string, object>)o;
                GameObject item=Instantiate(InfoItem);
                item.transform.parent = grid.transform;
                item.SetActive(true);
                item.transform.localScale=Vector3.one;
                TeaPowerItem powerItem = item.GetComponent<TeaPowerItem>();
                powerItem.UpdateView(dic2);
            }
            grid.Reposition();
        }

        public void DangQianClick()
        {
            getApplyList();
            DangQianUp.SetActive(true);
            DangQianDown.SetActive(false);
            YiJingUp.SetActive(false);
            YiJingDown.SetActive(true);
        }

        public void YiJingClick()
        {
            GetMemberList();
            DangQianUp.SetActive(false);
            DangQianDown.SetActive(true);
            YiJingUp.SetActive(true);
            YiJingDown.SetActive(false);
        }

        public void GetMemberList()
        {
            if (TeaId=="")
            {
                return;
            }
            grid.transform.DestroyChildren();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            object obj1 = TeaId;
            dic["id"] = obj1;
            Facade.Instance<TwManger>().SendAction("group.teaHouseMember", dic, BackMemberList);
        }

        private void BackMemberList(object msg)
        {
            Dictionary<string, object> dic = (Dictionary<string, object>)msg;
            object obj = dic["memberList"];
            List<object> list = (List<object>)obj;
            foreach (object o in list)
            {
                Dictionary<string, object> dic2 = (Dictionary<string, object>)o;
                GameObject item = Instantiate(InfoItemHasIn);
                item.transform.parent = grid.transform;
                item.SetActive(true);
                item.transform.localScale = Vector3.one;
                TeaPowerItem powerItem = item.GetComponent<TeaPowerItem>();
                powerItem.UpdateView(dic2);
            }
            grid.Reposition();
        }
    }
}
