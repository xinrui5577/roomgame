using System;
using System.Collections;
using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;

namespace Assets.Scripts.Common.Models.CreateRoomRules
{
    /// <summary>
    /// 创建房间规则数据
    /// </summary>
    public class CreateRoomRuleInfo
    {
        /// <summary>
        /// 是否记录
        /// </summary>
        public static bool NeedRecord;
        public const string PrefsKey = "RuleInfo";

        public string Id;
        public string Name;
        public string Key;
        public string GameKey;
        public string Type;
        public string CurTabId;
        public List<GroupData> GroupDatas;
        public List<GroupRowData> RowData;
        /// <summary>
        /// 隐藏信息
        /// </summary>
        public readonly Dictionary<string, List<string>> HideDict = new Dictionary<string, List<string>>();
        public readonly Dictionary<string, ItemData> CreateArgs = new Dictionary<string, ItemData>();
        private readonly Dictionary<string,string> _lastSaveDate = new Dictionary<string, string>();
        //        public string CurTabItemId = "";
        private readonly Dictionary<int, string> _buttonGroupDict = new Dictionary<int, string>();
        private readonly Dictionary<string,ItemData> _itemDict = new Dictionary<string, ItemData>();
        public int TabDefaultIndex;
        public CreateRoomRuleInfo(IDictionary dict, int defuatIndex = -1)
        {
            TabDefaultIndex = defuatIndex;
            _radioSelected.Clear();
            GroupDatas = new List<GroupData>();
            RowData = new List<GroupRowData>();
            if (dict == null)
            {
                Id = "";
                Name = "";
                Key = "";
                GameKey = "";
                Type = "";
                return;
            }
            InitLastData(dict);
            Id = dict.Contains("id") ? dict["id"].ToString() : "";
            Name = dict.Contains("name") ? dict["name"].ToString() : "";
            Key = dict.Contains("key") ? dict["key"].ToString() : "";
            GameKey = dict.Contains("gamekey") ? dict["gamekey"].ToString() : "";
            Type = dict.Contains("type") ? dict["type"].ToString() : "";
            if (dict.Contains("rule"))
            {
                SetRules(dict["rule"]);
                FreshItemSelected();
            }
            else if (dict.Contains("rows"))
            {
                var rows = dict["rows"] as List<object>;
                SetRowData(rows);
                FreshItemSelected();
            }
        }

        private void FreshItemSelected()
        { 
            FreshItemData(item =>
            {
                var type = item.Type;
                var itemId = item.Id;
                switch (type)
                {
                    case RuleItemType.none:
                    case RuleItemType.label:
                        break; 
                    case RuleItemType.button:
                        if (HasServerData(itemId))
                        {
                            SaveItemState(CurTabId, itemId, item.Group, GameKey, true);
                        }
                        else
                        {
                            if (!NeedRecord)
                            {
                                SaveItemState(CurTabId, itemId, item.Group, GameKey, item.State);
                            }
                            item.State = GetItemState(CurTabId, itemId, item.Group, GameKey, item.State);
                            if (item.State)
                            {
                                SetButtonId(item.Group, itemId);
                            }
                        }
                        break;
                    case RuleItemType.input:
                    case RuleItemType.pop:
                    case RuleItemType.slider:
                        if (HasServerData(itemId))
                        {
                            SaveItemValue(CurTabId, itemId, GameKey, GetServerData(Id));
                        }
                        else
                        {
                            if (!NeedRecord)
                            {
                                SaveItemValue(CurTabId, itemId, GameKey, item.Value);
                            }
                            item.Value = GetItemValue(CurTabId, itemId, GameKey, item.Value);
                        }
                        break;
                    default:
                        //                    parent.CreateArgs[Id] = this;
                        if (HasServerData(itemId))
                        {
                            SaveItemState(CurTabId, itemId, item.Group, GameKey, GetServerData(itemId) == item.Value);
                        }
                        else
                        {
                            if (!NeedRecord)
                            {
                                //Debug.LogError(string.Format("{5}--->CurTabId:{0}, itemId:{1}, item.Group:{2}, GameKey:{3}, item.State:{4}",CurTabId, itemId, item.Group, GameKey, item.State, item.Name));
                                SaveItemState(CurTabId, itemId, item.Group, GameKey, item.State);
                            }
                            item.State = GetItemState(CurTabId, itemId, item.Group, GameKey, item.State);
                        }
                        break;
                }
            });
        }
//        

        private void FreshItemData(Action<ItemData> callBack)
        {
            foreach (var kv in _itemDict)
            {
                var item = kv.Value;
                callBack(item);
            }
        }

      
        private void InitLastData(IDictionary dict)
        {
            var lastData = "";
            if (dict.Parse("cargs", ref lastData))
            {
                var arr = lastData.Split(',');
                var count = arr.Length-1;
                for (var i = 0; i < count; i++)
                {
                    var key = arr[i]; 
                    _lastSaveDate[key] = arr[i + 1];
                }
            }
        }

        private void UpdateRadios()
        {
            if (RowData == null) return;
            foreach (var rowData in GroupDatas)
            {
                var groupDatas = rowData.RowDatas;
                foreach (var gd in groupDatas)
                {
                    var items = gd.Items;
                    foreach (var item in items)
                    {
                        if (item.Type == RuleItemType.radio)
                        {
                            var id = item.Id;
                            var group = item.Group;
                            if (ViewIsHide(id))
                            {
                                item.State = false; 
                            }
                            else
                            {
                                var state = GetItemState(CurTabId, id, group, GameKey, item.DefaultState);
                                
                                if (state)
                                {
                                    AddRadioSelected(group, id);
                                }
                                else if (!HasRadioSelected(group))
                                { 
                                    AddRadioSelected(group, id);
                                }
                                item.State = state;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="ruleData"></param>
        private void SetRowData(List<object> ruleData)
        {
            if (ruleData == null) return;
            var count = ruleData.Count;
            for (var i = 0; i < count; i++)
            {
                var row = ruleData[i] as Dictionary<string,object>;
                if(row == null) { continue;}
                var rowData = new GroupRowData(row);
                RowData.Add(rowData);
            } 
        }


        private void SetRules(object groupObj)
        {
            if (groupObj == null) { return;}
            var groupDict = groupObj as Dictionary<string, object>;
            //旧版结构
            if (groupDict != null)
            { 
                //if (groupDict == null) return;
                GroupDatas.Clear();
                foreach (var groups in groupDict)
                {
                    var groupData = new GroupData(groups, this);
                    GroupDatas.Add(groupData);
                }
                return;
            }
            var groupList = groupObj as List<object>;
            //新版结构
            if (groupList==null) return;
            GroupDatas.Clear();
            foreach (var groupInfo in groupList)
            {
                var group = groupInfo as Dictionary<string, object>;
                if (group == null) continue;
                var groupData = new GroupData(group, this);
                GroupDatas.Add(groupData);
            }
        }

        public static string GetPrefsKey(string tabId, string id, int group, string gameKey)
        {
            if (string.IsNullOrEmpty(tabId))
            {
                return group > 0 ? 
                    string.Format("{0}_{1}_group_{2}", PrefsKey, gameKey, group):
                    string.Format("{0}_{1}_id_{2}", PrefsKey, gameKey, id); //RuleInfo_gameKey_group
            }
            return group > 0 ?
                string.Format("{0}_{1}_{2}_group_{3}", PrefsKey, gameKey, tabId, group) ://RuleInfo_gameKey_tableId_id
                string.Format("{0}_{1}_{2}_id_{3}", PrefsKey, gameKey, tabId, id);//RuleInfo_gameKey_tableId_id
        }

        /// <summary>
        ///  获取状态
        /// </summary>
        /// <param name="tabId"></param>
        /// <param name="id"></param>
        /// <param name="group"></param>
        /// <param name="gameKey"></param>
        /// <param name="defaultState"></param>
        /// <returns></returns>
        public static bool GetItemState(string tabId, string id, int group, string gameKey, bool defaultState)
        {
            var prefsId = GetPrefsKey(tabId, id, group, gameKey);
            if (!PlayerPrefs.HasKey(prefsId))
            {
                return defaultState;
            }
            var old = PlayerPrefs.GetString(prefsId);
            //            if(id == "biji_13") Debug.LogError(string.Format("取出：{0} | {1} | {2} | {3} = {4}", prefsId, id, group, gameKey, old));
            return old == id;
        }

        /// <summary>
        /// 保存 单选、多选 值
        /// </summary>
        /// <param name="tabId"></param>
        /// <param name="id"></param>
        /// <param name="group"></param>
        /// <param name="gameKey"></param>
        /// <param name="state"></param>
        public static void SaveItemState(string tabId, string id, int group, string gameKey, bool state)
        {
            if (group > 0 && !state) return;
            var prefsId = GetPrefsKey(tabId, id, group, gameKey);
            //            if (id == "biji_13") Debug.LogError(string.Format("保存：{0} | {1} | {2} | {3}", prefsId, id, group, gameKey));
            PlayerPrefs.SetString(prefsId, state ? id : null);
        }

        public static void RemoveItemState(string tabId, string id, int group, string gameKey)
        {
            var prefsId = GetPrefsKey(tabId, id, group, gameKey);
            if (PlayerPrefs.HasKey(prefsId))
            {
                PlayerPrefs.DeleteKey(prefsId);
            }
        }


        /// <summary>
        /// 获取 value
        /// </summary>
        /// <param name="tabId"></param>
        /// <param name="id"></param> 
        /// <param name="gameKey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetItemValue(string tabId, string id, string gameKey, string defaultValue)
        {
            var prefsId = GetPrefsKey(tabId, id, 0, gameKey);
            var old = PlayerPrefs.GetString(prefsId, defaultValue);
            return old;
        }
        /// <summary>
        /// 保存 value
        /// </summary>
        /// <param name="tabId"></param>
        /// <param name="id"></param> 
        /// <param name="gameKey"></param>
        /// <param name="value"></param>
        public static void SaveItemValue(string tabId, string id, string gameKey, string value)
        {
            var prefsId = GetPrefsKey(tabId, id, 0, gameKey);
            //            Debug.Log(id + " " + group + " " + gameKey + " " + state);
            //Debug.Log(string.Format("保存：{0},{1},{2}", prefsId, id, value));
            PlayerPrefs.SetString(prefsId, value);
        }

        public string CurrentButtonId(int group)
        {
            return _buttonGroupDict.ContainsKey(group) ? _buttonGroupDict[group] : "";
        }

        public void SetButtonId(int group, string id)
        {
            //            Debug.Log(group+ "," + id);
            _buttonGroupDict[group] = id;
        }

        /// <summary>
        /// 是否隐藏
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public bool ViewIsHide(string itemId)
        {
            if (!HideDict.ContainsKey(itemId)) return false;
            var list = HideDict[itemId]; 
            foreach (var btnId in list)
            {
                foreach (var keyValue in _buttonGroupDict)
                {
                    if (keyValue.Value == btnId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 添加隐藏id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="hideIds"></param>
        public void AddHideIds(string id, string[] hideIds)
        {
            foreach (var hideId in hideIds)
            {
                var list = HideDict.ContainsKey(hideId) ? HideDict[hideId] : HideDict[hideId] = new List<string>();
                list.Add(id);
            }
        }

        private readonly Dictionary<int, string> _radioSelected = new Dictionary<int, string>();

        public void AddRadioSelected(int group,string itemId)
        {
            _radioSelected[group] = itemId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public string GetRadioSelected(int group)
        {
            return _radioSelected.ContainsKey(group) ? _radioSelected[group] : string.Empty;
        }

        public bool HasRadioSelected(int group)
        {
            return _radioSelected.ContainsKey(group);
        }

        public void Reset()
        {
            CreateArgs.Clear();
            _radioSelected.Clear();
            UpdateRadios();
        }

        public bool HasServerData(string id)
        {
            return _lastSaveDate.ContainsKey(id);
        }

        public string GetServerData(string id)
        {
            return _lastSaveDate[id];
        }

        public void AddItemData(string itemDataId, ItemData itemData)
        {
            _itemDict[itemDataId] = itemData;
        }

        public ItemData GetItemData(string itemDataId)
        {
            return _itemDict.ContainsKey(itemDataId) ? _itemDict[itemDataId] : null;
        }

        public bool HasServerSaveOption()
        {
            return _lastSaveDate.Count > 0;
        }
    }
}
