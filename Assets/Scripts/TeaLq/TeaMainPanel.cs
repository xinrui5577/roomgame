using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using Assets.Scripts.Tea;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.TeaLq
{
    public class TeaMainPanel : YxNguiWindow
    {

        public TeaInfoView TeaInfoView;
        /// <summary>
        /// 上楼按钮
        /// </summary>
        public GameObject ComeupFloorBtn;
        /// <summary>
        /// 牌桌预设
        /// </summary>
        public TeaDeskItem TableItem;
        /// <summary>
        /// 当前楼层房间规则
        /// </summary>
        public NguiLabelAdapter RoomRule;
        /// <summary>
        /// 茶馆牌桌布局Grid
        /// </summary>
        public UIGrid Grid;
        /// <summary>
        /// 自动刷新茶馆牌桌列表频率
        /// </summary>
        public float AutoFrashTime = 10f;
        /// <summary>
        /// 自动刷新茶馆牌桌座位列表频率
        /// </summary>
        public float AutoFrashSeatTime = 1f;
        /// <summary>
        /// 茶馆ID，该字段为当前茶馆公用属性，相关茶馆功能需要使用
        /// </summary>
        public static int CurTeaId;
        /// <summary>
        /// 当前玩家的状态 1馆主2是成员3非成员4没有此茶馆
        /// </summary>
        public static int Mstatus;
        /// <summary>
        /// 当前选择游戏的gameKey
        /// </summary>
        public static string CurGameKey;
        /// <summary>
        /// 当前所处的楼层
        /// </summary>
        [HideInInspector]
        public static int Floor = 1;

        public YxECreateRoomType CreatType = YxECreateRoomType.Normal;

        public string OpenUserListWindowName;
        /// <summary>
        /// 最后一次牌桌请求时间
        /// </summary>
        private float _lastSecond;
        /// <summary>
        /// 牌桌列表
        /// </summary>
        private readonly List<TeaDeskItem> _usedItemList = new List<TeaDeskItem>();
        /// <summary>
        /// 请求的总数据
        /// </summary>
        private readonly List<object> _totalDatas = new List<object>();
        /// <summary>
        /// 数据存储
        /// </summary>
        private List<TeaDeskItem> _deskList;
        /// <summary>
        /// 当前选择的桌子
        /// </summary>
        private TeaDeskItem _teaDeskItem;

        private Coroutine _autoFrashTable;
        private Coroutine _autoFrashTableSeat;

        protected override void OnStart()
        {
            base.OnStart();
            Facade.EventCenter.AddEventListener<string, object>("TeaTableFresh", FreshTable);
            Facade.EventCenter.AddEventListener<string, object>("TeaFresh", UpdateView);
            Facade.EventCenter.AddEventListener<string, Dictionary<string, object>>("TeaChange", TeaChange);
            Util.SetString("Tea_OpenName", "TeaPanel");
            HallMainController.Instance.AddReturnHallEvent(ReturnHallEvent,true);
            Facade.EventCenter.AddEventListener<string, object>("BackHall", CallBackHall);
        }

        private void ReturnHallEvent(object data)
        {
            if (Util.HasKey("Tea_OpenName"))
            {
                var teaOpenName = Util.GetString("Tea_OpenName");
                YxWindowManager.OpenWindow(teaOpenName);
            }
        }

        private void CallBackHall(object obj)
        {
            Close();
        }

        public override void Close()
        {
            base.Close();
            Util.RemoveData("Tea_OpenName");
            HallMainController.Instance.RemoveReturnHallEvent(ReturnHallEvent);
            if (_teaDeskItem == null) return;
            var dic = new Dictionary<string, object>();
            dic["floor"] = Floor;
            dic["teaId"] = CurTeaId;
            dic["tableId"] = _teaDeskItem.TableIndex;
            Facade.Instance<TwManager>().SendAction("group.clearUserState", dic, null, true, null, false);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Facade.EventCenter.RemoveEventListener("TeaTableFresh");
            Facade.EventCenter.RemoveEventListener("TeaFresh");
            Facade.EventCenter.RemoveEventListener("TeaChange");
            Facade.EventCenter.RemoveEventListener("BackHall");
        }

        /// <summary>
        /// 刷新桌子
        /// </summary>
        /// <param name="obj"></param>
        private void FreshTable(object obj)
        {
            var dic = obj as Dictionary<string, object>;
            if (dic == null) return;
            GetTeaRoomList(obj);
            SendGetGameDataAction();
        }
        /// <summary>
        /// 坐下的时候发起请求
        /// </summary>
        /// <param name="teaDeskItem"></param>
        public void OnSitApply(TeaDeskItem teaDeskItem)
        {
            _teaDeskItem = teaDeskItem;
            if (int.Parse(_teaDeskItem.RoomInfoData.CurRound) > 0)
            {
                YxMessageBox.Show("本桌游戏已开始，请等待下一轮！");
                return;
            }
            var dic = new Dictionary<string, object>();
            dic["teaId"] = CurTeaId;
            dic["floor"] = Floor;
            dic["tableId"] = teaDeskItem.TableIndex;
            Facade.Instance<TwManager>().SendAction("group.applySit", dic, null, true, null, false);
        }
        /// <summary>
        /// 查找房间里面的玩家信息
        /// </summary>
        /// <param name="tableIndex"></param>
        public void OpenDeskUserList(string tableIndex)
        {
            var dic = new Dictionary<string, object>();
            dic["teaId"] = CurTeaId;
            dic["floor"] = Floor;
            dic["tableId"] = tableIndex;
            dic["roomId"] = _deskList[int.Parse(tableIndex)].RoomInfoData.RoomId;
            var win = CreateChildWindow(OpenUserListWindowName);
            win.UpdateView(dic);
        }
        /// <summary>
        /// 刷新茶馆的信息
        /// </summary>
        /// <param name="dict"></param>
        private void TeaChange(Dictionary<string, object> dict)
        {
            var teaId = dict["teaId"].ToString();
            Floor = 1;
            var dic = new Dictionary<string, object>();
            dic["id"] = teaId;
            dic["type"] = (int)CreatType;
            dic["floor"] = Floor;
            Facade.Instance<TwManager>().SendAction("group.teaGetIn", dic, FreshPanel,true,null,false);
        }

        IEnumerator AutoFrashTable()
        {
            while (true)
            {
                OnClickFresh();
                yield return new WaitForSeconds(AutoFrashTime);
            }
        }
        IEnumerator AutoFrashTableSeat()
        {
            while (true)
            {
                GetTableSeatList();
                yield return new WaitForSeconds(AutoFrashSeatTime);
            }
        }
        /// <summary>
        /// 刷新按钮点击事件
        /// </summary>
        public void OnClickFresh()
        {
            GetTableList(false);
        }

        /// <summary>
        /// 获取茶馆牌桌列表
        /// </summary>
        /// <param name="waitBox"></param>
        public void GetTableList(bool waitBox = true)
        {
            var dic = new Dictionary<string, object>();
            if (CurTeaId == 0) return;
            dic["id"] = CurTeaId;
            dic["type"] = (int)CreatType;
            dic["floor"] = Floor;
            RoomListController.Instance.GetGroupRoomList(dic, GetTeaRoomList, waitBox);
        }
        /// <summary>
        /// 茶馆房间列表
        /// </summary>
        /// <param name="msg"></param>
        private void GetTeaRoomList(object msg)
        {
            if (msg == null)
            {
                ComeupFloorBtn.SetActive(false);
            }
            var objects = msg as Dictionary<string, object>;
            if (objects != null)
            {
                ComeupFloorBtn.SetActive(true);
                var dic = objects;
                int overType = 0;
                string info = "";
                if (dic.ContainsKey("overType"))
                {
                    overType = int.Parse(dic["overType"].ToString());
                }
                if (dic.ContainsKey("info"))
                {
                    info = dic["info"].ToString();
                }
                if (overType == 1)
                {
                    if (!string.IsNullOrEmpty(info))
                    {
                        YxMessageBox.Show(info, "", (box, btnName) =>
                        {
                            if (btnName == YxMessageBox.BtnLeft)
                            {
                                Close();
                            }
                        }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
                    }
                    if (_autoFrashTable != null)
                    {
                        StopCoroutine(_autoFrashTable);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(info))
                    {
                        YxMessageBox.Show(info);
                    }
                    var data = dic["data"] as List<object>;
                    if (data == null) return;
                    _deskList = new List<TeaDeskItem>();
                    _usedItemList.Clear();
                    foreach (Transform child in Grid.transform)
                    {
                        TeaDeskItem item = child.GetComponent<TeaDeskItem>();
                        if (item)
                        {
                            _usedItemList.Add(item);
                        }
                    }
                    if (data != null)
                    {
                        _totalDatas.Clear();
                        _totalDatas.AddRange(data);
                    }
                    else
                    {
                        return;
                    }
                    var index = 0;
                    foreach (Dictionary<string, object> dicItem in _totalDatas)
                    {
                        TeaDeskItem item;
                        if (_usedItemList.Count > 0 && _usedItemList[0] != null)
                        {
                            item = _usedItemList[0];
                            _usedItemList.RemoveAt(0);
                        }
                        else
                        {
                            item = YxWindowUtils.CreateItem(TableItem, Grid.transform);
                        }
                        _deskList.Add(item);
                        dicItem["index"] = index++;
                        item.UpdateView(dicItem);
                    }
                    Grid.Reposition();
                }
            }
        }
        /// <summary>
        /// 茶馆座位信息
        /// </summary>
        private void GetTableSeatList()
        {
            var dic = new Dictionary<string, object>();
            dic["teaId"] = CurTeaId;
            dic["floor"] = Floor;
            Facade.Instance<TwManager>().SendAction("group.getGroupTableSeat", dic, FreshTableSeatView, true, null, false);
        }
        /// <summary>
        /// 刷新茶馆座位显示
        /// </summary>
        /// <param name="obj"></param>
        private void FreshTableSeatView(object obj)
        {
            var infos = obj as Dictionary<string, object>;
            if (infos == null) return;
            if (_deskList != null && _deskList.Count != 0)
            {
                foreach (var list in _deskList)
                {
                    list.ShowQuitBtn(false);
                }
            }

            var datas = infos["data"] as List<object>;
            if (datas != null)
            {
                var userInfo = UserInfoModel.Instance.UserInfo;
                foreach (var info in datas)
                {
                    var data = info as Dictionary<string, object>;
                    if (data == null) return;
                    var teaTableSeatData = new TeaTableSeatData(data);
                    if (_deskList != null && _deskList.Count != 0)
                    {
                        if (int.Parse(userInfo.UserId) == teaTableSeatData.UserId)
                        {
                            _deskList[teaTableSeatData.TableId].ShowQuitBtn(true);
                        }
                        _deskList[teaTableSeatData.TableId].FreshSeatView(teaTableSeatData);
                    }
                }
            }
            var joinRoom = infos["joinRoom"] as Dictionary<string, object>;
            if (joinRoom != null)
            {
                var joinData = new JoinRoomData(joinRoom);
                JoinRoom(joinData);
            }
        }

        private void JoinRoom(JoinRoomData joinRoomData)
        {
            if (joinRoomData.State != 1) return;
            //            Debug.LogError("joinRoomData.RoomTrueId:" + joinRoomData.RoomTrueId);
            RoomListController.Instance.FindRoom(joinRoomData.RoomTrueId, obj =>
            {
                var data = obj as IDictionary<string, object>;
                if (data == null)
                {
                    YxMessageBox.Show("没有找到房间！！");
                    return;
                }
                var rid = data["roomId"];

                var roomId = int.Parse(rid.ToString());
                //                Debug.LogError("roomId:" + roomId+ "joinRoomData.GameKey:"+ joinRoomData.GameKey);

                RoomListController.Instance.JoinFindRoom(roomId, joinRoomData.GameKey);

                if (_autoFrashTable != null)
                {
                    StopCoroutine(_autoFrashTable);
                }
                if (_autoFrashTableSeat != null)
                {
                    StopCoroutine(_autoFrashTableSeat);
                }
            });
        }

        protected override void OnFreshView()
        {
            var dict = Data as Dictionary<string, object>;
            if (dict != null)
            {
                InitData(dict);
            }
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="dict"></param>
        protected void InitData(Dictionary<string, object> dict)
        {
            var teaData = new TeaNewData(dict);
            TeaInfoView.UpdateView(teaData);
            CurTeaId = teaData.TeaId;
            Floor = teaData.Floor;
            Mstatus = teaData.Mstatus;
            SendGetGameDataAction();

            if (_autoFrashTable != null)
            {
                StopCoroutine(_autoFrashTable);
            }
            if (_autoFrashTableSeat != null)
            {
                StopCoroutine(_autoFrashTableSeat);
            }
            _autoFrashTable = StartCoroutine("AutoFrashTable");
            _autoFrashTableSeat = StartCoroutine("AutoFrashTableSeat");
        }
        /// <summary>
        /// 获得当前茶馆的gamekey
        /// </summary>
        private void SendGetGameDataAction()
        {
            var dic = new Dictionary<string, object>();
            dic["teaId"] = CurTeaId;
            dic["floor"] = Floor;
            Facade.Instance<TwManager>().SendAction("group.getTeaGameData", dic, GetCurGameKey, true, null, false);
        }

        private void GetCurGameKey(object obj)
        {
            var dic = obj as Dictionary<string, object>;
            if (dic == null) return;
            if (dic.ContainsKey("gameKey"))
            {
                CurGameKey = dic["gameKey"].ToString();
            }
            var info = dic["info"].ToString();
            RoomRule.TrySetComponentValue(info);
            RoomRule.transform.localPosition = new Vector3(500, -4, 0);
            RoomRule.GetComponent<TweenPosition>().enabled = true;
            var x = RoomRule.GetComponent<UILabel>().localSize.x;
            RoomRule.GetComponent<TweenPosition>().to = new Vector3(-500 - x, -4, 0);
        }

        /// <summary>
        /// 打开规则面板
        /// </summary>
        /// <param name="window"></param>
        public void OpenRuleWindow(string window)
        {
            if(CurTeaId==0)return;
            var win = CreateChildWindow(window);
            win.GetComponent<TeaCreateRoomWindow>().FromInfo = CurTeaId.ToString();
            win.GetComponent<TeaCreateRoomWindow>().GameKey = CurGameKey;
        }
        /// <summary>
        /// 打开子窗口没有权限则显示msg
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="msg"></param>
        public void OpenChildWindowWithMsg(string windowName, string msg)
        {
            if (Mstatus == 1)
            {
                OpenChildWindow(windowName);
            }
            else
            {
                YxMessageBox.Show(msg);
            }
        }
        /// <summary>
        /// 打开创建房间的界面
        /// </summary>
        /// <param name="windowName"></param>
        public void OpenTeaCreatRoomWindow(string windowName)
        {
            var win = CreateChildWindow(windowName);
            win.GetComponent<TeaCreateRoomWindow>().CurFloor = 1;
            win.GetComponent<TeaCreateRoomWindow>().FromInfo = 1.ToString();
        }
        /// <summary>
        /// 上楼点击按钮
        /// </summary>
        public void OnComeUp()
        {
            var nowSecond = Time.time;
            if (nowSecond - _lastSecond < 3f)
            {
                YxMessageBox.Show("您的操作过于频繁 请稍后点击");
                return;
            }
            _lastSecond = nowSecond;
            var dic = new Dictionary<string, object>();
            dic["id"] = CurTeaId;
            dic["type"] = (int)CreatType;
            dic["floor"] = Floor;
            Facade.Instance<TwManager>().SendAction("group.comeUpFloor", dic, FreshPanel);
        }

        private void FreshPanel(object msg)
        {
            var dict = msg as Dictionary<string, object>;
            if (dict != null)
            {
                InitData(dict);
            }
        }
    }

    public class TeaNewData
    {
        public int TeaId;
        public string TeaName;
        public string GameName;
        public int Floor;
        public int Mstatus;

        public TeaNewData(Dictionary<string, object> dict)
        {
            dict.Parse("tea_id", ref TeaId);
            dict.Parse("name", ref TeaName);
            dict.Parse("game_name", ref GameName);
            dict.Parse("floor_id", ref Floor);
            dict.Parse("mstatus", ref Mstatus);
        }
    }

    public class TeaTableSeatData
    {
        public int UserId = -1;
        public int RoomId;
        public int TableId;
        public int SeatId;
        public string NickName;
        public int Sex;
        public string Avatar;
        public bool IsJoinRoom;

        public TeaTableSeatData(Dictionary<string, object> dict)
        {
            dict.Parse("user_id", ref UserId);
            dict.Parse("room_true_id", ref RoomId);
            dict.Parse("room_show_id", ref TableId);
            dict.Parse("user_seat_id", ref SeatId);
            dict.Parse("nick_m", ref NickName);
            dict.Parse("sex_i", ref Sex);
            dict.Parse("avatar_x", ref Avatar);
            int roomId = 0;
            if (dict.Parse("join_room", ref roomId))
            {
                IsJoinRoom = roomId == 1;
            }
        }
    }

    public class JoinRoomData
    {
        public int RoomShowId;
        public int RoomTrueId;
        public string GameKey;
        public int State;


        public JoinRoomData(Dictionary<string, object> dic)
        {
            dic.Parse("room_show_id", ref RoomShowId);
            dic.Parse("room_true_id", ref RoomTrueId);
            dic.Parse("game_key", ref GameKey);
            dic.Parse("state", ref State);
        }
    }
}
