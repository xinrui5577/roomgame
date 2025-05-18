using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using YxFramwork.Common.DataBundles;
using YxFramwork.Framework;

namespace Assets.Scripts.TeaLq
{
    public class TeaTableInfoItem : YxView
    {
        public NguiTextureAdapter UserHead;
        public NguiLabelAdapter UserName;
        public NguiLabelAdapter UserId;
        public NguiLabelAdapter UserIp;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = Data as TeaTableInfoData;
            if (data == null) return;
            PortraitDb.SetPortrait(data.Avatar, UserHead, data.Sex);
            UserName.TrySetComponentValue(data.NickName);
            UserId.TrySetComponentValue(data.UserId);
            UserIp.TrySetComponentValue(data.UserIp);
        }
    }
}
