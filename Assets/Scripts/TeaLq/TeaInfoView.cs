using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using YxFramwork.Framework;

namespace Assets.Scripts.TeaLq
{
    public class TeaInfoView : YxView
    {
        public NguiLabelAdapter TeaName;
        public NguiLabelAdapter FloorAndGameName;
        public NguiLabelAdapter TeaId;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = Data as TeaNewData;
            if (data != null)
            {
                if (data.Floor == 0) return;
                TeaName.TrySetComponentValue(data.TeaName);
                var gameName = string.Format("{0}楼 {1}", data.Floor, data.GameName);
                FloorAndGameName.TrySetComponentValue(gameName);
                var teaId = string.Format("(ID:{0})", data.TeaId);
                TeaId.TrySetComponentValue(teaId);
            }
        }
    }
}
