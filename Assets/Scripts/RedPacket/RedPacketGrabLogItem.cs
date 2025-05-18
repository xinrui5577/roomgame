using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common.DataBundles;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketGrabLogItem : YxView
    {
        public NguiTextureAdapter Head;
        public NguiLabelAdapter NickName;
        public NguiLabelAdapter GrabTime;
        public NguiLabelAdapter GrabMoney;
        public GameObject BombBg;
        public GameObject Luck;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = Data as RedPacketLogData;
            if (data == null) return;
            if (data.IncludeMe)
            {
                gameObject.GetComponent<BoxCollider>().enabled = true;
            }
            PortraitDb.SetPortrait(data.Avatar, Head, 1);
            NickName.TrySetComponentValue(data.NickName);
            GrabTime.TrySetComponentValue(data.GrabTime);
            GrabMoney.TrySetComponentValue(YxUtiles.GetShowNumberForm(data.GrabMoney));
            BombBg.SetActive(data.IsPaymine);
            Luck.SetActive(data.IsLuck);
        }
    }
}
