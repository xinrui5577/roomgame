using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View.MahStadiumListWindow
{
    /// <summary>
    /// 我的创建的请求
    /// </summary>
    public class MyCreatWindow : YxNguiWindow
    {
        public GameObject MyCreatBg;
        public MahCreatItem MyCreatItem;
        public UIGrid MyCreatGrid;
        public string MahCreatWindowName;
        public string[] OpenWindowsNames;

        private List<object> _lableShows;

        public void OnMyCreatWindow()
        {
            Facade.Instance<TwManager>().SendAction("mahjongwm.myCreate", new Dictionary<string, object>(), OnFreshData);
        }

        private void OnFreshData(object msg)
        {
            var datas = msg as IDictionary;
            if (datas==null) return;
            if(datas.Contains("state"))
            {
               var win= YxWindowManager.OpenWindow(MahCreatWindowName);
                win.UpdateView(datas);
                return;
            }
            
            if (!datas.Contains("myCreateCfg")) return;
            MyCreatBg.SetActive(true);
            while (MyCreatGrid.transform.childCount > 0)
            {
                DestroyImmediate(MyCreatGrid.transform.GetChild(0).gameObject);
            }
            var data = datas["myCreateCfg"];
            if (data is List<object>)
            {
                _lableShows = data as List<object>;
                var index = 0;
                foreach (var lableShow in _lableShows)
                {
                    if (!(lableShow is Dictionary<string, object>)) continue;
                    var item = YxWindowUtils.CreateItem(MyCreatItem, MyCreatGrid.transform);
                    var dictionary = lableShow as Dictionary<string, object>;
                    item.SetItemView(dictionary);
                    item.name = index.ToString(CultureInfo.InvariantCulture);
                    UIEventListener.Get(item.gameObject).onClick = OnRequestBtn;
                    index++;
                }
            }
            MyCreatGrid.repositionNow = true;
        }

        public void OnRequestBtn(GameObject toggle)
        {
            MyCreatBg.SetActive(!MyCreatBg.activeSelf);
            var index = int.Parse(toggle.name);
            if (_lableShows[index] == null) return;
            var win = YxWindowManager.OpenWindow(OpenWindowsNames[index]);
            if (win == null) return;
            win.UpdateView(_lableShows[index]);
        }
        public void OnCtrlBg()
        {
            MyCreatBg.SetActive(!MyCreatBg.activeSelf);
        }
    }
}
