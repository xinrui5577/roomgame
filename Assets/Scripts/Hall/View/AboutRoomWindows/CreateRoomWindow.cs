using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assets.Scripts.Common.Models.CreateRoomRules;
using Assets.Scripts.Common.UI;
using Assets.Scripts.Common.Windows.TabPages;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework;
using com.yxixia.utile.YxDebug;
using YxFramwork.Enums;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View.AboutRoomWindows
{
    /// <summary>
    /// 创建房间窗口
    /// </summary>
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
        public string FromInfo;
        public string RoomLastOption;
        protected Transform RoomItemParent;
        protected CreateRoomRuleInfo CurRuleInfo;
        public YxECreateRoomType CreateType = YxECreateRoomType.Normal;
        public bool NeedRecord = true;

        protected override void OnAwake()
        {
            base.OnAwake();
            CreateRoomRuleInfo.NeedRecord = NeedRecord;
        }

        protected override void OnStart()
        {
            var dic = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(FromInfo)) { dic["teaId"] = FromInfo; } 
            RoomListController.Instance.GetCreateRoomParm(IsDesignated ? GameKey : "", UpdateView,dic, CreateType);
            if (MyRcCountLabel != null) MyRcCountLabel.text = UserInfoModel.Instance.BackPack.GetItem("item2_q").ToString(CultureInfo.InvariantCulture);
        }

        protected override void OnFreshView()
        {
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
            SetDesignatedItem();
            UpdateTabs(TabDatas); 
        }

        private void SetDesignatedItem()
        {
            if (IsDesignated) return;
            if (string.IsNullOrEmpty(GameKey)) return;

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

        protected int GetTabDefaultIndex()
        {
            if (IsDesignated) return TabDefaultIndex;


            return TabDefaultIndex;
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
                if (!string.IsNullOrEmpty(RoomLastOption))
                {
                    dict["cargs"] = RoomLastOption;
                }
                var ruleInfo = new CreateRoomRuleInfo(dict, TabDefaultIndex);

                var tabData = new TabData
                {
                    Name = ruleInfo.Name,
                    Data = ruleInfo,
                    Index = i
                };
                TabDatas[i] = tabData;
            }
        }

        public string SelectedGameKey { get; protected set; }
        protected string SelectedRuleId;
        protected string SelectedGameName;
        protected UIToggle SelectTabToggle;
        public void OnChangeTabWithToggle(UIToggle toggle)
        {
            SelectedGameKey = "";
            SelectedGameName = "";
            if (!toggle.value) return;
            SelectTabToggle = toggle;
            int index;
            int.TryParse(toggle.name, out index);
            OnChangeTab(index);
        }

        protected int LastIndex = -1;
        private CreateRoomItemView _curCreateRoomItemView;
        private void OnChangeTab(int index)
        {
            if (RoomItemParent==null) { YxWindowUtils.CreateItemParent(RoomItemParentPerfab, ref RoomItemParent, RoomItemParentPerfab.parent);}
            if (index >= TabDatas.Length) return;
            if (LastIndex != index)
            {
                LastIndex = index;
                if (ScrollView != null) ScrollView.ResetPosition();
            }
            var tabData = TabDatas[index];
            if (tabData == null) return; 
            if(_curCreateRoomItemView==null) _curCreateRoomItemView = YxWindowUtils.CreateItem(RoomItemViewPerfab, RoomItemParent);
            var itemData = tabData.Data;
            if (itemData == null) return;
            var data1 = itemData as CreateRoomRuleInfo;
            if (data1 != null)
            {
                UpdateItemView(data1, _curCreateRoomItemView);
                return;
            }

            var dic = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(FromInfo)) { dic["teaId"] = FromInfo; }
            RoomListController.Instance.GetCreateRoomParm(itemData.ToString(), obj =>
            { 
                var list = obj as List<object>;
                if (list == null) return; 
                foreach (var itemObj in list)
                {
                    if (!(itemObj is Dictionary<string, object>)) continue;
                    var itemDict = itemObj as Dictionary<string, object>;
                    if (!string.IsNullOrEmpty(RoomLastOption))
                    {
                        itemDict["cargs"] = RoomLastOption;
                    }
                    var ruleInfo = new CreateRoomRuleInfo(itemDict, TabDefaultIndex);
                    if (!_tabWithData.ContainsKey(ruleInfo.Id)) continue;
                    var tbData = _tabWithData[ruleInfo.Id];
                    tbData.Data = ruleInfo;
                }
                itemData = tabData.Data;
                var data = itemData as CreateRoomRuleInfo;
                if (data==null) return;
                UpdateItemView(data, _curCreateRoomItemView);
            }, dic, CreateType);
        }

        /// <summary>
        /// 刷新页面
        /// </summary>
        /// <param name="info"></param>
        /// <param name="itemView"></param>
        private void UpdateItemView(CreateRoomRuleInfo info, YxView itemView)
        {
            if (info == null) return;
            SelectedGameKey = info.GameKey;
            SelectedRuleId = info.Id;
            CurRuleInfo = info; 
            info.Reset();
            SelectedGameName = info.Name;
//            if (_curRuleInfo != null)
//            {
//                var cArgs = _curRuleInfo.CreateArgs;
//                var allCreateData = GetAllRp(cArgs);
//                UpdataAllState(allCreateData, cArgs);
//            }
            itemView.UpdateView(info);
        }
         

        public void CreateRoom()
        {
            if (CurRuleInfo == null) return;
            var cArgs = CurRuleInfo.CreateArgs;
            var argsInfo = AnalyzeArgs(cArgs);
            var data = GetParm(argsInfo);
            if (argsInfo.HasSpecial("-gpsCtrl") && Application.isMobilePlatform)
            {
                //todo 1 判断权限 和 服务是否都开启   没找到好法子，版本低？
                YxGPSManager.CheckServiceAndSetting(() =>
                {
                    SendCreateRoomInfo(data);
                });
                return;
            }
            SendCreateRoomInfo(data);
        }

        private void SendCreateRoomInfo(Dictionary<string, object> data)
        {
            if (!string.IsNullOrEmpty(SelectedGameKey))
            {
                var info = string.Format("{0}|{1}|{2}", data["cargs"], data["ruleId"], data["num"]);
                PlayerPrefs.SetString(string.Format("{0}_createRoomParm", SelectedGameKey), info);
            }
            if (!string.IsNullOrEmpty(FromInfo))
            {
                data["tea_id"] = FromInfo;
            }
            //继续开局，创建茶馆房间
            string teadId = PlayerPrefs.GetString("teaId");
            if (!string.IsNullOrEmpty(teadId))
            {
                data["teaId"] = teadId;
            }
            SaveLastGame();
            SendCreateRoom(data);
        }

        protected virtual Dictionary<string, object> GetParm(CrParameter argsInfo)
        {
            var data = new Dictionary<string, object>();
            var args = argsInfo.SendContent;
            if (!string.IsNullOrEmpty(args))
            {
                data["cargs"] = args;
            }
            data["ruleId"] = SelectedRuleId;
            data["num"] = argsInfo.UseNum.ToString(CultureInfo.InvariantCulture);
            return data;
        }

        /// <summary>
        /// 发送创建房间消息
        /// </summary>
        /// <param name="data"></param>
        protected virtual void SendCreateRoom(Dictionary<string, object> data)
        {
            data["type"] = (int)CreateType;
            if (data.ContainsKey("tea_id"))
            {
                RoomListController.Instance.CreatGroupRoom(data, CreateRoomBack);
            }
            else
            {
                RoomListController.Instance.CreatRoom(data);
            }
        }

        /// <summary>
        /// 获取选择的参数
        /// </summary>
        /// <param name="cArgs"></param>
        /// <returns></returns>
        protected CrParameter AnalyzeArgs(IDictionary<string, ItemData> cArgs)
        {
            var args = "";
            var sign = "";
            if (cArgs.ContainsKey(AllField))
            {
                cArgs.Remove(AllField);
            }
            var crparam = new CrParameter(args);
            foreach (var pair in cArgs)
            {
                var createData = pair.Value;
                var view = createData.View;
                if (view != null)
                {
                    var state = view.IsValid(); 
                    if (!state) continue;
                }
                else
                {
                    if(createData.Type != RuleItemType.none) { continue;}
                }
                var key = createData.Key;
                if (key == AllField) continue;
                crparam.AddKeyIfSpecial(key,createData.Value);
                args = string.Format("{0}{1}{2},{3}", args, sign, createData.Id, createData.Value);
                sign = ",";
                var expnum = createData.UseNum;
                if (expnum <= 0) continue;
                crparam.UseNum = expnum;
                crparam.UseTyp = createData.UseItem;
            }
            YxDebug.Log(args);
            crparam.SendContent = args;
            return crparam;
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
            if (CurRuleInfo == null) return;
            var id = view.Id;
            var cArgs = CurRuleInfo.CreateArgs;
            if (!cArgs.ContainsKey(id)) return;
            var cData = cArgs[id];
            if (cData == null) return;
            var type = cData.Type;
            var curToggle = view.Toggle;
            var isSelected = curToggle.value;
            var group = cData.Group;
            var curBtnId = cData.Id;
            if (isSelected)
            {
                switch (type)
                {
                    case RuleItemType.tab:
                        if (CurRuleInfo.CurTabId == curBtnId) return;
                        CurRuleInfo.CurTabId = cData.Id;
                        CurRuleInfo.SetButtonId(group, cData.Id);
                        CreateRoomRuleInfo.SaveItemState(null, cData.Id, group, CurRuleInfo.GameKey, true);
                        if (SelectTabToggle != null)
                        {
                            OnChangeTabWithToggle(SelectTabToggle);
                        }
                        return;
                    case RuleItemType.button:
                        curBtnId = CurRuleInfo.CurrentButtonId(group);
                        if (curBtnId == cData.Id) return;
                        CurRuleInfo.SetButtonId(group, cData.Id);
                        cData.FreshHides();
                        CreateRoomRuleInfo.SaveItemState(CurRuleInfo.CurTabId,cData.Id, group, CurRuleInfo.GameKey, true);
                        if (SelectTabToggle != null)
                        {
                            OnChangeTabWithToggle(SelectTabToggle);
                        }
                        return;
                    default:
                        CreateRoomRuleInfo.SaveItemState(CurRuleInfo.CurTabId, cData.Id, group, CurRuleInfo.GameKey, true);
                        ExecuteWithClickType(cData, cArgs);
                        if (!IsNotClick(cData.Key == AllField, curToggle.gameObject))
                        {
                            UpdateOtherOptions(cData.Dp, cArgs, true);
                            UpdateOtherOptions(cData.Ep, cArgs, false);
                        }
                        break;
                }
            }
            else
            {
                CreateRoomRuleInfo.SaveItemState(CurRuleInfo.CurTabId, cData.Id, group, CurRuleInfo.GameKey, false);
                if (!IsNotClick(cData.Key == AllField, curToggle.gameObject))
                {
                    UpdateOtherOptions(cData.Rp, cArgs, false); 
                }
            }
            if (cData.Key == AllField) return; 
            if (!IsNotClick(true, view.gameObject))
            {
                var allCreateData = GetAllRp(cArgs);
                UpdataAllState(allCreateData, cArgs);
            }
        }

        protected bool IsNotClick(bool needCheck,GameObject go)
        {
            if (needCheck)
            {
                var touch = UICamera.currentTouch;
                if (touch == null || touch.current != go)
                { 
                    return true;
                }
            }
            return false;
        }

        private void ExecuteWithClickType(ItemData data, Dictionary<string, ItemData> dict)
        {
            if (data == null) { return; }
            var clickTypeData = data.ClickType;
            if (clickTypeData == null) { return; }
            switch (clickTypeData.Type)
            {
                case ECLickType.Show:
                    var parm = clickTypeData.Parm;
                    if (parm != null && parm.Length > 1)
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
            var allView = (NguiCheckBox) data.View;
            if (allView == null) return;
            var toggle = allView.Toggle;
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
                CreateRoomRuleInfo.SaveItemState(CurRuleInfo.CurTabId, data.Id, data.Group, CurRuleInfo.GameKey, false);
                return;
            }
            toggle.value = true;
            CreateRoomRuleInfo.SaveItemState(CurRuleInfo.CurTabId, data.Id, data.Group, CurRuleInfo.GameKey, true);
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
                CreateRoomRuleInfo.SaveItemState(CurRuleInfo.CurTabId, otherData.Id, otherData.Group, CurRuleInfo.GameKey, isSelected);
            }
        }


        public void OnHelpClick()
        {
            if (SelectTabToggle == null) return;
            int index;
            if (!int.TryParse(SelectTabToggle.name, out index)) return;
            if (index >= TabDatas.Length) return;
            var tabDataObj = TabDatas[index];
            if (tabDataObj == null) return;
            var data = tabDataObj.Data as CreateRoomRuleInfo;
            if (data == null) return;

            var win = CreateChildWindow("RuleWindow");
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
            PlayerPrefs.SetString(SaveKey, SelectedGameName);
        }
        #endregion
        private void CreateRoomBack(object data)
        {
            var roomData = data as Dictionary<string, object>;
            if(roomData==null){return;}
            var noJoin = roomData.ContainsKey("noJoin") && bool.Parse(roomData["noJoin"].ToString());
            if (noJoin)
            {
                PlayerPrefs.SetInt("noJoin", 1);
                Close();
            }
        }

        protected struct CrParameter
        {
            /// <summary>
            /// 发送内容
            /// </summary>
            public string SendContent;
            /// <summary>
            /// 
            /// </summary>
            public string UseTyp;
            /// <summary>
            /// 
            /// </summary>
            public float UseNum;

            /// <summary>
            /// 特殊key
            /// </summary>
            private readonly Dictionary<string, object> _specialKeys;

            public CrParameter(string sendContent)
            {
                UseTyp = "";
                UseNum = 0;
                SendContent = sendContent;
                _specialKeys = new Dictionary<string, object>();
            }

            public void AddKeyIfSpecial(string key,object value)
            {
                switch (key)
                {
                    case "-gpsCtrl"://gps 权限
                        _specialKeys[key] = value;
                        break;
                }
            }

            public void ClearSpecial()
            {
                _specialKeys.Clear();
            }

            public bool HasSpecial(string key)
            {
                return _specialKeys.ContainsKey(key);
            }

            public object GetSpecial(string key)
            {
                return HasSpecial(key) ? _specialKeys[key] : null;
            }
        }
    }
}
