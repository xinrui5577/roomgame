using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts.Common.UI;
using UnityEngine;

namespace Assets.Scripts.Common.Models.CreateRoomRules
{
    /// <summary>
    /// 组件数据item
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
        /// 选项内容
        /// </summary>
        public string Options;
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
        public float UseNum;
        /// <summary>
        /// 选择的小组，实现单选
        /// </summary>
        public int Group;
        /// <summary>
        /// 分段数组
        /// </summary>
        public string[] Range;
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
        /// 字体大小
        /// </summary>
        public int Size;
        /// <summary>
        /// 提示
        /// </summary>
        public string Tip;
        /// <summary>
        /// 需要转换
        /// </summary>
        public bool NeedConvert;
        /// <summary>
        /// Slider类型 
        /// </summary>
        public YxECrSliderType SliderType;
        /// <summary>
        /// slider类型为float的时候，保留小数点位数
        /// </summary>
        public int FloatValidity;
        /// <summary>
        /// 输入类型
        /// </summary>
        public UIInput.Validation Validation;

        public string[] HideIds { get; private set; }

        /// <summary>
        ///  
        /// </summary>
        public ClickTypeData ClickType; 
        public CreateRoomRuleInfo Parent;
        public NguiCRComponent View;
        /// <summary>
        /// 默认选项
        /// </summary>
        public bool DefaultState;

        public ItemData(int index, object item, CreateRoomRuleInfo parent)
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
            DefaultState = State = itemDict.ContainsKey("state") && itemDict["state"].ToString().ToLower() == "true";
            NeedConvert = itemDict.ContainsKey("needConvert") && itemDict["needConvert"].ToString().ToLower() == "true";
            Options = itemDict.ContainsKey("options") ? itemDict["options"].ToString() : "";
            Value = itemDict.ContainsKey("value") ? itemDict["value"].ToString() : "";
            Width = itemDict.ContainsKey("width") ? int.Parse(itemDict["width"].ToString()) : 0;
            Height = itemDict.ContainsKey("height") ? int.Parse(itemDict["height"].ToString()) : 0;
            UseItem = itemDict.ContainsKey("useitem") ? itemDict["useitem"].ToString() : "";

            if (itemDict.ContainsKey("shownum"))
            {
                UseNum = float.Parse(itemDict["shownum"].ToString());
            }
            else
            {
                UseNum = itemDict.ContainsKey("usenum") ? int.Parse(itemDict["usenum"].ToString()) : 0;
            }
            Group = itemDict.ContainsKey("group") ? int.Parse(itemDict["group"].ToString()) : 0;
            Size = itemDict.ContainsKey("fontSize") ? int.Parse(itemDict["fontSize"].ToString()) : 0;
            Dp = itemDict.ContainsKey("dp") ? itemDict["dp"].ToString().Split(',') : new string[0];
            Ep = itemDict.ContainsKey("ep") ? itemDict["ep"].ToString().Split(',') : new string[0];
            Rp = itemDict.ContainsKey("rp") ? itemDict["rp"].ToString().Split(',') : new string[0];
            Range = itemDict.ContainsKey("range") ? itemDict["range"].ToString().Split(',') : null;
            ShowType = itemDict.ContainsKey("showtype") ? int.Parse(itemDict["showtype"].ToString()) : 0;
            Type = itemDict.ContainsKey("type") ? (RuleItemType)Enum.Parse(typeof(RuleItemType), itemDict["type"].ToString()) : RuleItemType.none;
            Validation = itemDict.ContainsKey("validation") ? (UIInput.Validation)Enum.Parse(typeof(UIInput.Validation), itemDict["validation"].ToString()) : UIInput.Validation.None;
            SliderType = itemDict.ContainsKey("sliderType") ? (YxECrSliderType)Enum.Parse(typeof(YxECrSliderType), itemDict["sliderType"].ToString()) : YxECrSliderType.Integer;
            Tip = itemDict.ContainsKey("tip") ? itemDict["tip"].ToString() : "";
            FloatValidity = itemDict.ContainsKey("floatValidity") ? int.Parse(itemDict["floatValidity"].ToString()) : 2;

            if (Type == RuleItemType.tab)
            {
                if (parent.HasServerData(Id))
                {
                    CreateRoomRuleInfo.SaveItemState(null, Id, Group, parent.GameKey, true);
                }
                if (!CreateRoomRuleInfo.NeedRecord)
                {
                    CreateRoomRuleInfo.SaveItemState(parent.CurTabId, Id, Group, parent.GameKey, State);
                }
                State = CreateRoomRuleInfo.GetItemState(null, Id, Group, parent.GameKey, State);
                if (State)
                {
                    parent.CurTabId = Id;
                    parent.SetButtonId(Group, Id);
                }
            }
            if (itemDict.ContainsKey("hide"))
            {
                HideIds = itemDict["hide"].ToString().Split(',');
                parent.AddHideIds(Id, HideIds);
            }
            else
            {
                HideIds = new string[0];
            }

            //                Debug.Log(string.Format("--------->:{0},{1},{2},{3}", Id,Group,Name,State));
            if (itemDict.ContainsKey("clicktype"))
            {
                var info = itemDict["clicktype"].ToString();
                var infos = info.Split('|');
                if (infos.Length > 0)
                {
                    ClickType = new ClickTypeData
                    {
                        Type = (ECLickType)Enum.Parse(typeof(ECLickType), infos[0]),
                        Parm = infos.Length > 1 ? infos[1].Split(',') : new string[0]
                    };
                }
            }
        }
         
        public void FreshHides()
        { 
            if (HideIds.Length>0)
            {
                foreach (var hideId in HideIds)
                {
                    var itemData = Parent.GetItemData(hideId); 
                    if(itemData == null || itemData.Group < 1)
                    {
                        continue;
                    }
                    var curTablId = Parent.CurTabId;
                    if (CreateRoomRuleInfo.GetItemState(curTablId, itemData.Id, itemData.Group, Parent.GameKey, false))
                    {
                        CreateRoomRuleInfo.RemoveItemState(curTablId, itemData.Id, itemData.Group, Parent.GameKey);
                    }
                }
            }
        }
    }


    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum RuleItemType
    {
        /// <summary>
        /// 无
        /// </summary>
        none,
        /// <summary>
        /// 多选框
        /// </summary>
        checkbox,
        /// <summary>
        /// 按钮
        /// </summary>
        button,
        /// <summary>
        /// 单选框
        /// </summary>
        radio,
        /// <summary>
        /// 下拉列表
        /// </summary>
        pop,
        /// <summary>
        /// 范围选择框
        /// </summary>
        slider,
        /// <summary>
        /// 标签页
        /// </summary>
        tab,
        /// <summary>
        /// 文本
        /// </summary>
        label,
        /// <summary>
        /// 输入框
        /// </summary>
        input
    }

    public class ClickTypeData
    {
        public ECLickType Type;
        public string[] Parm;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ECLickType
    {
        Show = 0
    }

    public enum YxECrSliderType
    {
        Integer,
        Float
    }
}
