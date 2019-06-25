using System.Collections.Generic;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
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
            if (dic.ContainsKey("last_update_dt"))
            {
                data.text = (string)dic["last_update_dt"];
            }
            if (dic.ContainsKey("tea_id"))
            {
                tea_id = (string)dic["tea_id"];
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
            Facade.Instance<TwManger>().SendAction("group.teaHouseAudit", dic, Ok);
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
            Facade.Instance<TwManger>().SendAction("group.teaHouseAudit", dic, Ok);
        }

        public void GoOut()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            object obj1 = tea_id;
            dic["id"] = obj1;
            object obj2 = realUserId;
            dic["user_id"] = obj2;
            object obj3 = "2";
            dic["status"] = obj3;
            Facade.Instance<TwManger>().SendAction("group.teaHouseAudit", dic, GoOutOk);
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

    }

}
