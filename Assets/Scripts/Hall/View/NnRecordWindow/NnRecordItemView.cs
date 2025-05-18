using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.Utiles;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View.NnRecordWindow
{
    public class NnRecordItemView : YxView
    {
        public UILabel RoomId;
        public UILabel Round;
        public UILabel BetAnte;
        public UILabel GameType;
        public UILabel CreateDt;
        public UISprite DetialBtn;
        public UIGrid ItemGridPrefab;
        public NnRecordItem NnRecordItem;

        private UIGrid _itemGrid;
        private string _function;
        private string _roomId;
        private string _gameKey;

        protected override void OnFreshView()
        {
            YxWindowUtils.CreateMonoParent(ItemGridPrefab, ref _itemGrid);
            var dict = Data as IDictionary;
            if (dict == null) return;
            _roomId = dict.Contains("room_id") ? dict["room_id"].ToString() : "";
            if (RoomId != null) RoomId.text = _roomId;
            if (Round != null) Round.text = dict.Contains("round") ? dict["round"].ToString() : "";
            if (BetAnte != null) BetAnte.text = dict.Contains("betAnte") ? dict["betAnte"].ToString() : "";
            if (GameType != null) GameType.text = dict.Contains("gameType") ? dict["gameType"].ToString() : "";
            _gameKey = dict.Contains("game_key_c") ? dict["game_key_c"].ToString() : "";
            _function = dict.Contains("function") ? dict["function"].ToString() : "";
            if (CreateDt != null) CreateDt.text = dict.Contains("create_dt") ? dict["create_dt"].ToString() : "----/--/-- --:--:--";
            if (DetialBtn.GetComponent<UIButton>().onClick.Count == 0) DetialBtn.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnDetialBtn));
            if (!dict.Contains("overinfo")) return;
            if (!(dict["overinfo"] is List<object>)) return;
            var infoLists = dict["overinfo"] as List<object>;
            foreach (var infoList in infoLists)
            {
                if (!(infoList is Dictionary<string, object>)) continue;
                var infoData = infoList as Dictionary<string, object>;
                var item = YxWindowUtils.CreateItem(NnRecordItem, _itemGrid.transform);
                item.UpdateView(infoData);
            }
            _itemGrid.repositionNow = true;
        }

        public void OnDetialBtn()
        {
            var dic = new Dictionary<string, object>();
            dic["game_key_c"] = _gameKey;
            dic["room_id"] = _roomId;
            Facade.Instance<TwManager>().SendAction(_function, dic, OnFreshData);
        }

        private void OnFreshData(object msg)
        {
            var win = YxWindowManager.OpenWindow("RecordDetialWindow", true);
            if (win == null) return;
            win.UpdateView(msg);
        }
    }
}
