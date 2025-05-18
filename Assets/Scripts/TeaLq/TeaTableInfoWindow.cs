using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.TeaLq
{
    public class TeaTableInfoWindow : YxNguiWindow
    {
        public UIGrid TeaTableInfoGrid;
        public TeaTableInfoItem TeaTableInfoItem;

        private Dictionary<string, object> _dic;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var dic = Data as Dictionary<string, object>;
            if (dic == null) return;
            _dic = dic;
            Facade.Instance<TwManager>().SendAction("group.getTableUserList", _dic, FreshTableUserList, true, null, false);

        }

        private void FreshTableUserList(object obj)
        {
            var datas = obj as List<object>;
            if (datas == null) return;
            foreach (var data in datas)
            {
                var info = data as Dictionary<string, object>;
                if (info == null) return;
                var teaTableInfo = new TeaTableInfoData(info);
                var item = YxWindowUtils.CreateItem(TeaTableInfoItem, TeaTableInfoGrid.transform);
                item.UpdateView(teaTableInfo);
            }
            TeaTableInfoGrid.repositionNow = true;
        }

        public void OnDeleteRoom()
        {
            var dic = new Dictionary<string, object>();
            dic["teaId"] = _dic["teaId"].ToString();
            dic["roomId"] = _dic["roomId"].ToString();
            Facade.Instance<TwManager>().SendAction("group.deleteRoom", dic, FreshPanelShow, true, null, false);
        }

        private void FreshPanelShow(object msg)
        {
            var dic = msg as Dictionary<string, object>;
            if (dic == null) return;
            var info = dic["info"].ToString();
            var status = dic["status"].ToString();
            YxMessageBox.Show(info);
            if (status.Equals("success"))
            {
                Close();
            }
        }
    }

    public class TeaTableInfoData
    {
        public int Sex;
        public string Avatar;
        public string NickName;
        public int UserId;
        public string UserIp;

        public TeaTableInfoData(Dictionary<string, object> dic)
        {
            dic.Parse("sex_i", ref Sex);
            dic.Parse("avatar_x", ref Avatar);
            dic.Parse("nick_m", ref NickName);
            dic.Parse("user_id", ref UserId);
            dic.Parse("last_login_ip", ref UserIp);
        }
    }
}
