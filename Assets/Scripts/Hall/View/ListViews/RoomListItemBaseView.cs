using YxFramwork.Common.Model;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.ListViews
{
    /// <summary>
    /// 房间列表视图样式基类
    /// </summary>
    public class RoomListItemBaseView : YxView
    {
        /// <summary>
        /// 房间信息
        /// </summary>
        protected RoomUnitModel RoomInfo;

        public void Init(RoomUnitModel roomModel)
        {
            RoomInfo = roomModel;
            OnInit();
        }

        protected virtual void OnInit()
        {
            
        }


        public void OnRoomClick(string index)
        {
            var roomItem = MainYxView as RoomListItem;
            if (roomItem == null) { return;}
            roomItem.OnRoomClick(index);
        }
    }
}
