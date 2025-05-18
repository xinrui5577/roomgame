using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon
{
    public class ShareContrl
    {
        private TableData _table;

        public ShareContrl()
        {
            EventDispatch.Instance.RegisteEvent((int)ShareEventID.OnWeiChatShareGameResult, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)ShareEventID.OnWeiChatShareTableInfo, OnRecvEvent);
        }

        public void Init(TableData data)
        {
            _table = data;
        }


        protected void OnRecvEvent(int eventId, EventData evn)
        {
            var id = (ShareEventID)eventId;
            switch (id)
            {
                case ShareEventID.OnWeiChatShareGameResult:
                    OnWeiChatShareGameResult(evn);
                    break;
                case ShareEventID.OnWeiChatShareTableInfo:
                    OnWeiChatShareTableInfo(evn);
                    break;
            }

        }
        public void OnWeiChatShareTableInfo(EventData evn)
        {
            YxTools.ShareFriend(_table.RoomInfo.RoomID.ToString(), _table.RoomInfo.GetRoomRuleString());
        }

        public void OnWeiChatShareGameResult(EventData evn)
        {
            var imageUrl = (string) evn.data1;

            Facade.Instance<WeChatApi>().InitWechat(AppInfo.WxAppId);

            UserController.Instance.GetShareInfo(info =>
                {
                    info.ImageUrl = imageUrl;
                    info.ShareType = ShareType.Image;
                    Facade.Instance<WeChatApi>().ShareContent(info);
                });
        }
    }
}
