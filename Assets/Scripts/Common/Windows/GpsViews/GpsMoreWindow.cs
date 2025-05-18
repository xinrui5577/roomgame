using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Common.Windows.GpsViews
{
    /// <summary>
    /// 更多版gps
    /// </summary>
    public class GpsMoreWindow : YxNguiWindow
    {
        /// <summary>
        /// Item预制体
        /// </summary>
         [Tooltip("Item预制体")]
        public GpsMoreItemView ItemViewPrefab;
        /// <summary>
        /// 容器预制体
        /// </summary>
        [Tooltip("容器预制体")]
        public Transform ContainerPrefab;
        /// <summary>
        /// 警告距离
        /// </summary>
        [Tooltip("警告距离")]
        public int WarringDistance = 100;
        /// <summary>
        /// 间距
        /// </summary>
        [Tooltip("间距")]
        public int Spacing = 10;
        /// <summary>
        /// 距离过近文本内容
        /// </summary>
        [Tooltip("距离过近文本内容")]
        public string WarringTitle = "该{0}位 玩家距离<={1}";
        /// <summary>
        /// 无距离过近文本内容
        /// </summary>
        [Tooltip("无距离过近文本内容")]
        public string NoWarringTitle = "无玩家距离过近";
        /// <summary>
        /// 正常标题背景名称
        /// </summary>
        [Tooltip("正常标题背景名称")]
        public string TitleBgNormalName = "normal";
        /// <summary>
        /// 警告标题背景名称
        /// </summary>
        [Tooltip("警告标题背景名称")]
        public string TitleBgWarringName = "warring";
        private Transform _container;

        protected override void OnFreshView()
        {
            YxWindowUtils.CreateItemParent(ContainerPrefab, ref _container); 
            var gdata = App.GameData;
            if (gdata == null) return;
            var playerList = gdata.PlayerList;
            if (playerList == null) return;
            var pcount = playerList.Length;
            var noLastCount = pcount - 1;
            var dict = new Dictionary<int, GpsMoreItemData>();
            GpsMoreItemData itemData = null;

            var gpsMgr = Facade.GetInterimManager<YxGPSManager>();
            if (gpsMgr != null)
            {
                var warnDistance = gpsMgr.WarnDistance;
                for (var i = 1; i < noLastCount; i++)
                {
                    var player1 = gdata.GetPlayer(i);
                    if (player1 == null) continue;//该座位暂时没人
                    var userInfo1 = player1.Info;
                    if (userInfo1 == null) continue;//该座位暂时没人
                    for (var j = i + 1; j < pcount; j++)
                    {
                        var distance = gpsMgr.GetDistance(i, j); //Vector2.Distance(p1.Gps, p2.Gps);
                        var player2 = gdata.GetPlayer(j);
                        if (player2 == null) continue;//该座位暂时没人
                        var userInfo2 = player2.Info;
                        if (userInfo2 == null) continue;//该座位暂时没人
                        if (distance < 0)
                        {
                            continue;
                        } 
                        if (!(distance <= warnDistance)) continue;//不是警告距离
                        if (FindRepeated(dict, userInfo1, userInfo2)) continue;
                        itemData = dict.ContainsKey(i) ? dict[i] : dict[i] = new GpsMoreItemData(userInfo1); 
                        itemData.List.Add(userInfo2);
                    }
                    if(itemData==null)continue;
                    var dcount = itemData.List.Count;
                    if(dcount<1)continue;
                    var warring = warnDistance < 1000
                        ? string.Format("{0}米", warnDistance)
                        : string.Format("{0:0.##}千米", warnDistance / 1000f);
                    itemData.Title = string.Format(WarringTitle, dcount, warring);//有 标题 “该n位 玩家距离过近” ；
                    itemData.TitleType = TitleBgWarringName;
                }
            }
            if (dict.Count < 1) //没有就显示所有玩家，标题 “无 玩家距离过近”;
            {
                itemData = new GpsMoreItemData
                {
                    Title = NoWarringTitle,
                    TitleType = TitleBgNormalName
                };
                var nolist = itemData.List;
                var gameData = App.GameData;
                for (var i = 1; i < pcount; i++)
                {
                    var userinfo = gameData.GetPlayerInfo(i);
                    if(userinfo==null)continue;
                    nolist.Add(userinfo);
                }
                if (nolist.Count > 0)
                {
                    dict[0] = itemData;
                }
            }
            var curPos = Vector2.zero;
            foreach (var data in dict)
            {
                var item = YxWindowUtils.CreateItem(ItemViewPrefab, _container, curPos);
                item.UpdateViewWithCallBack(data.Value, FreshMakeup);
                item.transform.localPosition = curPos;
                curPos.y -= item.Height + Spacing;
            }
        }

        private static bool FindRepeated(Dictionary<int, GpsMoreItemData> dict, YxBaseUserInfo p1, YxBaseUserInfo p2)
        {
            foreach (var keyValue in dict)
            {
                var data = keyValue.Value;
                var list = data.List;
                var count = list.Count; 
                for (var i= list.IndexOf(p1) + 1; i < count; i++)
                {
                    var temp = list[i];
                    if (p2 != temp) continue;
                    return true;
                } 
            }
            return false;
        }

        private void FreshMakeup(object obj)
        {

        }
    }
}
