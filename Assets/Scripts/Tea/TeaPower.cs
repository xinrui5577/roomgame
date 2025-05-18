using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

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

        public GameObject TeaChangeUp;
        public GameObject TeaChangeDown;

        public UIInput TeaNameInput;
        public UIInput WeChatInput;
        public UIInput SignInput;
        public GameObject TeaChange;

        public UIScrollView ScrollView;
        public UIGrid grid;

        public bool DangQianUpBool = true;

        public string TeaId="";

        public bool HasSend = false;

        public UILabel TeaMemberNum;

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
            Facade.Instance<TwManager>().SendAction("group.teaApplyList", dic, BackList);
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
            if (ScrollView != null)
            {
                ScrollView.ResetPosition();
            }
        }

        public void DangQianClick()
        {
            getApplyList();
            if (TeaChange != null) TeaChange.SetActive(false);
            DangQianUp.SetActive(true);
            DangQianDown.SetActive(false);
            YiJingUp.SetActive(false);
            YiJingDown.SetActive(true);
            if (TeaMemberNum!=null) TeaMemberNum.gameObject.SetActive(false);
            if (TeaChangeUp != null) TeaChangeUp.SetActive(false);
            if (TeaChangeDown != null) TeaChangeDown.SetActive(true);
        }

        public void YiJingClick()
        {
            GetMemberList();
            if(TeaChange!=null) TeaChange.SetActive(false);
            DangQianUp.SetActive(false);
            DangQianDown.SetActive(true);
            YiJingUp.SetActive(true);
            YiJingDown.SetActive(false);
            if(TeaChangeUp!=null) TeaChangeUp.SetActive(false);
            if(TeaChangeDown!=null) TeaChangeDown.SetActive(true);
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
            Facade.Instance<TwManager>().SendAction("group.teaHouseMember", dic, BackMemberList);
        }
       
        private void BackMemberList(object msg)
        {
            Dictionary<string, object> dic = (Dictionary<string, object>)msg;
            object obj = dic["memberList"];
            List<object> list = (List<object>)obj;
            if (TeaMemberNum)
            {
                TeaMemberNum.gameObject.SetActive(true);
                var str = string.Format("茶馆人数：{0}", list.Count);
                TeaMemberNum.TrySetComponentValue(str);
            }
            foreach (object o in list)
            {
                Dictionary<string, object> dic2 = (Dictionary<string, object>)o;
                dic2["hasSend"] = HasSend;
                GameObject item = Instantiate(InfoItemHasIn);
                item.transform.parent = grid.transform;
                item.SetActive(true);
                item.transform.localScale = Vector3.one;
                TeaPowerItem powerItem = item.GetComponent<TeaPowerItem>();
                powerItem.UpdateView(dic2);
            }
            grid.Reposition();
            if (ScrollView != null)
            {
                ScrollView.ResetPosition();
            }
        }

        public void OnTeaChange()
        {
            if (TeaId == "") return;
            if (_teaName.Equals(TeaNameInput.value) && _weChat.Equals(WeChatInput.value) &&
                _groupSign.Equals(SignInput.value))
            {
                YxMessageBox.Show("您没有任何修改！！！");
                return;
            }
            var dic = new Dictionary<string, object>();
            dic["tea_name"] = TeaNameInput.value;
            dic["wechat"] = WeChatInput.value;
            dic["group_sign"] = SignInput.value;
            dic["tea_id"] = TeaId;
            Facade.Instance<TwManager>().SendAction("group.teaChange", dic,null);
        }
        private string _teaName;
        private string _weChat;
        private string _groupSign;

        public void OnRequestTeaData()
        {
            if (TeaChange != null) TeaChange.SetActive(true);
            DangQianUp.SetActive(false);
            DangQianDown.SetActive(true);
            YiJingUp.SetActive(false);
            YiJingDown.SetActive(true);
            if (TeaChangeUp != null) TeaChangeUp.SetActive(true);
            if (TeaChangeDown != null) TeaChangeDown.SetActive(false);

            grid.transform.DestroyChildren();
            if (TeaId == "")return;
            var dic = new Dictionary<string, object>();
            dic["tea_id"] = int.Parse(TeaId);
            Facade.Instance<TwManager>().SendAction("group.requestTeaInfo",dic, data =>
            {
                var dataInfo = data as Dictionary<string, object>;
                if (dataInfo== null)return;
                _teaName = dataInfo.ContainsKey("tea_name")? dataInfo["tea_name"].ToString():"";
                _weChat = dataInfo.ContainsKey("wechat") ? dataInfo["wechat"].ToString() : "";
                 var groupSign= dataInfo.ContainsKey("group_sign") ? dataInfo["group_sign"] : null;
                _groupSign = groupSign!=null ? groupSign.ToString() : "";
                TeaNameInput.value = _teaName;
                WeChatInput.value = _weChat;
                SignInput.value = _groupSign;
            });
        }
    }
}
