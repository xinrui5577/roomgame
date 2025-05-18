using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common.DataBundles;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketGrabWindow : YxNguiRedPacketWindow
    {
        public NguiTextureAdapter Head;
        public NguiLabelAdapter NickName;
        public GameObject CanGrabTip;
        public GameObject GrabRedPacketBtn;
        public GameObject NoCanGrabTip;
        public GameObject LookHistoryBtn;
        public GameObject HasGrabTip;
        public GameObject NoGrabSelfTip;

        public string OpenWindowName;

        private int _redId;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var redPacketStateData = Data as RedPacketStateData;
            if (redPacketStateData == null) return;

            _redId = redPacketStateData.RedId;
            PortraitDb.SetPortrait(redPacketStateData.Avatar, Head, 1);
            var str = string.Format("{0}的红包", redPacketStateData.NickName);
            NickName.TrySetComponentValue(str);

            if (redPacketStateData.Status == 2|| redPacketStateData.Status == 3)
            {
                NoCanGrabTip.SetActive(true);
                LookHistoryBtn.SetActive(true);
            }

            if (redPacketStateData.Status == 1)
            {
                CanGrabTip.SetActive(true);
                GrabRedPacketBtn.SetActive(true);
            }
        }

        public void OnLookHistory()
        {
            var win = YxWindowManager.OpenWindow(OpenWindowName);
            win.UpdateView(_redId);
        }

        public void OnGrabRedPacket()
        {
            var dic = new Dictionary<string, object>();
            dic["redId"] = _redId;
            Facade.Instance<TwManager>().SendAction("RedEnvelope.grabRedEnvelope", dic, FreshCurView);
        }

        private void FreshCurView(object obj)
        {
            var data = obj as Dictionary<string, object>;
            if (data == null) return;
            var status = int.Parse(data["status"].ToString());
            if (status == 0)
            {
                CanGrabTip.SetActive(false);
                GrabRedPacketBtn.SetActive(false);
                NoCanGrabTip.SetActive(true);
                LookHistoryBtn.SetActive(true);
            }
            else if (status == 1)
            {
                OnLookHistory();
                Close();
            }
            else if (status == 2)
            {
                CanGrabTip.SetActive(false);
                GrabRedPacketBtn.SetActive(false);
                NoCanGrabTip.SetActive(false);
                LookHistoryBtn.SetActive(true);
                HasGrabTip.SetActive(true);
            }
            else
            {
                NoGrabSelfTip.SetActive(true);
                CanGrabTip.SetActive(false);
                GrabRedPacketBtn.SetActive(false);
                NoCanGrabTip.SetActive(false);
                LookHistoryBtn.SetActive(true);
                HasGrabTip.SetActive(false);
            }
        }
    }
}
