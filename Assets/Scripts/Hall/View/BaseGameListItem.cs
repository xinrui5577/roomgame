using Assets.Scripts.Common.Components;
using YxFramwork.Common.Model;
using YxFramwork.Controller;

namespace Assets.Scripts.Hall.View
{
    public class BaseGameListItem : NguiListItem
    {
        public void OnGameClickWithModel(GameUnitModel gModel)
        {
            RoomListController.Instance.ShowRoomList(gModel);
        }
    }
}
