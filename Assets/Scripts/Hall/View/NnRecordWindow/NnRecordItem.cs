using System.Collections.Generic;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.NnRecordWindow
{
    public class NnRecordItem : YxView
    {
        public UILabel UserId;
        public UILabel UserName;
        public UILabel UserGold;
        public UISprite RoomOwner;
        public UISprite BigWinner;
        public UISprite Kuang;
        public YxBaseTextureAdapter UserHead;

        protected override void OnFreshView()
        {
            if (!(Data is Dictionary<string, object>)) return;
            var userData = Data as Dictionary<string, object>;
            if (UserId != null) UserId.text = userData.ContainsKey("id") ? userData["id"].ToString() : "";
            if (UserName != null) UserName.text = userData.ContainsKey("name") ? userData["name"].ToString() : "";
            var avatar = userData.ContainsKey("avatar_x") ? userData["avatar_x"].ToString() : "";
            var sex = userData.ContainsKey("sex_i") ? int.Parse(userData["sex_i"].ToString()) : 1;
            if (UserHead != null) PortraitDb.SetPortrait(avatar, UserHead, sex);
            var glod = userData.ContainsKey("gold") ? int.Parse(userData["gold"].ToString()) : 0;
            if (UserGold != null) UserGold.text = glod >= 0 ? "[BA412DFF]+" + glod : "[599239FF]" + glod;
            var bigWin = userData.ContainsKey("dayingjia") && bool.Parse(userData["dayingjia"].ToString());
            if (!bigWin && Kuang != null) Kuang.spriteName = "lose";
            if (BigWinner != null) BigWinner.gameObject.SetActive(bigWin);
            var owner = userData.ContainsKey("owner") && bool.Parse(userData["owner"].ToString());
            if (RoomOwner != null) RoomOwner.gameObject.SetActive(owner);
        }
    }
}
