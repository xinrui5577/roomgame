using System.Collections.Generic;
using YxFramwork.Common.Model;
using YxFramwork.Framework;
using YxFramwork.View;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahFriendsCellView : SuperScrollerCellView
    {
        public YxBaseGamePlayer Player;

        public void SetData(YxBaseUserInfo data)
        {
            Player.Info = data;
        }

        public void OnItemClick()
        {
            var db = GameCenter.DataCenter;
            if (db.Players.SearchPlayer(data => data.UserId == Player.Info.UserId))
            {
                YxMessageBox.Show(new YxMessageBoxData()
                {
                    Msg = "玩家已经在房间了！",
                    IsTopShow = true,
                    BtnStyle = YxMessageBox.MiddleBtnStyle
                });
                return;
            }
            if (db.MaxPlayerCount == db.Players.CurrPlayerCount)
            {
                YxMessageBox.Show(new YxMessageBoxData()
                {
                    Msg = "房间已满！",
                    IsTopShow = true,
                    BtnStyle = YxMessageBox.MiddleBtnStyle
                });
                return;
            }
            var apiInfo = new Dictionary<string, object>()
            {
                { "bundleID", Application.bundleIdentifier },
                { "inviteId", Player.Info.UserId },
                { "roomId", db.Room.RoomID }
            };
            MahjongUtility.SendAction("mahjongwm.inviteWmFriends", apiInfo, data => { });
        }
    }
}