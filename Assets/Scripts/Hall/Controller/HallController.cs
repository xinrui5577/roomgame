using System.Collections.Generic;
using Assets.Scripts.Hall.Models;
using Assets.Scripts.Hall.View.Panels;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.Controller
{
    public class HallController : YxBaseController<HallController>
    {
        /// <summary>
        /// 当前游戏分组
        /// </summary>
        public int CurGameGroup;
        public int RoomListStyleIndex;
        public int DeskListStyleIndex;
        /// <summary>
        /// 请求走势信息
        /// </summary>
        /// <param name="deskData"></param>
        /// <param name="callBack"></param>
        public void SendGetDesks(DeskData deskData,TwCallBack callBack)
        {
            var dict = new Dictionary<string, object>();
            dict["gameKey"] = deskData.GameKey;
            if (deskData.RoomType != null)
            {
                dict["groupId"] = GetGroupId(deskData.GameKey, deskData.RoomType);//;
            }
            Facade.Instance<TwManager>().SendAction("room.getDesks", dict, callBack);
        }

        /// <summary>
        /// 请求所有走势信息
        /// </summary>
        /// <param name="deskData"></param>
        /// <param name="callBack"></param>
        public void SendGetGameRecord(DeskData deskData,TwCallBack callBack)
        {
            var dict = new Dictionary<string, object>();
            dict["gameKey"] = deskData.GameKey;
            if (deskData.RoomType != null)
            {
                dict["groupId"] = GetGroupId(deskData.GameKey, deskData.RoomType);//;
            }
            Facade.Instance<TwManager>().SendAction("room.getGameRecord",dict, callBack);
        }

        /// <summary>
        /// 请求桌面走势
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="callBack"></param>
        public void SendGetGameRecordByRoomId(int roomId, TwCallBack callBack)
        {
            var dict = new Dictionary<string, object>();
            dict["roomId"] = roomId;
            Facade.Instance<TwManager>().SendAction("room.getGameRecord", dict, callBack);
        }

        /// <summary>
        /// 获得组id
        /// </summary>
        /// <param name="gameKey"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetGroupId(string gameKey, string type)
        {
            return string.Format("{0}_{1}_", gameKey, type);
        }

        /// <summary>
        /// 显示游戏列表面板
        /// </summary>
        /// <returns></returns>
        public bool ShowGameListPanel()
        {
            var panelMgr = Facade.Instance<YxPanelManager>();
            var panel = panelMgr.GetPanel<HallPanel>("GameHallPanel");
            if (panel == null) return false;
            var gameListPanel = panel.ShowChildPanel("GameListPanel");
            return gameListPanel == null;
        }

        /// <summary>
        /// 显示房间类型列表面板
        /// </summary>
        /// <returns></returns>
        public bool ShowRoomListPanel()
        {
            var panelMgr = Facade.Instance<YxPanelManager>();
            var panel = panelMgr.GetPanel<HallPanel>("GameHallPanel");
            if (panel == null) return false;
            var roomListPanel = panel.ShowChildPanel("RoomListPanel");
            return roomListPanel == null;
        }

        /// <summary>
        /// 打开桌面窗口
        /// </summary>
        public bool ShowDeskListPanel(string gameKey)
        {
            var panelMgr = Facade.Instance<YxPanelManager>();
            var panel = panelMgr.GetPanel<HallPanel>("GameHallPanel");
            if (panel == null) return false;
            var deskData = new DeskData
            {
                GameKey = gameKey,
                RoomType = null
            };
            var deskListPanel = panel.ShowChildPanel("DeskListPanel", deskData);
            return deskListPanel == null;
        } 

    }
}
