using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.TeaLq
{
    public class TeaSettingWindow : YxNguiWindow
    {
        public UIInput TeaName;
        public UIInput GroupSign;
        public List<UIToggle> Toggles;

        protected override void OnStart()
        {
            base.OnStart();
            var dic = new Dictionary<string, object>();
            dic["tea_id"] = TeaMainPanel.CurTeaId;
            Facade.Instance<TwManager>().SendAction("group.requestTeaInfo", dic, UpdateView);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var dic = Data as Dictionary<string, object>;
            if (dic == null) return;
            var teaName = dic["tea_name"].ToString();
            TeaName.TrySetComponentValue(teaName);
            var groupSign = dic["group_sign"].ToString();
            GroupSign.TrySetComponentValue(groupSign);
            var teaLimit = dic["tea_lim"].ToString();
            foreach (var toggle in Toggles)
            {
                if (toggle.name.Equals(teaLimit))
                {
                    toggle.startsActive = true;
                    toggle.value = true;
                }
            }
        }

        public void OnChangeTea()
        {
            var dic = new Dictionary<string, object>();
            dic["tea_id"] = TeaMainPanel.CurTeaId;
            dic["tea_name"] = TeaName.value;
            dic["group_sign"] = GroupSign.value;

            foreach (var toggle in Toggles)
            {
                if (toggle.value)
                {
                    dic["tea_lim"] = toggle.name;
                }
            }
            Facade.Instance<TwManager>().SendAction("group.teaChange", dic, FreshView);
        }

        private void FreshView(object obj)
        {

        }
    }
}
