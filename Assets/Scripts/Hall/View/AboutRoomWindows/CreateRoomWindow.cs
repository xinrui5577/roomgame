using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Assets.Scripts.Common.UI;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows.TabPages;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Manager;
using YxFramwork.View;
using com.yxixia.utile.YxDebug;
using fastJSON;

namespace Assets.Scripts.Hall.View.AboutRoomWindows
{
    public class CreateRoomWindow : YxTabPageWindow
    {
        /// <summary>
        /// 我的房卡Label
        /// </summary>
        [Tooltip("我的房卡Label")]
        public UILabel MyRcCountLabel;
        /// <summary>
        /// 房间玩法标签页视图预制体
        /// </summary>
        [Tooltip("房间玩法标签页视图预制体")]
        public CreateRoomItemView RoomItemViewPerfab;
        /// <summary>
        /// 房间玩法标签页视图Grid预制体
        /// </summary>
        [Tooltip("房间玩法标签页视图Grid预制体")]
        public Transform RoomItemParentPerfab;
        public UIScrollView ScrollView;
        public string GameKey;
        public bool IsDesignated;
        /// <summary>
        /// 标签默认索引
        /// </summary>
        public int TabDefaultIndex = -1;
        /// <summary>
        /// 全选字段
        /// </summary>
        [Tooltip("全选字段")]
        public string AllField = "-all";
        [Tooltip("将上次创建的游戏显示在第一个")]
        public bool SetFirst = false;
        protected Transform _roomItemParent;
        protected RuleInfo _curRuleInfo;

        protected override void OnStart()
        {
            RoomListController.Instance.GetCreateRoomParm(IsDesignated ? "" : GameKey, UpdateView);
            if (MyRcCountLabel != null) MyRcCountLabel.text = UserInfoModel.Instance.BackPack.GetItem("item2_q").ToString(CultureInfo.InvariantCulture);
        }

        protected override void OnFreshView()
        {
            YxWindowManager.HideWaitFor();
            if (Data == null) return;
            if (Data is IList)
            {
                OnRefreshOne();//标签
            }
            else
            {
                OnRefreshAll(); //创建信息
            }
            SetLastGameOnFirst(SetFirst);
            UpdateTabs(TabDatas);
        }


        private void OnRefreshAll()
        {
            var dict = GetData<Dictionary<string, object>>();
            if (dict == null) return;
            if (!dict.ContainsKey("tabs")) return;
            var tabs = dict["tabs"];
            if (!(tabs is List<object>)) return;
            var tabList = tabs as List<object>;

            var count = tabList.Count;

            TabDatas = new TabData[count];
            for (var i = 0; i < count; i++)
            {
                var temp = tabList[i];
                if (!(temp is Dictionary<string, object>)) continue;
                var infoDict = temp as Dictionary<string, object>;
                var tabData = new TabData { Index = i };
                if (infoDict.ContainsKey("name"))
                {
                    var obj = infoDict["name"];
                    if (obj != null)
                    {
                        tabData.Name = obj.ToString();
                    }
                }
                if (infoDict.ContainsKey("id"))
                {
                    var obj = infoDict["id"];
                    if (obj != null)
                    {
                        var tid = obj.ToString();
                        tabData.Data = tid;
                        _tabWithData[tid] = tabData;
                        var ainfo = tid.Split('_');
                        if (ainfo.Length > 0)
                        {
                            tabData.StarttingState = ainfo[0] == GameKey;
                        }
                    }
                }
                TabDatas[i] = tabData;
            }
        }

        private readonly Dictionary<string, TabData> _tabWithData = new Dictionary<string, TabData>();
        private void OnRefreshOne()
        {
            var list = GetData<List<object>>();
            if (list == null) return;
            var listCount = list.Count;
            TabDatas = new TabData[listCount];
            for (var i = 0; i < listCount; i++)
            {
                var info = list[i];
                var dict = info as IDictionary;
                if (dict == null) continue;

                var ruleInfo = new RuleInfo(dict, TabDefaultIndex);

                var tabData = new TabData
                {
                    Name = ruleInfo.Name,
                    Data = ruleInfo,
                    Index = i
                };
                TabDatas[i] = tabData;
            }
        }

        protected string _selectedGameKey;
        protected string _selectedRuleId;
        protected string _selectedGameName;
        protected UIToggle _selectTabToggle;
        public void OnChangeTabWithToggle(UIToggle toggle)
        {
            if (ScrollView != null) ScrollView.ResetPosition();
            _selectedGameKey = "";
            _selectedGameName = "";
            if (!toggle.value) return;
            _selectTabToggle = toggle;
            int index;
            int.TryParse(toggle.name, out index);
            OnChangeTab(index);
        }

        private void OnChangeTab(int index)
        {
            YxWindowUtils.CreateItemParent(RoomItemParentPerfab, ref _roomItemParent, RoomItemParentPerfab.parent);
            if (index >= TabDatas.Length) return;
            var tabData = TabDatas[index];
            if (tabData == null) return;
            var itemView = YxWindowUtils.CreateItem(RoomItemViewPerfab, _roomItemParent);
            var itemData = tabData.Data;
            if (itemData == null) return;
            if (itemData is RuleInfo)
            {
                var data = itemData as RuleInfo;
                UpdateItemView(data, itemView);
                return;
            }
            RoomListController.Instance.GetCreateRoomParm(itemData.ToString(), obj =>
            {
                if (!(obj is List<object>)) return;
                var list = (List<object>) obj;
                foreach (var itemObj in list)
                {
                    if (!(itemObj is Dictionary<string, object>)) continue;
                    var itemDict = itemObj as Dictionary<string, object>;
                    var ruleInfo = new RuleInfo(itemDict, TabDefaultIndex);
                    if (!_tabWithData.ContainsKey(ruleInfo.Id)) continue;
                    var tbData = _tabWithData[ruleInfo.Id];
                    tbData.Data = ruleInfo;
                }
                itemData = tabData.Data;
                if (!(itemData is RuleInfo)) return;
                var data = itemData as RuleInfo;
                UpdateItemView(data, itemView);
            });
        }

        private void UpdateItemView(RuleInfo info, YxView itemView)
        {
            if (info == null) return;
            _selectedGameKey = info.GameKey;
            _selectedRuleId = info.Id;
            _curRuleInfo = info;
            _selectedGameName = info.Name;
            itemView.UpdateView(info);
        }

        public virtual void CreateRoom()
        {
            if (_curRuleInfo == null) return;
            var cArgs = _curRuleInfo.CreateArgs;
            var useTyp = "";
            var useNum = 0;
            var args = AnalyzeArgs(cArgs, ref useTyp, ref useNum);// LPlayerCheckItem.GetCargs();
            //if (!string.IsNullOrEmpty(useTyp) && useNum > 0 && !CheckConsumeCount(useNum, useTyp))
            //{
            //    YxMessageBox.Show("消耗品不足，请购买后再创建房间");
            //    return;
            //}

            if (!string.IsNullOrEmpty(_selectedGameKey))
            {
                var curGameKey = _selectedGameKey;
                App.GameKey = curGameKey;
            }
            var data = GetParm(args, useNum);
            SaveLastGame();
            SendCreateRoom(data);
        }

        protected virtual Dictionary<string, object> GetParm(string args, int useNum)
        {
            var data = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(args))
            {
                data["cargs"] = args;
            }
            data["ruleId"] = _selectedRuleId;
            data["num"] = useNum.ToString(CultureInfo.InvariantCulture);
            return data;
        }

        /// <summary>
        /// 发送创建房间消息
        /// </summary>
        /// <param name="data"></param>
        protected virtual void SendCreateRoom(Dictionary<string, object> data)
        {
            RoomListController.Instance.CreatRoom(data);
        }

        /// <summary>
        /// 获取选择的参数
        /// </summary>
        /// <param name="cArgs"></param>
        /// <param name="useItem"></param>
        /// <param name="useNum"></param>
        /// <returns></returns>
        protected string AnalyzeArgs(IDictionary<string, ItemData> cArgs, ref string useItem, ref int useNum)
        {
            var args = "";
            var sign = "";
            if (cArgs.ContainsKey(AllField))
            {
                cArgs.Remove(AllField);
            }
            foreach (var pair in cArgs)
            {
                var createData = pair.Value;
                var view = createData.View;
                if (view == null) continue;

                var state = view.IsValid();
                if (createData.Type != RuleItemType.button) RuleInfo.SaveItemState(createData.Parent.CurTabItemId, createData.Id, state);
                if (!state) continue;
                var key = createData.Key;
                if (key == AllField) continue;

                args = string.Format("{0}{1}{2},{3}", args, sign, createData.Key, createData.Value);
                sign = ",";
                var expnum = createData.UseNum;
                if (expnum <= 0) continue;
                useNum = expnum;
                useItem = createData.UseItem;
            }
            YxDebug.Log(args);
            return args;
        }

        protected static bool CheckConsumeCount(int need, string type)
        {
            var userInfo = UserInfoModel.Instance.UserInfo;
            switch (type)
            {
                case "":
                    return true;
                case "1":
                    return userInfo.CoinA >= need;
                case "2":
                    return userInfo.CashA >= need;
                case "4":
                    var num2 = UserInfoModel.Instance.BackPack.GetItem("item2_q");
                    return num2 >= need;
                case "coin_q":
                case "coin_a":
                    return userInfo.CoinA >= need;
                case "cash_q":
                case "cash_a":
                    return userInfo.CashA >= need;
                case "3":
                case "coupon_q":
                case "coupon_a":
                    return userInfo.CouponA >= need;
                default:
                    var num = UserInfoModel.Instance.BackPack.GetItem(type);
                    return num >= need;
            }
        }

        /// <summary>
        /// 选项更改
        /// </summary>
        /// <param name="view"></param>
        public void OnOptionClick(NguiCheckBox view)
        {
            if (_curRuleInfo == null) return;
            var id = view.Id;
            var cArgs = _curRuleInfo.CreateArgs;
            if (!cArgs.ContainsKey(id)) return;
            var cData = cArgs[id];
            if (cData == null) return;
            var type = cData.Type;
            var isSelected = view.Toggle.value;
            if (isSelected)
            {
                if (type == RuleItemType.button)
                {
                    if (cData.Id == _curRuleInfo.CurTabItemId) return;
                    _curRuleInfo.CurTabItemId = cData.Id;
                    if (_selectTabToggle != null) OnChangeTabWithToggle(_selectTabToggle);
                    return;
                }
                ExecuteWithClickType(cData,cArgs);
                UpdateOtherOptions(cData.Dp, cArgs, true);
                UpdateOtherOptions(cData.Ep, cArgs, false);
            }
            else
            {
                UpdateOtherOptions(cData.Rp, cArgs, false);
            }
            if (cData.Key == AllField) return;
            var allCreateData = GetAllRp(cArgs);
            UpdataAllState(allCreateData, cArgs);
        }

        private void ExecuteWithClickType(ItemData data, Dictionary<string, ItemData> dict)
        {
            if (data == null) { return;}
            var clickTypeData = data.ClickType;
            if (clickTypeData == null) { return;}
            switch (clickTypeData.Type)
            {
                case ECLickType.Show: 
                    var parm = clickTypeData.Parm;
                    if (parm != null && parm.Length>1)
                    {
                        var id = parm[0];
                        if (dict.ContainsKey(id))
                        {
                            var itemData = dict[id];
                            var view = itemData.View;
                            if (view != null)
                            {
                                itemData.Value = parm[1];
                                view.UpdateView(itemData);
                            }
                        }
                    }
                    break;
            }
        }

        private ItemData GetAllRp(Dictionary<string, ItemData> cArgs)
        {
            var pair = cArgs.Values;
            foreach (var createData in pair)
            {
                if (createData.Key != AllField) continue;
                return createData;
            }
            return null;
        }

        private void UpdataAllState(ItemData data, IDictionary<string, ItemData> cArgs)
        {
            if (data == null) return;
            var toggle = ((NguiCheckBox)data.View).Toggle;
            toggle.value = false;
            var option = data.Dp;
            if (option == null) return;
            var count = option.Length;
            for (var i = 0; i < count; i++)
            {
                var dpId = option[i];
                if (!cArgs.ContainsKey(dpId)) continue;
                var otherData = cArgs[dpId];
                var dpView = otherData.View;
                if (dpView == null) continue;
                if (((NguiCheckBox)dpView).Toggle.value) continue;
                return;
            }
            toggle.value = true;
        }

        private void UpdateOtherOptions(IList<string> option, IDictionary<string, ItemData> cArgs, bool isSelected)
        {
            if (option == null) return;
            var count = option.Count;
            for (var i = 0; i < count; i++)
            {
                var dpId = option[i];
                if (!cArgs.ContainsKey(dpId)) continue;
                var otherData = cArgs[dpId];
                var dpView = otherData.View;
                if (dpView == null) continue;
                var nguiCb = (NguiCheckBox)dpView;
                nguiCb.Toggle.value = isSelected;
            }
        }


        public void OnHelpClick()
        {
            if (_selectTabToggle == null) return;
            int index;
            if (!int.TryParse(_selectTabToggle.name, out index)) return;
            if (index >= TabDatas.Length) return;
            var tabDataObj = TabDatas[index];
            if (tabDataObj == null) return;
            var data = tabDataObj.Data as RuleInfo;
            if (data == null) return;

            var win = CreateChildWindow("DefRuleWindow");
            if (win == null) return;
            var ruleWindow = win.gameObject.GetComponent<RuleWindow>();
            ruleWindow.SetFirstTab(data.GameKey);
        }

        #region 将上次创建的游戏显示在第一个
        [Tooltip("本地记录保存上次游戏的")]
        public string KeySaveGame = "GameItem";
        /// <summary>
        /// 当前玩家当前应用中最后一个游戏的存储字段
        /// </summary>
        protected string SaveKey
        {
            get
            {
                return string.Format("{0}_{1}_{2}", Application.bundleIdentifier, App.UserId, KeySaveGame);
            }
        }
        private void SetLastGameOnFirst(bool state)
        {
            if (state)
            {
                string lastGame = PlayerPrefs.GetString(SaveKey);
                if (string.IsNullOrEmpty(lastGame))
                {
                    return;
                }
                List<TabData> datas = TabDatas.ToList();
                int index = datas.FindIndex(item => item.Name.Equals(lastGame));
                if (index > 0)
                {
                    TabData cacheTab = TabDatas[0];
                    TabDatas[0] = TabDatas[index];
                    TabDatas[index] = cacheTab;
                }
            }
        }

        private void SaveLastGame()
        {
            PlayerPrefs.SetString(SaveKey, _selectedGameName);
        }
        #endregion
    }

}
