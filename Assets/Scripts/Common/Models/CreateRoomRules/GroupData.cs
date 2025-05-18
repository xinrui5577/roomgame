using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common.Models.CreateRoomRules
{
    public class GroupData
    {
        public string Id;
        public string Name;
        public List<ItemRowData> RowDatas;
        public CreateRoomRuleInfo Parent;
        public float CellWidth;
        public float CellHeight;
        public int NameWidth;
        public float NameX = float.NaN;
        public float NameY = float.NaN;
        /// <summary>
        /// 顶部偏移量
        /// </summary>
        public float OffY;

        public GroupData(KeyValuePair<string, object> groups, CreateRoomRuleInfo parent)
        {
            RowDatas = new List<ItemRowData>();
            Name = groups.Key;
            Parent = parent;
            var rowlist = groups.Value as List<object>;
            if (rowlist == null)
            {
                return;
            }
            foreach (var row in rowlist)
            {
                var rowInfo = row as List<object>;
                if (rowInfo == null) continue;
                var rowData = new ItemRowData(rowInfo, Parent);
                RowDatas.Add(rowData);
            }
        }

        public GroupData(IDictionary<string, object> groupInfo, CreateRoomRuleInfo parent)
        {
            RowDatas = new List<ItemRowData>();
            Id = groupInfo.ContainsKey("id") ? groupInfo["id"].ToString() : "";
            Name = groupInfo.ContainsKey("name") ? groupInfo["name"].ToString() : "";
            if (groupInfo.ContainsKey("cellwidth"))
            {
                float.TryParse(groupInfo["cellwidth"].ToString(), out CellWidth);
            }
            if (groupInfo.ContainsKey("cellheight"))
            {
                float.TryParse(groupInfo["cellheight"].ToString(), out CellHeight);
            }
            if (groupInfo.ContainsKey("nameX"))
            {
                float.TryParse(groupInfo["nameX"].ToString(), out NameX);
            }
            if (groupInfo.ContainsKey("nameY"))
            {
                float.TryParse(groupInfo["nameY"].ToString(), out NameY);
                NameY = -NameY;
            }
            if (groupInfo.ContainsKey("namewidth"))
            {
                int.TryParse(groupInfo["namewidth"].ToString(), out NameWidth);
            }
            OffY = groupInfo.ContainsKey("offy") ? -float.Parse(groupInfo["offy"].ToString()) : -10;
            Parent = parent;
            if (!groupInfo.ContainsKey("rows")) return;
            var rows = groupInfo["rows"] as List<object>;
            if (rows == null) return;
            foreach (var row in rows)
            {
                var rowInfo = row as Dictionary<string, object>;
                if (rowInfo == null) continue;
                var rowData = new ItemRowData(rowInfo, Parent);
                RowDatas.Add(rowData);
            }
        }
    }
}
