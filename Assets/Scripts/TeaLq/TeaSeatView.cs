using System.Collections.Generic;
using YxFramwork.Framework;

namespace Assets.Scripts.TeaLq
{
    public class TeaSeatView : YxView
    {
        public List<TeaSimpleUserInfoView> UserInfoViews;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = Data as TeaTableSeatData;
            if (data == null) return;
            if (data.SeatId >= UserInfoViews.Count) return;
            if (data.UserId == -1 && UserInfoViews[data.SeatId])
            {
                UserInfoViews[data.SeatId].gameObject.SetActive(false);
                return;
            }
            if (UserInfoViews[data.SeatId])
            {
                UserInfoViews[data.SeatId].gameObject.SetActive(true);
                UserInfoViews[data.SeatId].UpdateView(data);
            }
        }
    }
}
