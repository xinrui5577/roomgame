using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using YxFramwork.Controller;

namespace Assets.Scripts.Hall.View.CombatGains
{
    /// <summary>
    /// 视频重播窗口
    /// </summary>
    public class CombatGainsDetailWindow : YxNguiWindow
    {
        public UIGrid ItemGridPrefab;
        public CombatGainsDetailItemView ItemPrefab;

        private UIGrid _itemGrid;

        protected override void OnFreshView()
        {
            if (Data == null) return;
            var roomId = Data.ToString();
            if (string.IsNullOrEmpty(roomId)) return;
            var gameKey = CombatGainsController.Instance.CurGameKey;
            CombatGainsController.Instance.GetGameDetail(gameKey, roomId, UpdateViewData);
        }

        private void UpdateViewData(object msg)
        {
            YxWindowUtils.CreateItemGrid(ItemGridPrefab, ref _itemGrid);
            if (msg == null) return;
            var dict = msg as Dictionary<string,object>;
            if (dict == null) return;
            if (dict.ContainsKey("detail"))
            {
                var list = dict["detail"] as List<object>;
                var weburl = "";
                if (dict.ContainsKey("web_host"))
                {
                    var temp = dict["web_host"];
                    weburl = temp.ToString();
                }
                if (list != null)
                {
                    var count = list.Count;
                    for (var i = 0; i < count;i++ )
                    {
                        var obj = list[i];
                        if (obj == null) continue;
                        if (!(obj is Dictionary<string, object>)) continue;
                        var cgdDict = obj as Dictionary<string, object>;
                        var item = YxWindowUtils.CreateItem(ItemPrefab, _itemGrid.transform);
                        var cgdData = new CombatGainsDetailItemData(cgdDict)
                            {
                                Index = (i+1).ToString(),
                                WebHost = weburl
                            };
                        item.UpdateView(cgdData);
                    }
                }
            }
            if (dict.ContainsKey("room_id"))
            {
            }
            _itemGrid.repositionNow = true;
            _itemGrid.Reposition();
        }
    }  
}
