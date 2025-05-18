using System.Collections.Generic;
using Assets.Scripts.Common.Windows.TabPages;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.ShopWindows
{
    public class YxShopWindow : YxTabPageWindow
    {
        public const string KeyStore = "store"; 
        protected override void ActionCallBack()
        {
            base.ActionCallBack();
            var dict = Data as Dictionary<string, object>;
            if (dict == null) { return;}
            if (dict.ContainsKey(KeyStore))
            {
                var storeDict = dict[KeyStore] as Dictionary<string, object>;
                if (storeDict != null)
                {
                    var list = new List<TabData>();
                    foreach (var keyValue in storeDict)
                    {
                        var value = keyValue.Value as Dictionary<string, object>;
                        if (value == null) { continue; }
                        var id = value["id"].ToString();
                        var tdata = new TabData
                        {
                            Id = id,
                            Name = value["tab_name"].ToString(),
                            UpStateName = id,
                            DownStateName = id
                        };
                        if (ViewsDictionary.ContainsKey(id))
                        {
                            tdata.View = ViewsDictionary[id];
                        }else if (ViewsDictionary.ContainsKey("0"))
                        {
                            tdata.View = ViewsDictionary["0"];
                        }
                        list.Add(tdata);
                    }
                    UpdateTabs(list);
                }
            } 
        }

        protected override void OnChageTab(YxView view, TabData tabData)
        {
            if (view == null) { return; }
            var parm = new Dictionary<string, object>();
            parm["type"] = tabData.Id;
            CurTwManager.SendAction(TabActionName, parm, view.UpdateView);
        }
    }
}
