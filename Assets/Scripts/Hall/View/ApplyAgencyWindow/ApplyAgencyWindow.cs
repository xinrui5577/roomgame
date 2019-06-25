using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

public class ApplyAgencyWindow : YxNguiWindow
{
    public UIInput QqInput;

    public UIInput WeiXinInput;

    public UIInput PhoneInput;

    public UIInput SelfInput;

    public void CreatAgency()
    {
        var dic = new Dictionary<string, object>();
        if (QqInput.value.Length == 0 || WeiXinInput.value.Length == 0 || PhoneInput.value.Length == 0)
        {
            //为系统提示窗口中显示的文字赋值
            YxMessageBoxData mesdata = new YxMessageBoxData { Msg = "某些地方为空,请重新输入" };
            //弹出系统提示窗口
            YxMessageBox.Show(mesdata);
            return;
        }
        dic.Add("qq", QqInput.value);
        dic.Add("weixin", WeiXinInput.value);
        dic.Add("phone", PhoneInput.value);
        dic.Add("self", SelfInput.value);
        //给服务器发送数据,接受服务器返回的数据并做处理
        Facade.Instance<TwManger>().SendAction("creatAgency", dic, (data) =>
        {
            if (data == null) return;
            if (data is Dictionary<string, object>)
            {
                var dataInfo = data as Dictionary<string, object>;

                var message = dataInfo.ContainsKey("mssage") ? dataInfo["mssage"].ToString() : "";

                YxMessageBoxData mesdata = new YxMessageBoxData { Msg = message };

                YxMessageBox.Show(mesdata);
            }
        });

    }
}
