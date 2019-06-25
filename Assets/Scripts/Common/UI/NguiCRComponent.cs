using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.UI
{
    public class NguiCRComponent : NguiView
    {
        /// <summary>
        /// 提示按钮
        /// </summary>
        public GameObject TipBtn;

        protected override void OnFreshView()
        {
            var itemData = GetData<ItemData>();
            if (itemData == null) return;
            if (TipBtn != null)
            {
                var tip = itemData.Tip;
                TipBtn.SetActive(!string.IsNullOrEmpty(tip));
            }
            OnFreshCRCView(itemData);
            if (CallBack != null) CallBack(null);
        }

        protected virtual void OnFreshCRCView(ItemData itemData)
        {
        }

        private static YxView _curView;
        /// <summary>
        /// 
        /// </summary>
        public void ShowTip()
        {
            if (_curView == this)
            {
                UITooltip.Hide();
                _curView = null;
                return;
            }
            _curView = this;
            var itemData = GetData<ItemData>();
            if (itemData == null) return;
            UITooltip.Show(itemData.Tip);
        }


        /// <summary>
        /// 是否有效
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValid()
        {
            return true;
        }
    }


    public class RuleInfo
    {
        public const string PrefsKey = "RuleInfo";
        public string Id;
        public string Name;
        public string Key;
        public string GameKey;
        public string Type;
        public List<GroupData> GroupDatas;
        public readonly Dictionary<string, string[]> Tabs;
        public readonly Dictionary<string, ItemData> CreateArgs = new Dictionary<string, ItemData>();
        public string CurTabItemId = "";
        public int TabDefaultIndex;
        public RuleInfo(IDictionary dict, int defuatIndex = -1)
        {
            TabDefaultIndex = defuatIndex;
            GroupDatas = new List<GroupData>();
            Tabs = new Dictionary<string, string[]>();
            if (dict == null)
            {
                Id = "";
                Name = "";
                Key = "";
                GameKey = "";
                Type = "";
                return;
            }
            Id = dict.Contains("id") ? dict["id"].ToString() : "";
            Name = dict.Contains("name") ? dict["name"].ToString() : "";
            Key = dict.Contains("key") ? dict["key"].ToString() : "";
            GameKey = dict.Contains("gamekey") ? dict["gamekey"].ToString() : "";
            Type = dict.Contains("type") ? dict["type"].ToString() : "";
            if (!dict.Contains("rule")) return;
            var groupObj = dict["rule"];
            if (groupObj == null) return;
            //旧版结构
            if (groupObj is Dictionary<string, object>)
            {
                var groupDict = groupObj as Dictionary<string, object>;
                //if (groupDict == null) return;
                GroupDatas.Clear();
                foreach (var groups in groupDict)
                {
                    var groupData = new GroupData(groups, this);
                    GroupDatas.Add(groupData);
                }
                return;
            }
            //新版结构
            if (!(groupObj is List<object>)) return;
            var groupList = groupObj as List<object>;
            GroupDatas.Clear();
            foreach (var groupInfo in groupList)
            {
                var group = groupInfo as Dictionary<string, object>;
                if (group == null) continue;
                var groupData = new GroupData(group, this);
                GroupDatas.Add(groupData);
            }
        }

        public static string GetPrefsKey(string tabId, string id)
        {
            return string.Format("{0}_{1}{2}", PrefsKey, tabId, id);
        }


        public static bool GetItemState(string tabId, string id, bool defaultState)
        {
            var prefsId = GetPrefsKey(tabId, id);
            var b = PlayerPrefs.HasKey(prefsId) ? PlayerPrefs.GetInt(prefsId) > 0 : defaultState;
            //            Debug.Log(prefsId + " " + b);
            return b;
        }

        public static void SaveItemState(string tabId, string id, bool state)
        {
            var prefsId = GetPrefsKey(tabId, id);
            //            Debug.Log(prefsId + " " + state);
            PlayerPrefs.SetInt(prefsId, state ? 1 : 0);
        }
    }

    /// <summary>
    /// 一个组
    /// </summary>
    public class GroupData
    {
        public string Name;
        public List<RowData> RowDatas;
        public RuleInfo Parent;
        public float CellWidth;
        public float CellHeight;
        public int NameWidth;
        /// <summary>
        /// 顶部偏移量
        /// </summary>
        public float OffY;

        public GroupData(KeyValuePair<string, object> groups, RuleInfo parent)
        {
            RowDatas = new List<RowData>();
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
                var rowData = new RowData(rowInfo, Parent);
                RowDatas.Add(rowData);
            }
        }

        public GroupData(IDictionary<string, object> groupInfo, RuleInfo parent)
        {
            RowDatas = new List<RowData>();
            Name = groupInfo.ContainsKey("name") ? groupInfo["name"].ToString() : "";
            if (groupInfo.ContainsKey("cellwidth"))
            {
                float.TryParse(groupInfo["cellwidth"].ToString(), out CellWidth);
            }
            if (groupInfo.ContainsKey("cellheight"))
            {
                float.TryParse(groupInfo["cellheight"].ToString(), out CellHeight);
            }
            if (groupInfo.ContainsKey("namewidth"))
            {
                int.TryParse(groupInfo["namewidth"].ToString(), out NameWidth);
            }
            OffY = groupInfo.ContainsKey("offy") ? float.Parse(groupInfo["offy"].ToString()) : 10;
            Parent = parent;
            if (!groupInfo.ContainsKey("rows")) return;
            var rows = groupInfo["rows"] as List<object>;
            if (rows == null) return;
            foreach (var row in rows)
            {
                var rowInfo = row as Dictionary<string, object>;
                if (rowInfo == null) continue;
                var rowData = new RowData(rowInfo, Parent);
                RowDatas.Add(rowData);
            }
        }
    }

    /// <summary>
    /// 一行数据
    /// </summary>
    public class RowData
    {
        public List<ItemData> Items;
        public RuleInfo Parent;
        public float Spacing = 40;

        public RowData(IList<object> rowInfo, RuleInfo parent)
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
                if (itemData.ShowType < 0) continue;
                Items.Add(itemData);
            }
        }

        public RowData(IDictionary<string, object> rowInfo, RuleInfo parent)
        {
            Items = new List<ItemData>();
            Parent = parent;
            if (rowInfo == null)
            {
                Items = null;
                return;
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
                if (itemData.ShowType < 0) continue;
                Items.Add(itemData);
            }
        }
    }

    /// <summary>
    /// 一行数据item
    /// </summary>
    public class ItemData
    {
        public int Index;
        /// <summary>
        /// id号，唯一识别
        /// </summary>
        public string Id;
        /// <summary>
        /// 名字
        /// </summary>
        public string Name;
        /// <summary>
        /// 类型
        /// </summary>
        public RuleItemType Type;
        /// <summary>
        /// 发送给服务器的key
        /// </summary>
        public string Key;
        /// <summary>
        /// 当前的状态
        /// </summary>
        public bool State;
        /// <summary>
        /// 发送给服务器的对应的值
        /// </summary>
        public string Value;
        /// <summary>
        /// label的宽度
        /// </summary>
        public int Width;
        /// <summary>
        /// label的高度
        /// </summary>
        public int Height;
        /// <summary>
        /// 消耗品
        /// </summary>
        public string UseItem;
        /// <summary>
        /// 消耗品的数量
        /// </summary>
        public int UseNum;
        /// <summary>
        /// 选择的小组，实现单选
        /// </summary>
        public int Group;
        /// <summary>
        /// 依赖选项，选择该项，会选择依赖项
        /// </summary>
        public string[] Dp;
        /// <summary>
        /// 互斥，选择该项，会取消互斥项
        /// </summary>
        public string[] Ep;
        /// <summary>
        /// 前置关联，取消该项，会取消前置关联项
        /// </summary>
        public string[] Rp;
        /// <summary>
        /// 显示类型 0 都显示，大于0：前台，小于0：后台
        /// </summary>
        public int ShowType;
        /// <summary>
        /// 提示
        /// </summary>
        public string Tip;
        /// <summary>
        /// 单击事件
        /// </summary>
        public ClickTypeData ClickType;
        public RuleInfo Parent;
        public NguiCRComponent View;
        public ItemData(int index, object item, RuleInfo parent)
        {
            Parent = parent;
            Index = index;
            var itemDict = item as Dictionary<string, object>;
            if (itemDict == null)
            {
                Id = "";
                Name = "";
                Type = RuleItemType.none;
                Key = "";
                State = false;
                Value = "";
                Width = 0;
                Height = 0;
                UseItem = "";
                UseNum = 0;
                Group = 0;
                Dp = null;
                Ep = null;
                Rp = null;
                Tip = "";
                return;
            }
            Id = itemDict.ContainsKey("id") ? itemDict["id"].ToString() : "";
            Name = itemDict.ContainsKey("name") ? itemDict["name"].ToString() : "";
            Key = itemDict.ContainsKey("key") ? itemDict["key"].ToString() : "";
            State = itemDict.ContainsKey("state") && bool.Parse(itemDict["state"].ToString());
            Value = itemDict.ContainsKey("value") ? itemDict["value"].ToString() : "";
            Width = itemDict.ContainsKey("width") ? int.Parse(itemDict["width"].ToString()) : 0;
            Height = itemDict.ContainsKey("height") ? int.Parse(itemDict["height"].ToString()) : 0;
            UseItem = itemDict.ContainsKey("useitem") ? itemDict["useitem"].ToString() : "";
            UseNum = itemDict.ContainsKey("usenum") ? int.Parse(itemDict["usenum"].ToString()) : 0;
            Group = itemDict.ContainsKey("group") ? int.Parse(itemDict["group"].ToString()) : 0;
            Dp = itemDict.ContainsKey("dp") ? itemDict["dp"].ToString().Split(',') : new string[0];
            Ep = itemDict.ContainsKey("ep") ? itemDict["ep"].ToString().Split(',') : new string[0];
            Rp = itemDict.ContainsKey("rp") ? itemDict["rp"].ToString().Split(',') : new string[0];
            ShowType = itemDict.ContainsKey("showtype") ? int.Parse(itemDict["showtype"].ToString()) : 0;
            Type = itemDict.ContainsKey("type") ? (RuleItemType)Enum.Parse(typeof(RuleItemType), itemDict["type"].ToString()) : RuleItemType.none;
            Tip = itemDict.ContainsKey("tip") ? itemDict["tip"].ToString() : "";

            if (itemDict.ContainsKey("hide"))
            {
                parent.Tabs[Id] = itemDict["hide"].ToString().Split(',');
            }
            if (Type != RuleItemType.none)
            {
                parent.CreateArgs[Id] = this;
                if (Type == RuleItemType.button)
                {
                    var tabIndex = parent.TabDefaultIndex;
                    if (tabIndex > -1 ? tabIndex == Index : State)
                    {
                        parent.CurTabItemId = Id;
                    }
                }
                else
                {
                    State = RuleInfo.GetItemState(parent.CurTabItemId, Id, State);
                }
            }
            if (itemDict.ContainsKey("clicktype"))
            {
                var info = itemDict["clicktype"].ToString();
                var infos = info.Split('|');
                if (infos.Length > 0)
                {
                    ClickType = new ClickTypeData
                    {
                        Type = (ECLickType)Enum.Parse(typeof(ECLickType), infos[0]),
                        Parm = infos.Length > 1 ? infos[1].Split(','):new string[0]
                    };
                }
            }
        }
    }

    public enum RuleItemType
    {
        none,
        checkbox,
        button,
        radio,
        list,
        range,
        label
    }

    public class ClickTypeData
    {
        public ECLickType Type;
        public string[] Parm;
    }

    public enum ECLickType
    {
        Show = 0
    }
}
