using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 推广码窗口
    /// </summary>
    public class SpreadWindow : YxNguiWindow
    {
        /// <summary>
        /// 
        /// </summary>
        public UIInput SpreadInput;

        public void OnOkClick()
        {
            var spread = SpreadInput.value;
//            if (string.IsNullOrEmpty(spread))
//            {
//                YxMessageBox.Show("请输入推广码，否则将无法进入！");
//                return;
//            }
            var parm = new Dictionary<string, object>
                {
                    {"AffilateID",spread}
                };
            Facade.Instance<TwManger>().SendAction("getAffilateRelationAward", parm, OnSuccess, true, OnFail);
        }

        private void OnSuccess(object msg)
        {
            if (!(msg is Dictionary<string, object>)) return;
            var dict = msg as Dictionary<string, object>;
            YxDebug.Log(msg, "spreadwindow");
            if (!dict.ContainsKey("p")) return;
            bool isPass;
            bool.TryParse(dict["p"].ToString(), out isPass);
            UserInfoModel.Instance.UserInfo.Promoter = isPass;
            UserInfoModel.Instance.Save();
            if (isPass) Close();
            if (!dict.ContainsKey("info")) return;
            var info = dict["info"].ToString();
            YxMessageBox.Show(info);
        }

        private void OnFail(object obj)
        {
            var tmp = (Dictionary<string, Object>)obj;
            var msg = string.Format("网络异常:{0}", tmp["errorMessage"]);
            YxMessageBox.Show(msg);
        }
    }
}
