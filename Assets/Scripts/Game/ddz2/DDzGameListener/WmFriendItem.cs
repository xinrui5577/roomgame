using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Game.ddz2.InheritCommon;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.DataBundles;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.ddz2.DDzGameListener
{
    public class WmFriendItem : MonoBehaviour
    {

        public NguiTextureAdapter Image;

        public UILabel NickLabel;

        public UILabel IdLabel;

        private int _id;



        public void SetData(object data)
        {
            var dic = (Dictionary<string,object>)data ;
            NickLabel.text = (string)dic["nick_m"];
            string id = (string) dic["user_id"];
            IdLabel.text = id;
            int.TryParse(id, out _id);
            string sex = (string)dic["sex_i"];
            int sexI;
            int.TryParse(sex, out sexI);
            PortraitDb.SetPortrait((string)dic["avatar_x"], Image, sexI);
        }

        public void OnClickItem()
        {
            var gdata = App.GetGameData<DdzGameData>();
            var playerList = gdata.PlayerList;
            for (int i = 0; i < playerList.Length; i++)
            {
                var info = playerList[i].Info;
                if (info == null) continue;
                if (info.Id == _id)
                {
                    YxMessageBox.Show("此玩家已经在房间中");
                    return;
                }
            }

            var apiInfo = new Dictionary<string, object>()
            {
                {"bundleID", Application.bundleIdentifier},
                {"inviteId", _id},
                {"roomId", gdata.RoomId}
            };
            Facade.Instance<TwManager>().SendAction("mahjongwm.inviteWmFriends", apiInfo, data => { });
        }
    }
}
