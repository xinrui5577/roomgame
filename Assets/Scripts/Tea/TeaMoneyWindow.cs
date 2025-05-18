using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Tea
{
    /// <summary>
    /// 茶馆群积分的变化
    /// </summary>
    public class TeaMoneyWindow : YxNguiWindow
    {
        public UIInput SendIdInput;

        public UIInput SendCountInput;

        public int TeaId;

        public int TeaState;
        public int TeaOwnerId;

        void Start()
        {
            if (TeaState == 2)
            {
                SendIdInput.value = TeaOwnerId.ToString();
            }
        }
        /// <summary>
        /// 赠送接口
        /// </summary>
        public void TeaSendMoney()
        {
            var dic = new Dictionary<string, object>();
            dic["tea_id"] = TeaId;
            if (SendCountInput.value.Equals(""))
            {
                YxMessageBox.Show("赠送人的钱数不能为空！！！");
                return;
            }

            var excrate = UserInfoModel.Instance.UserInfo.Excrate;
            dic["send_coin"] = float.Parse(SendCountInput.value) * excrate;
            if (SendIdInput.value.Equals(""))
            {
                YxMessageBox.Show("被赠送人的账号不能为空！！！");
                return;
            }
            dic["login_m"] = SendIdInput.value;
            Facade.Instance<TwManager>().SendAction("group.teaSendMoney", dic, (data) =>
            {
                Facade.EventCenter.DispatchEvent("teaSendMoney", data);
                Close();
            });
        }
    }
}
