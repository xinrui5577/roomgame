using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View
{
    public class WithdrawWindow : YxNguiWindow
    {
        public string ActionName;
        public string ParamKey;
        public string ParamValue;
        public UIGrid TabGrid;
        public List<UIToggle> ChildTabs;

        protected override void OnStart()
        {
            base.OnStart();
            var dic = new Dictionary<string, object>();
            dic[ParamKey] = ParamValue;
            Facade.Instance<TwManager>().SendAction(ActionName, dic, UpdateView);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var dataDic = Data as Dictionary<string, object>;
            if (dataDic == null) return;
            if (dataDic.ContainsKey("data"))
            {
                var data = dataDic["data"] as Dictionary<string, object>;
                if (data == null) return;

                for (int i = 0; i < ChildTabs.Count; i++)
                {
                    if (data.ContainsKey(ChildTabs[i].name))
                    {
                        ChildTabs[i].startsActive = true;
                        ChildTabs[i].SetActive(true);
                    }
                }
            }

            TabGrid.repositionNow = true;
        }
    }
}
