using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 发起喇叭
    /// </summary>
    public class TrumpetWindow : YxNguiWindow
    {
        [Tooltip("喊话内容")]
        public UIInput InputLabel;
        [Tooltip("喇叭剩余数Label")]
        public UILabel SurplusLabel;
        [Tooltip("喇叭剩余数内容格式")]
        public string SurplusTip = "您还剩余 [ffff00]{0}[-] 个喇叭";
        [Tooltip("喇叭id")]
        public string TrumpetId = "item4_q";
        protected override void OnStart()
        {
            UpdateTrumpet();
        }

        private void UpdateTrumpet()
        {
            if (SurplusLabel == null) return;
            var bp = UserInfoModel.Instance.BackPack;
            var count = bp.GetItem(TrumpetId);
            SurplusLabel.text = string.Format(SurplusTip, count);
        }

        public void OnSendMessage()
        {
            var msg = InputLabel.value;
            if (string.IsNullOrEmpty(msg))
            {
                YxMessageBox.Show("发送的消息不能为空！");
                return;
            }
            var backPack = UserInfoModel.Instance.BackPack;
            var count = backPack.GetItem(TrumpetId);
            if (count < 1)
            {
                YxMessageBox.Show("喇叭个数不够，请到商城购买！");
                return;
            }
            YxMessageBox.Show("发送消息会消耗一个喇叭，确定使用吗？", "", (box, btnName) =>
                {
                    if (btnName == YxMessageBox.BtnLeft)
                    {
                        var parm = new Dictionary<string, object>();
                        parm["msg"] = msg;
                        Facade.Instance<TwManager>().SendAction("addHornNotice", parm, msgObj =>
                            {
                                var msgDict = msgObj as Dictionary<string, object>;
                                if (msgDict == null) { return;}
                                if (!msgDict.ContainsKey("info")) return;
                                var msgTemp = msgDict["info"];
                                if (msgTemp != null) YxMessageBox.Show(msgTemp.ToString());
                            }); 
                    }
                    backPack.DeleteItem(TrumpetId, 1);
                    UpdateTrumpet();
                    Close();
                }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
        }
    }
}                 