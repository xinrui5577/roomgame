using System.Collections.Generic;
using Assets.Scripts.Common.Models.CreateRoomRules;
using UnityEngine;

namespace Assets.Scripts.Common.Models
{
    /// <summary>
    /// 每行的组数据
    /// 
    /// </summary>
    public class GroupRowData
    {
        public CreateRoomRuleInfo Root;
        /// <summary>
        /// id
        /// </summary>
        public string Id;
        /// <summary>
        /// x偏移量
        /// </summary>
        public float X;
        /// <summary>
        /// y偏移量
        /// </summary>
        public float Y;
        /// <summary>
        /// 俩组之间的间距
        /// </summary>
        public float Spacing;
        /// <summary>
        /// 组列表
        /// </summary>
        public List<GroupData> GroupDatas;
        /// <summary>
        /// 数据是Dictionary，有行有列
        /// </summary>
        /// <param name="data"></param>
        public GroupRowData(Dictionary<string, object> data)
        {
            if (data == null) { return;}
            const string keyId = "id";
            const string keyX = "x";
            const string keyY = "y";
            const string keyGroups = "groups";

            Id = data.ContainsKey(keyId) ? data[keyId].ToString() : "";
            if (data.ContainsKey(keyX))
            {
                float.TryParse(data[keyX].ToString(), out X);
            }
            if (data.ContainsKey(keyY))
            {
                float.TryParse(data[keyY].ToString(), out X);
            }
            if (data.ContainsKey(keyGroups))
            {
                var groups = data[keyGroups] as List<object>;
                if (groups != null)
                {
                    var count = groups.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var group = groups[i] as Dictionary<string,object>;
                        if (group != null)
                        {
                            var groupData = new GroupData(group, Root);
                            GroupDatas.Add(groupData);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 数据是List ，只有行数据
        /// </summary>
        /// <param name="data"></param>
        public GroupRowData(List<object> data)
        {
        }
    }
}
