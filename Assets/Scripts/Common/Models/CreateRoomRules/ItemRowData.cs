using System.Collections.Generic;

namespace Assets.Scripts.Common.Models.CreateRoomRules
{
    /// <summary>
    /// 组件行数据
    /// </summary>
    public class ItemRowData
    {
        public string Id;
        public List<ItemData> Items;
        public CreateRoomRuleInfo Parent;
        public float Y=float.NaN;
        public float X=float.NaN;
        public float Height;
        public float Spacing = 40;

        /// <summary>
        /// 旧数据
        /// </summary>
        /// <param name="rowInfo"></param>
        /// <param name="parent"></param>
        public ItemRowData(IList<object> rowInfo, CreateRoomRuleInfo parent)
        {
            Items = new List<ItemData>();
            Parent = parent;
            if (rowInfo == null)
            {
                Items = null;
                return;
            }
            var len = rowInfo.Count;
            for (var i = 0; i < len; i++)
            {
                var item = rowInfo[i];
                var itemData = new ItemData(i, item, Parent);
                parent.AddItemData(itemData.Id, itemData);
                if (itemData.ShowType < 0) continue;
                Items.Add(itemData);
            }
        }

        /// <summary>
        /// 新版数据
        /// </summary>
        /// <param name="rowInfo"></param>
        /// <param name="parent"></param>
        public ItemRowData(IDictionary<string, object> rowInfo, CreateRoomRuleInfo parent)
        {
            Items = new List<ItemData>();
            Parent = parent;
            if (rowInfo == null)
            {
                Items = null;
                return;
            }
            Id = rowInfo.ContainsKey("id") ? rowInfo["id"].ToString() : "";
            if (rowInfo.ContainsKey("x"))
            {
                float.TryParse(rowInfo["x"].ToString(), out X);
            }
            if (rowInfo.ContainsKey("y"))
            {
                float.TryParse(rowInfo["y"].ToString(), out Y);
                Y = -Y;
            }
            if (rowInfo.ContainsKey("spacing"))
            {
                var temp = rowInfo["spacing"];
                if (temp != null) float.TryParse(rowInfo["spacing"].ToString(), out Spacing);
            }

            if (!rowInfo.ContainsKey("item")) return;
            var row = rowInfo["item"] as List<object>;
            if (row == null) return;
            var len = row.Count;
            for (var i = 0; i < len; i++)
            {
                var item = row[i];
                var itemData = new ItemData(i, item, Parent);
                parent.AddItemData(itemData.Id,itemData);
                if (itemData.ShowType < 0) continue;
                Items.Add(itemData);
            }
        }
    }
}
