using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using YxFramwork.Common.DataBundles;
using YxFramwork.Common.Utils;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.TeaLq
{
    public class TeaHouseListItem : YxView
    {
        public NguiTextureAdapter UserHead;
        public NguiLabelAdapter TeaInfo;
        public NguiLabelAdapter TeaOwnerName;
        public UIToggle Toggle;

        private TeaHouseListData _teaHouseListData;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            _teaHouseListData = Data as TeaHouseListData;
            if (_teaHouseListData == null) return;

            if (Util.HasKey("tea_id"))
            {
                var teaId = Util.GetInt("tea_id");
                if (_teaHouseListData.TeaId == teaId)
                {
                    Toggle.startsActive = true;
                }
            }
            PortraitDb.SetPortrait(_teaHouseListData.Avatar, UserHead, _teaHouseListData.Sex);
            var teaInfo = string.Format("{0}({1})", _teaHouseListData.TeaName, _teaHouseListData.TeaId);
            TeaInfo.TrySetComponentValue(teaInfo);
            var ownerName = string.Format("老板:{0}", _teaHouseListData.OwnerName);
            TeaOwnerName.TrySetComponentValue(ownerName);
        }

        public void OnChangeTea()
        {
            if (Toggle.value)
            {
                var dic = new Dictionary<string, object>();
                dic["teaId"] = _teaHouseListData.TeaId;
                Util.SetInt("tea_id", _teaHouseListData.TeaId);
                Facade.EventCenter.DispatchEvent<string, Dictionary<string, object>>("TeaChange", dic);
            }
        }
    }
}
