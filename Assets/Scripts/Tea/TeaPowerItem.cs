using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Utils;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Tea
{
    public class TeaPowerItem : YxView
    {
        public UILabel UserId;
        private string realUserId;
        public UILabel Name;
        public UILabel data;
        private string tea_id;
        public TeaPower teaPower;
        public UILabel UserCoin;

        public GameObject SendBtn;

        protected override void OnFreshView()
        {
            var dic = GetData<Dictionary<string, object>>();
            if (dic == null) return;
            if (dic.ContainsKey("nickname"))
            {
                Name.text = (string)dic["nickname"];
            }
            if (dic.ContainsKey("user_id"))
            {
                realUserId = (string) dic["user_id"];
                UserId.text = "ID:" + realUserId;
            }
            if (dic.ContainsKey("create_dt"))
            {
                data.text = (string)dic["create_dt"];
            }
            if (dic.ContainsKey("tea_id"))
            {
                tea_id = (string)dic["tea_id"];
            }
            if (dic.ContainsKey("hasSend"))
            {
               if(SendBtn!=null) SendBtn.SetActive((bool)dic["hasSend"]);
            }

            var coin = 0L;
            if (DictionaryHelper.Parse(dic, "coin", ref coin) && UserCoin != null)
            {
                UserCoin.text = string.Format("{0}{1}", "金币:", YxUtiles.ReduceNumber(coin));
            }
        }

        public void Agree()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            object obj1 = tea_id;
            dic["id"] = obj1;
            object obj2 = realUserId;
            dic["user_id"] = obj2;
            object obj3 = "1";
            dic["status"] = obj3;
            Facade.Instance<TwManager>().SendAction("group.teaHouseAudit", dic, Ok);
        }

        public void DisAgree()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            object obj1 = tea_id;
            dic["id"] = obj1;
            object obj2 = realUserId;
            dic["user_id"] = obj2;
            object obj3 = "2";
            dic["status"] = obj3;
            Facade.Instance<TwManager>().SendAction("group.teaHouseAudit", dic, Ok);
        }

        public void GoOut()
        {
            YxMessageBox.Show(
                "您是否确定请出茶馆",
                null,
                (window, btnname) =>
                {
                    switch (btnname)
                    {
                        case YxMessageBox.BtnLeft:
                            Dictionary<string, object> dic = new Dictionary<string, object>();
                            object obj1 = tea_id;
                            dic["id"] = obj1;
                            object obj2 = realUserId;
                            dic["user_id"] = obj2;
                            object obj3 = "2";
                            dic["status"] = obj3;
                            Facade.Instance<TwManager>().SendAction("group.teaHouseAudit", dic, GoOutOk);
                            break;
                    }
                },
                true,
                YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
            );
        }

        private void GoOutOk(object msg)
        {
            TeaUtil.GetBackString(msg);
            teaPower.GetMemberList();
        }

        private void Ok(object msg)
        {
            TeaUtil.GetBackString(msg);
            teaPower.getApplyList();
        }

        public void OnSendBtn()
        {
            if (MainYxView)
            {
                YxWindow obj = MainYxView.GetComponent<YxNguiWindow>().CreateChildWindow("TeaMoneyWindow");
                TeaMoneyWindow infoWindow = obj.GetComponent<TeaMoneyWindow>();
                infoWindow.SendIdInput.value = realUserId;
                infoWindow.TeaId = int.Parse(tea_id);
            }
        }
    }

}
