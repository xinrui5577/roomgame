using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelInviteFriends), UIPanelhierarchy.Popup)]
    public class PanelInviteFriends : UIPanelBase
    {
        public MahFriendScrollController Controller;
        private bool mDataInitFlag;

        public override void OnStartGameUpdate() { Close(); }

        public override void Open()
        {
            base.Open();
            if (!mDataInitFlag)
            {
                mDataInitFlag = true;
                var apiInfo = new Dictionary<string, object>() { { "bundleID", Application.bundleIdentifier } };
                MahjongUtility.SendAction("mahjongwm.wmFriends", apiInfo, data =>
                {
                    var info = (Dictionary<string, object>)data;
                    if (info.ContainsKey("data"))
                    {
                        List<object> list = (List<object>)info["data"];
                        Controller.SetData(list);
                    }
                });
            }
        }
    }
}