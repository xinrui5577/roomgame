using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View.MahStadiumListWindow
{
    public class MahCreatWindow : YxNguiWindow
    {
        private List<object> _datas=new List<object>();

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var datas = Data as IDictionary;
            if (datas != null)
            {
                var data = datas["myCreateCfg"];
                if (data is List<object>)
                {
                    _datas = data as List<object>;
                }
            }
        }

        public void OnClick(string objName)
        {
            var infos = objName.Split('|');
            var win= YxWindowManager.OpenWindow(infos[0]);
            win.UpdateView(_datas[int.Parse(infos[1])]);
        }
    }
}
