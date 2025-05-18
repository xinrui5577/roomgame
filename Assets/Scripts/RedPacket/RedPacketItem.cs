using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.Utiles;
using YxFramwork.Common.DataBundles;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.RedPacket
{
    public class RedPacketItem : YxView
    {
        public NguiTextureAdapter Head;
        public NguiLabelAdapter NickName;
        public NguiLabelAdapter RedDetail;
        public NguiLabelAdapter RedShowState;
        public NguiSpriteAdapter RedPacketBg;
        public NguiLabelAdapter SpecialInfoItem;
        public UIGrid SpecialInfoGrid;
        public string HasGetRedPacket;
        public string OpenWindowName;

        private RedPacketData _redPacketData;
//        private bool _hasOpen;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            _redPacketData = Data as RedPacketData;
            if (_redPacketData == null) return;
            PortraitDb.SetPortrait(_redPacketData.Avatar, Head, 1);
            NickName.TrySetComponentValue(_redPacketData.NickName);
            var str = string.Format("{0}-{1}", YxUtiles.GetShowNumberForm(_redPacketData.RedMoney), _redPacketData.InmineNum);
            RedDetail.TrySetComponentValue(str);

            if (_redPacketData.SpecialInfos.Count != 0)
            {
                foreach (var strInfo in _redPacketData.SpecialInfos)
                {
                    var item = YxWindowUtils.CreateItem(SpecialInfoItem, SpecialInfoGrid.transform);
                    item.TrySetComponentValue(strInfo);
                }
                SpecialInfoGrid.repositionNow = true;
            }
        }

        public void OnClick()
        {
//            if (_hasOpen) return;
            var dic = new Dictionary<string, object>();
            dic["redId"] = _redPacketData.RedId;
            dic["op"] = 1;
            Facade.Instance<TwManager>().SendAction("RedEnvelope.checkRedEnvelope", dic, FreshRedPacketState);
        }

        private void FreshRedPacketState(object obj)
        {
            var redPacketStateData = new RedPacketStateData(obj);

//            if (redPacketStateData.Status == 3)
//            {
//                _hasOpen = true;
//            }
//            else
//            {
                var win = MainYxView as YxWindow;
                if (win)
                {
                    var child = win.CreateChildWindow(OpenWindowName);
                    child.UpdateView(redPacketStateData);
//                }
            }
            RedShowState.TrySetComponentValue("已领取");
            RedPacketBg.SetSpriteName(HasGetRedPacket);
        }
    }

    public class RedPacketStateData
    {
        public string NickName;
        public string Avatar;
        public int Status;
        public int RedId;
        public string InmineNum;

        public RedPacketStateData(object obj)
        {
            var info = obj as Dictionary<string, object>;
            if (info == null) return;
            var data = info["data"] as Dictionary<string, object>;
            if (data == null) return;
            if (data.ContainsKey("nick_m"))
            {
                NickName = data["nick_m"].ToString();
            }
            if (data.ContainsKey("avatar_x"))
            {
                Avatar = data["avatar_x"].ToString();
            }
            if (data.ContainsKey("status_i"))
            {
                Status = int.Parse(data["status_i"].ToString());
            }
            if (data.ContainsKey("id"))
            {
                RedId = int.Parse(data["id"].ToString());
            }
            if (data.ContainsKey("inmine_num"))
            {
                InmineNum = data["inmine_num"].ToString();
            }
        }
    }
}
