using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Constants;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Tea
{
    public class TeaPanel : YxNguiWindow
    {
        [Tooltip("牌桌预设")] public TeaTableItem TableItem;
        [Tooltip("添加游戏按钮")] public GameObject AddGame;
        [Tooltip("申请游戏按钮")] public GameObject ShenQing;
        [Tooltip("权限按钮父级")] public GameObject GuanLiBts;
        [Tooltip("赠送按钮")] public GameObject SendBtn;
        [Tooltip("茶馆口令文本")] public UILabel Code;
        [Tooltip("茶馆名称")] public UILabel Name;
        [Tooltip("茶馆牌桌布局Grid")] public UIGrid Grid;
        [Tooltip("茶馆牌桌数量文本")] public UILabel TableNumLabel;
        [Tooltip("积分文本")] public UILabel CoinTLabel;
        [Tooltip("金币文本")] public UILabel CoinALabel;
        [Tooltip("牌桌GameKey与TableColor对齐")] public string[] TableGameKey;
        [Tooltip("牌桌颜色 与TableGameKey对齐")] public Color[] TableColor;
        [Tooltip("自动刷新茶馆牌桌列表频率")] public float AutoFrashTime = 10f;
        [Tooltip("牌桌数量")] public int roomNum;
        [Tooltip("积分字段")] public string KeyCoinT = "coin_t";
        [Tooltip("牌桌数量格式")] public string FormatTableNumber = "桌数:{0}/{1}";
        [Tooltip("1:馆主2:是成员3:非成员4:没有此茶馆")] public int TeaState;
        [Tooltip("是否是直接进入茶馆")] public bool IsQuickJoin;
        [Tooltip("进入茶馆前上个界面")] public string LastWindowName;
        [Tooltip("茶馆牌桌名称格式(原逻辑只处理了未完结牌桌，逻辑不变)")]
        public string TableNameFormat = "{0}{1}";
        /// <summary>
        /// "馆主ID"
        /// </summary>
        public int TeaOwnerId
        {
            set { CurTeaOwnerId = value;}
            get { return CurTeaOwnerId;}
        }

        /// <summary>
        /// 茶馆ID，该字段为当前茶馆公用属性，相关茶馆功能需要使用
        /// </summary>
        public static int CurTeaId;
        /// <summary>
        /// 茶馆名称
        /// </summary>
        public static string CurTeaName;
        /// <summary>
        /// 馆主Id
        /// </summary>
        public static int CurTeaOwnerId;
        /// <summary>
        /// 是否开启授权开房 0，关闭 1.开启
        /// </summary>
        public static int PersonCtrl;
        /// <summary>
        /// 成员在茶馆内创建房间开关：0，成员可开房 其它，不可开房
        /// </summary>
        public static int OnlyOwner = -1;
        /// <summary>
        /// 牌桌数量
        /// </summary>
        int _tableNum;

        public int TableNum
        {
            get { return _tableNum; }
            set
            {
                _tableNum = value;
                TableNumLabel.TrySetComponentValue(string.Format(FormatTableNumber, _tableNum, roomNum));
            }
        }

        /// <summary>
        /// 牌桌列表
        /// </summary>
        private List<TeaTableItem> _usedItemList = new List<TeaTableItem>();

        /// <summary>
        /// 最后一次获得房间列表的时间
        /// </summary>
        private float _lastGetListTime;
        /// <summary>
        /// 获得数据的当前页数
        /// </summary>
        private int _currentPage;
        /// <summary>
        /// 请求的总数据
        /// </summary>
        private List<object> _totalDatas;
        /// <summary>
        /// 是否是使用群积分
        /// </summary>
        private bool _hasSend;
        /// <summary>
        /// 是否请求gamehistory
        /// </summary>
        private bool _showLog;
        /// <summary>
        /// 请求grouproom单页数据个数
        /// </summary>
        private int _groupRoomsPageSize;

        protected override void OnStart()
        {
            Facade.EventCenter.AddEventListener<string, object>("teaSendMoney", FreshUserGroupCoin);
            HallMainController.Instance.AddLaunchInStartEvent(HallStartEventType.TeaWindow);
            StartCoroutine("AutoFrash");
        }

        IEnumerator AutoFrash()
        {
            while (true)
            {
                yield return new WaitForSeconds(AutoFrashTime);
                GetTableList(false);
            }
        }

        public void OpPowerWindow()
        {
            YxWindow obj = CreateChildWindow("TeaMember");
            TeaPower power = obj.GetComponent<TeaPower>();
            power.TeaId = Code.text;
            power.HasSend = _hasSend;
        }

        public void OpTeaRoomInfoWindow()
        {
            YxWindow obj = CreateChildWindow("TeaRoomInfo");
            TeaRoomInfo infoWindow = obj.GetComponent<TeaRoomInfo>();
            infoWindow.TeaId = Code.text;
        }

        public void CasePower(int index)
        {
            switch (index)
            {
                case 1:
                    ShenQing.SetActive(false);
                    GuanLiBts.SetActive(true);
                    if (AddGame != null) AddGame.SetActive(true);
                    break;
                case 2:
                    ShenQing.SetActive(false);
                    GuanLiBts.SetActive(false);
                    if (AddGame != null) AddGame.SetActive(OnlyOwner == 0);
                    break;
                case 3:
                    ShenQing.SetActive(true);
                    GuanLiBts.SetActive(false);
                    if (AddGame != null) AddGame.SetActive(false);
                    break;
            }
        }

        public void SetTeaCode(int code)
        {
            Code.text = code + "";
            CurTeaId = code;
        }

        public void OtherTea()
        {
            var window = MainYxView as YxWindow;
            if (window != null)
            {
                window.CreateChildWindow("TeaFindRoom");
            }
            else
            {
                CreateOtherWindow("TeaFindRoom");
                Close();
            }
        }

        public void ApplyGetIn()
        {
            TeaUtil.ApplyJoinTeaHouse(Code.text);
        }

        public void SetTeaName(string teaName)
        {
            CurTeaName = teaName;
            Name.text = teaName;
        }

        /// <summary>
        /// 最后一次牌桌请求时间
        /// </summary>
        private float _lastSecond;

        /// <summary>
        /// 获取茶馆牌桌列表
        /// </summary>
        /// <param name="WaitBox"></param>
        public void GetTableList(bool WaitBox = true)
        {
            if (!WaitBox)
            {
                _lastSecond = 0;
            }
            var nowSecond = Time.time;
            if (nowSecond - _lastSecond < 2f)
            {
                return;
            }
            if (Grid == null)
            {
                return;
            }
            _lastSecond = nowSecond;
            var dic = new Dictionary<string, object>();
            if (CurTeaId > 0)
            {
                dic["id"] = CurTeaId;
                dic["p"] = _currentPage++;
                if (TeaState == 1 || TeaState == 2)
                {
                    _totalDatas = new List<object>();
                    RoomListController.Instance.GetGroupRoomList(dic, GetTeaRoomList, WaitBox);
                }
            }
        }

        private void GetTeaRoomList(object msg)
        {
            if (Time.time - _lastGetListTime < 0.1f)
            {
                return;
            }
            if (this == null) //todo 很奇葩，但是真的走里面去了，猜测为：收到消息回调时，当前对象已销毁？，待验证
            {
                YxDebug.LogError("当前对象Teapanel是空的........");
                return;
            }
            _usedItemList.Clear();

            foreach (Transform child in Grid.transform)
            {
                TeaTableItem item = child.GetComponent<TeaTableItem>();
                if (item)
                {
                    _usedItemList.Add(item);
                }
            }

            List<object> dic = (List<object>)msg;
            if (dic != null)
            {
                _totalDatas.AddRange(dic);
                if (dic.Count == _groupRoomsPageSize)
                {
                    var dict = new Dictionary<string, object>();
                    dict["id"] = CurTeaId;
                    dict["p"] = _currentPage++;
                    RoomListController.Instance.GetGroupRoomList(dict, GetTeaRoomList);
                    return;
                }
                else
                {
                    _currentPage = 0;
                    StopCoroutine("AutoFrash");
                    StartCoroutine("AutoFrash");
                    if (_showLog)
                    {
                        Dictionary<string, object> dic2 = new Dictionary<string, object>();
                        object obj1 = Code.text;
                        dic2["id"] = obj1;
                        Facade.Instance<TwManager>().SendAction("group.historyRoom", dic2, GetHistoryTableItem, false, null, false);
                    }
                }
            }
           
            _lastGetListTime = Time.time;
            _tableNum = 0;
          
            foreach (Dictionary<string, object> dicItem in _totalDatas)
            {
                TeaTableItem item;
                if (_usedItemList.Count > 0 && _usedItemList[0] != null)
                {
                    item = _usedItemList[0];
                    _usedItemList.RemoveAt(0);
                }
                else
                {
                    item = YxWindowUtils.CreateItem(TableItem, Grid.transform);
                }
                if (TeaState == 1)
                {
                    item.SetTableState(TableState.BeforePlay);
                }
                if (TeaState == 2)
                {
                    item.SetTableState(TableState.PlayerBeforPlay);
                }
                item.Id = (TableNum + 1).ToString();
                item.TeaPanel = this;
                RoomInfoData roomInfo = new RoomInfoData();
                roomInfo.ParseGameServerData(dicItem);
                roomInfo.GameName = string.Format(TableNameFormat, roomInfo.GameName, roomInfo.AnteInfo);
                item.UpdateView(roomInfo);
                _tableNum++;
            }

            if (!_showLog)
            {
                TableNum = _tableNum;
            }
            //            if (TeaState == 2)
            //            {
            Grid.Reposition();
            if (!_showLog)
            {
                foreach (TeaTableItem item in _usedItemList)
                {
                    if (item != null)
                    {
                        DestroyImmediate(item.gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// 牌桌历史记录数据
        /// </summary>
        /// <param name="msg"></param>
        void GetHistoryTableItem(object msg)
        {
            if (msg == null)
            {
                TableNum = _tableNum;
                return;
            }
            Dictionary<string, object> dic = (Dictionary<string, object>) msg;
            object obj = dic["history"];
            List<object> objList = (List<object>) obj;
            foreach (Dictionary<string, object> info in objList)
            {
                TeaTableItem item;
                if (_totalDatas.Count+ objList.Count > Grid.transform.childCount)
                {
                    item = YxWindowUtils.CreateItem(TableItem, Grid.transform);
                }
                else
                {
                    item = _usedItemList[0];
                    _usedItemList.RemoveAt(0);
                }
                item.Id = (_tableNum + 1).ToString();
                item.TeaPanel = this;
                item.SetTableState(TableState.Over);
                RoomInfoData roomInfo = new RoomInfoData();
                roomInfo.ParseData(info);
                if (TeaState == 2)
                {
                    item.CloseBt.SetActive(false);
                }
                item.UpdateView(roomInfo);
                _tableNum++;
            }
            TableNum = _tableNum;
            foreach (TeaTableItem item in _usedItemList)
            {
                if (item != null)
                {
                    DestroyImmediate(item.gameObject);
                }
            }
            if (Grid)
            {
                Grid.Reposition();
            }
        }

        public void ClickFlesh()
        {
            GetTableList();
        }

        public void OnTaskInfo()
        {
            CreateChildWindow("TaskWindow");
            //if (win == null) return;
            //win.UpdateView(HallModel.Instance.OptionSwitch.Task);
        }

        public void OnSendMoney()
        {
            var win = CreateChildWindow("TeaMoneyWindow");
            win.GetComponent<TeaMoneyWindow>().TeaId = int.Parse(Code.text);
            win.GetComponent<TeaMoneyWindow>().TeaState = TeaState;
            win.GetComponent<TeaMoneyWindow>().TeaOwnerId = TeaOwnerId;
        }

        private TeaData _teaData;

        protected void InitData(Dictionary<string, object> dict)
        {
            var teaData = new TeaData(dict);
            PersonCtrl = teaData.PersonCtrlStatus;
            OnlyOwner = teaData.OnlyOwner;
            TeaState = (int)teaData.Mstatus;
            CasePower(TeaState);
            roomNum = teaData.RoomNum;
            SetTeaName(teaData.Name);
            var teaId = teaData.TeaId;
            if (teaId > 0) SetTeaCode(teaId);
            TeaOwnerId = teaData.UserId;
            _showLog = teaData.IsShowLog;
            _groupRoomsPageSize = teaData.GroupRoomsPageSize;
        }

        protected override void OnFreshView()
        {
            var dict = Data as Dictionary<string, object>;
            if (dict != null)
            {
                InitData(dict);
                FreshUserGroupCoin(Data);
                GetTableList(false);
            }
        }

        private void FreshUserGroupCoin(object data)
        {
            var dict = data as Dictionary<string, object>;
            if(dict==null)return;
            var useGroupcoin = int.Parse(dict["use_groupcoin"].ToString());
            long coinT;
            dict.TryGetValueWitheKey(out coinT, KeyCoinT);
            switch (useGroupcoin)
            {
                case 0:
                    CoinALabel.TrySetComponentValue(
                        YxUtiles.GetShowNumber(UserInfoModel.Instance.UserInfo.CoinA)
                            .ToString(CultureInfo.InvariantCulture));
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    _hasSend = true;
                    if (SendBtn != null) SendBtn.SetActive(true);
                    if (CoinTLabel != null) CoinTLabel.gameObject.SetActive(true);
                    CoinTLabel.TrySetComponentValue(
                        YxUtiles.GetShowNumber(coinT).ToString(CultureInfo.InvariantCulture));
                    break;
            }
        }

        public void OnOpenCreateWindow(string objName)
        {
            if (_tableNum >= roomNum)
            {
                YxMessageBox.Show("房间数已达上限，无法继续创建房间");
                return;
            }
            //var win = YxWindowManager.OpenWindow("TeaCreateRoomWindow", true);
            var win = CreateChildWindow("TeaCreateRoomWindow");
            var createWin = (TeaCreateRoomWindow) win;
            if (createWin == null) return;
            createWin.teaPanel = this;
            createWin.FromInfo = Code.text;
            createWin.GameKey = "";
        }

        public override void Close()
        {
            base.Close();
            //UserController.Instance.GetUserDate();
            UserController.Instance.GetBackPack();
            HallMainController.Instance.RemoveLaunchInStartEvent(HallStartEventType.TeaWindow);
        }

        public void OnOtherTea()
        {
            if (IsQuickJoin)
            {
                CreateOtherWindow("TeaFindRoom");
                IsQuickJoin = false;
            }
            Close();
        }
    }


    public class TeaData
    {
        /// <summary>
        /// Key是否开启授权开房
        /// </summary>
        private const string KeyPersonCtrl= "per_ctrl";
        /// <summary>
        /// 开启授权开房状态
        /// </summary>
        private int _personCtrlStatus;

        /// <summary>
        /// 进入茶馆时的状态   1：馆主  2：成员 3：非成员 4：没有此茶馆
        /// </summary>
        public TeaState Mstatus;
        /// <summary>
        /// 茶馆名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 成员在茶馆内创建房间开关：0，成员可开房 其它，不可开房
        /// </summary>
        public int OnlyOwner;
        /// <summary>
        /// 牌桌数量
        /// </summary>
        public int RoomNum;
        /// <summary>
        /// 茶馆Id
        /// </summary>
        public int TeaId;
        /// <summary>
        /// "馆主ID"
        /// </summary>
        public int UserId;
        /// <summary>
        /// 使用群积分
        /// </summary>
        public int UseGroupCoin;
        /// <summary>
        ///控制是否请求gamhistory 
        /// </summary>
        public bool IsShowLog;
        /// <summary>
        /// getGroupRooms每页请求的数据个数
        /// </summary>
        public int GroupRoomsPageSize=10;
        public int PersonCtrlStatus
        {
            get { return _personCtrlStatus;}
        }

        public TeaData(Dictionary<string, object> dict)
        {
            Pare(dict);
        }

        public void Pare(Dictionary<string, object> dict)
        {
            if (dict == null) return;
            if (dict.ContainsKey("mstatus"))
            {
                Mstatus = (TeaState)int.Parse(dict["mstatus"].ToString());
            }
            if (dict.ContainsKey("name"))
            {
                Name = dict["name"].ToString();
            }
            OnlyOwner = dict.ContainsKey("only_owner") ? int.Parse(dict["only_owner"].ToString()) : -1;
            if (dict.ContainsKey("roomNum"))
            {
                RoomNum = int.Parse(dict["roomNum"].ToString());
            }
            TeaId = dict.ContainsKey("tea_id") ? int.Parse(dict["tea_id"].ToString()) : 0;
            if (dict.ContainsKey("user_id"))
            {
                UserId = int.Parse(dict["user_id"].ToString());
            }
            if (dict.ContainsKey("use_groupcoin"))
            {
                UseGroupCoin = int.Parse(dict["use_groupcoin"].ToString());
            }
            dict.TryGetValueWitheKey(out _personCtrlStatus, KeyPersonCtrl);
            if (dict.ContainsKey("show_log"))
            {
                IsShowLog= int.Parse(dict["show_log"].ToString())==1;
            }
            if (dict.ContainsKey("groupRoomsPageSize"))
            {
                GroupRoomsPageSize = int.Parse(dict["groupRoomsPageSize"].ToString());
            }
        }
    }

    public enum TeaState
    {
        /// <summary>
        /// 馆长
        /// </summary>
        Curator = 1,
        /// <summary>
        /// 成员
        /// </summary>
        Member,
        /// <summary>
        /// 非成员
        /// </summary>
        NonMember,
        /// <summary>
        /// 无效茶馆
        /// </summary>
        Invalid
    }
}
