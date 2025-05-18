using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.MahStadiumListWindow
{
    public class MahPlayerHeadItem : YxView
    {
        public UILabel UserGold;
        public UILabel UserName;
        public GameObject OwnerIcon;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var userInfos = Data as Dictionary<string, object>;
            if (userInfos != null)
            {
                var userGold = userInfos["gold"].ToString();
                YxTools.TrySetComponentValue(UserGold, userGold);
                var userName = userInfos["name"].ToString();
                YxTools.TrySetComponentValue(UserName, userName);
                if (userInfos.ContainsKey("owner"))
                {
                    var owner = bool.Parse(userInfos["owner"].ToString());
                    YxTools.TrySetComponentValue(OwnerIcon, owner);
                }
            }
        }
    }
}
