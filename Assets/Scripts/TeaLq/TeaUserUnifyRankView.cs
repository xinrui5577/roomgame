using System.Collections.Generic;
using Assets.Scripts.Common.Windows.TabPages;
using Assets.Scripts.Tea.Page;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.TeaLq
{
    public class TeaUserUnifyRankView : UserUnifyRankView
    {
        protected override void DealTabsData()
        {
            var dic = new Dictionary<string, object>();
            dic["teaId"] = TeaMainPanel.CurTeaId;
            Facade.Instance<TwManager>().SendAction("group.findTeaGame", dic, FreshTabDatas);
        }

        private void FreshTabDatas(object msg)
        {
            var lists = msg as List<object>;
            if (lists == null) return;
            var count = lists.Count;
            TabDatas = new TabData[count];
            for (var i = 0; i < count; i++)
            {
                var dic = lists[i] as Dictionary<string, object>;
                if (dic == null) return;
                var tdata = new TabData
                {
                    Name = dic["game_name"].ToString(),
                    Data = dic["game_key"].ToString()
                };
                TabDatas[i] = tdata;
            }
            if (TabDatas.Length < 1) return;
            var td = TabDatas[0];
            td.StarttingState = true;
            UpdateView();
        }
    }
}
