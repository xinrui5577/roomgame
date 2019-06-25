using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
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
        public UIInput InputLabel;
        public UILabel SurplusLabel;
        public string SurplusTip = "您还剩余 [ffff00]{0}[-] 个喇叭";

        protected override void OnStart()
        {
            UpdateTrumpet();
        }

        private void UpdateTrumpet()
        {
            if (SurplusLabel == null) return;
            var bp = UserInfoModel.Instance.BackPack;
            var count = bp.GetItem("item4_q");
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
            var count = backPack.GetItem("item4_q");
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
                        Facade.Instance<TwManger>().SendAction("addHornNotice", parm, msgObj =>
                            {
                                if (!(msgObj is Dictionary<string, object>)) return;
                                var msgDict = msgObj as Dictionary<string, object>;
                                if (!msgDict.ContainsKey("info")) return;
                                var msgTemp = msgDict["info"];
                                if (msgTemp != null) YxMessageBox.Show(msgTemp.ToString());
                            }); 
                    }
                    backPack.DeleteItem("item4_q",1);
                    UpdateTrumpet();
                    Close();
                }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
        }
    }
}                 