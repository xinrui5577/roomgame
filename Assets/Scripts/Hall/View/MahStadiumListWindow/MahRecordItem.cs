using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.MahStadiumListWindow
{
    public class MahRecordItem : YxView
    {
        public UILabel RoomId;
        public UILabel GameName;
        public UILabel GameCreatTime;
        public MahPlayerHeadItem MahPlayerHeadItem;
        public UIGrid MahPlayerHeadGrid;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var infoData = Data as Dictionary<string, object>;
            if (infoData != null)
            {
                var gameName = infoData["game_name"].ToString();
                var str = string.Format("({0})", gameName);
                YxTools.TrySetComponentValue(GameName, str);
                var roomId = infoData["room_id"].ToString().Substring(3, 6);
                YxTools.TrySetComponentValue(RoomId, roomId);
                var gameCreatTime = infoData["create_dt"].ToString();
                YxTools.TrySetComponentValue(GameCreatTime, gameCreatTime);
                var infos = infoData["overinfo"];
                var infoLists = infos as List<object>;
                if (infoLists != null)
                {
                    foreach (var infoList in infoLists)
                    {
                        var infoDic = infoList as Dictionary<string, object>;
                        if (infoDic != null)
                        {
                            var item = YxWindowUtils.CreateItem(MahPlayerHeadItem, MahPlayerHeadGrid.transform);
                            item.UpdateView(infoDic);
                        }
                    }
                    MahPlayerHeadGrid.repositionNow = true;
                }
            }
        }
    }
}
