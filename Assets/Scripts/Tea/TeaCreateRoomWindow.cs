using System.Collections.Generic;
using Assets.Scripts.Hall.View.AboutRoomWindows;
using YxFramwork.Controller;

namespace Assets.Scripts.Tea
{
    public class TeaCreateRoomWindow : CreateRoomWindow
    {
        public TeaPanel teaPanel;

        protected override void SendCreateRoom(Dictionary<string, object> data)
        {
            if (teaPanel != null)
            {
                string teaid = teaPanel.Code.text;
                data["tea_id"] = teaid;
            }
            RoomListController.Instance.CreatGroupRoom(data, CreateRoomBack); 
        }

        private void CreateRoomBack(object obj)
        {
            TeaUtil.GetBackString(obj);
            teaPanel.GetTableList(false);
            Close();
        }
    }
}
