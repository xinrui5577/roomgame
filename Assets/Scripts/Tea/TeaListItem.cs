using System;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Utils;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Tea
{
    /// <summary>
    /// 茶馆的Item 显示单个茶馆的简单信息
    /// </summary>
    public class TeaListItem : YxView
    {

        public UILabel OrderNumber;

        public UILabel TeaName;

        public UILabel TeaId;

        public UILabel TeaSign;
        /// <summary>
        /// 当前茶馆功能的命名 可以叫俱乐部 亲友圈 自己填写
        /// </summary>
        public string CurrentName= "茶馆";

        private int _currentRoomId;
        private Dictionary<string, object> _teaInfo;

        protected override void OnFreshView()
        {
            _teaInfo = Data as Dictionary<string, object>;
            if (_teaInfo != null)
            {
                var teaId = _teaInfo.ContainsKey("tea_id") ? _teaInfo["tea_id"] : null;
                var teaName = _teaInfo.ContainsKey("tea_name") ? _teaInfo["tea_name"] : null;
                var groupSign = _teaInfo.ContainsKey("group_sign") ? _teaInfo["group_sign"] : null;

                OrderNumber.text = Id;
                if (teaId != null && TeaId)
                {
                    name = teaId.ToString();
                    TeaId.text= teaId.ToString();
                }
                if (teaName != null&& TeaName) TeaName.text = teaName.ToString();
                if (groupSign != null&& TeaSign) TeaSign.text = groupSign.ToString();
            }
        }

        public void FindRoom(GameObject obj)
        {
            _currentRoomId =int.Parse(obj.name) ;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["id"] = _currentRoomId;
            Facade.Instance<TwManager>().SendAction("group.teaGetIn", dic, GetInTea);
        }

        public void ShowSingleTeaView()
        {
            var win = CreateOtherWindow("TeaSingleInfoWindow");
            win.UpdateView(_teaInfo);
        }

        private void GetInTea(object msg)
        {
            Dictionary<string, object> dic = (Dictionary<string, object>)msg;
            long value = (long)dic["mstatus"];
            if (value != 4)
            {
                YxWindow obj = CreateOtherWindow("TeaPanel");
                TeaPanel panel = obj.GetComponent<TeaPanel>();
                panel.UpdateView(dic);
                panel.SetTeaCode(_currentRoomId);
                Util.SetString("TeaId", _currentRoomId.ToString());
            }
            else
            {
                YxMessageBox.Show(string.Format("{0}不存在", CurrentName));
            }
            
        }
    }
}
