using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.AboutRoomWindows;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.TableList
{
    /// <summary>
    /// 亲友圈缺人牌桌列表
    /// </summary>
    public class SocialTableListView : BaseSocialWrapListView
    {
        /// <summary>
        /// 房间中心
        /// </summary>
        private SocialRoomCenter _roomManager
        {
            get { return Facade.Instance<SocialRoomCenter>().InitCenter(); }
        }
        protected override void OnVisible()
        {
            _roomManager.GetShowList();
        }

        protected override void AddListeners()
        {
            AddEventListener<Dictionary<string, object>>(InitAction, OnInitReceive);
            AddEventListener<Dictionary<string, object>>(SocialTools.KeyActionRoomUpdate,OnTableUpdate);
        }

        protected override void RemoveListeners()
        {
            RemoveEventListener<Dictionary<string, object>>(InitAction, OnInitReceive);
            RemoveEventListener<Dictionary<string, object>>(SocialTools.KeyActionRoomUpdate,OnTableUpdate);
        }

        protected override void OnInitDataValid()
        {
            Dictionary<string, Dictionary<string, object>> getDic;
            InitGetData.TryGetValueWitheKey(out PageIds, SocialTools.KeyIds);
            InitGetData.TryGetValueWitheKey(out getDic, SocialTools.KeyData);
            FreshWrapList(getDic);
        }


        private void OnTableUpdate(Dictionary<string,object> tableInfo)
        {
            _roomManager.InitList();
        }

        /// <summary>
        /// 点击牌桌
        /// </summary>
        /// <param name="args">房间参数</param>
        /// <param name="gameKey">gameKey</param>
        /// <param name="roomId">房间号</param>
        /// <param name="windowName">窗口名称</param>
        public void OnClickTableItem(string args,string gameKey,string roomId,string windowName= "CreateRoomWindow")
        {
            var window = MainYxView.OpenWindowWithData(windowName, null) as CreateRoomWindow;
            if (window)
            {
                window.RoomLastOption = args;
                window.GameKey = gameKey;
               var findRoomWindow=window.GetComponent<FindRoomWindow>();
                if (findRoomWindow)
                {
                    findRoomWindow.RoomIdLabel.TrySetComponentValue(roomId);
                }
            }
        }
    }
}
