using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using YxFramwork.Common.DataBundles;
using YxFramwork.Framework;

namespace Assets.Scripts.TeaLq
{
    public class TeaSimpleUserInfoView : YxView
    {
        public NguiTextureAdapter UserHead;
        public NguiLabelAdapter UserName;

        private string _head;
        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = Data as TeaTableSeatData;
            if (data == null) return;
            if (string.IsNullOrEmpty(_head))
            {
                _head = data.Avatar;
                PortraitDb.SetPortrait(data.Avatar, UserHead, data.Sex);
                UserName.TrySetComponentValue(data.NickName);
            }
            else
            {
                if (!_head.Equals(data.Avatar))
                {
                    _head = data.Avatar;
                    PortraitDb.SetPortrait(data.Avatar, UserHead, data.Sex);
                    UserName.TrySetComponentValue(data.NickName);
                }
            }
        }
    }
}
