using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using Assets.Scripts.Hall.View.AboutRoomWindows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;
using com.yxixia.utile.YxDebug;
using System;
using System.Collections;

namespace Assets.Scripts.Hall.View.NnFriendWindow
{
    //牛友群的功能
    public class NnFriendWindow : YxNguiWindow
    {
        public UILabel InputGroupLable;
        public UIInput InputDes;
        public UILabel CurrentGroupName;
        public UILabel CurrentGroupId;
        public UILabel CurrentGroupSign;
        public GameObject BackGround;
        public UIScrollView ScrollView;
        public TweenPosition Left;
        public TweenPosition Bg;
        public GameObject CreatBg;
        public UILabel GroupTitle;
        public TweenScale GroupInfo;
        public TweenScale CreatNnGroup;
        public GameObject GroupPeopleBg;
        public GameObject BtnShowDeleteBg;
        public UILabel ShowBtnShowDeleteBgState;
        public GameObject GroupModificaOwner;
        public GameObject GroupModificaOther;
        public GameObject GroupPeopleInfo;
        public GameObject ChangeGroupNamePanel;
        public UIInput GroupNameLable;
        public UIInput GroupSignLable;
        public GameObject HupGroupPanel;
        public UILabel HupGroupShowLable;
        public GameObject BtnWaitApply;
        public GameObject BtnRemind;

        public UIGrid GroupRoomGrid;
        public NnGroupRoomItem NnGroupRoomItem;

        public UIGrid GroupRecreationRoomGrid;
        public NnGroupRecreationRoomItem GroupRecreationRoomItem;

        public UIGrid FriendGroupGrid;
        public NnFriendGroupItem NnFriendGroupItem;

        public UIGrid GroupMemberGrid;
        public NnGroupMemberItem NnGroupMemberItem;


        public UIGrid GroupWaitApply;
        public NnGroupMemberItem NnGroupWaitItem;

        private int _curPageNum;
        private int _showState = -1;
//        private string _currentGroupId;
        private string _currentUserId;
        private string _currentUserName;
        private string _currenrGroupOwnerId;
        private bool _isCurrentGroupOwner;
        private bool _isStart;
        private UIToggle _currentToggle;
        private readonly List<Dictionary<string, object>> _currentGroupUesrData = new List<Dictionary<string, object>>();
        private BtnState _sureBtnState;

        protected void Awake()
        {
            if (ScrollView != null)
            {
                ScrollView.onMomentumMove = OnDragFinished;
            }
        }
        protected void Start()
        {
            RequestGroupData();
            _isStart = true;
        }

        private void OnDragFinished()
        {
            ScrollView.UpdateScrollbars(true);
            var constraint = ScrollView.panel.CalculateConstrainOffset(ScrollView.bounds.min, ScrollView.bounds.min);
            if (constraint.y <= 1f && GroupRoomGrid.transform.childCount == 20)
            {
                var dic = new Dictionary<string, object>();
                dic["tea_id"] =TeahouseController.Instance.CurrentTeaId ;
                dic["p"] = ++_curPageNum;
                Facade.Instance<TwManager>().SendAction("friends.getGroupRooms", dic, GroupRoomData);

            }
        }
        /// <summary>
        /// 请求玩家所在群的数据
        /// </summary>
        public void RequestGroupData()
        {
            Facade.Instance<TwManager>().SendAction("friends.requestGroupData", new Dictionary<string, object>(), OnFreshFriendGroup);
        }
        /// <summary>
        /// 刷新玩家所在群数据的回调
        /// </summary>
        /// <param name="data"></param>
        private void OnFreshFriendGroup(object data)
        {
            while (FriendGroupGrid.transform.childCount > 0)
            {
                DestroyImmediate(FriendGroupGrid.transform.GetChild(0).gameObject);
            }
            if (data == null)
            {
                Left.PlayForward();
                return;
            }
            if (!(data is Dictionary<string, object>)) return;
            var dataInfo = (Dictionary<string, object>)data;
            var groupDatas = dataInfo.ContainsKey("data") ? dataInfo["data"] : null;
            var head = dataInfo.ContainsKey("head") ? dataInfo["head"] : null;
            var groupRoomCount = dataInfo.ContainsKey("groupRoomCount") ? dataInfo["groupRoomCount"] : null;
            if (groupDatas == null)
            {
                Bg.gameObject.SetActive(false);
                BackGround.SetActive(false);
            }
            if (!(groupDatas is List<object>) || !(head is List<object>) || !(groupRoomCount is List<object>)) return;
            var groupData = groupDatas as List<object>;
            if (_isStart)
            {
                if (groupData.Count > 0)
                {
                    BackGround.SetActive(true);
                    Left.PlayForward();
                    Left.AddOnFinished(() =>
                    {
                        Bg.gameObject.SetActive(true);
                        Bg.PlayForward();
                    });
                }
                else
                {
                    Left.PlayForward();
                }
                _isStart = false;
            }
            else
            {
                if (groupData.Count > 0)
                {
                    BackGround.SetActive(true);
                    Bg.gameObject.SetActive(true);
                    Bg.PlayForward();
                }
            }
            var headData = head as List<object>;
            var roomCount = groupRoomCount as List<object>;
            for (int i = 0; i < groupData.Count; i++)
            {
                var groupInfo = groupData[i] as Dictionary<string, object>;
                var headInfo = headData[i] as Dictionary<string, object>;
                if (groupInfo == null) return;
                if (headInfo == null) return;
                var userId = groupInfo.ContainsKey("user_id") ? groupInfo["user_id"].ToString() : "";
                var groupId = groupInfo.ContainsKey("tea_id") ? int.Parse(groupInfo["tea_id"].ToString()) : -1;
                var groupNum = groupInfo.ContainsKey("group_n") ? int.Parse(groupInfo["group_n"].ToString()) : 0;
                var groupName = groupInfo.ContainsKey("tea_name") ? groupInfo["tea_name"].ToString() : "";
                var avatarData = headInfo.ContainsKey("avatar_x") ? headInfo["avatar_x"].ToString() : "";
                var groupSign = groupInfo.ContainsKey("group_sign") ? groupInfo["group_sign"] :null;
                var item = YxWindowUtils.CreateItem(NnFriendGroupItem, FriendGroupGrid.transform);
                item.name = i.ToString();
                var groupSignDes="";
                if (groupSign != null)
                {
                    groupSignDes = groupSign.ToString();
                }
                item.InitData(groupName, int.Parse(roomCount[i].ToString()), groupNum, avatarData, groupId, userId, groupSignDes);
            }

            SelectDefault();
            FriendGroupGrid.repositionNow = true;
        }

        private void SelectDefault()
        {
            if (PlayerPrefs.HasKey("teaSelectIndex"))
            {
                var teaSelectIndex = int.Parse(PlayerPrefs.GetString("teaSelectIndex"));
                FriendGroupGrid.GetChild(teaSelectIndex).GetComponent<UIToggle>().startsActive = true;
            }
            else
            {
                FriendGroupGrid.transform.GetChild(0).GetComponent<UIToggle>().startsActive = true;
            }

        }


        /// <summary>
        /// 群操作的请求（加入群或者创建群）
        /// </summary>
        public void OperateFriendGroup()
        {
            var dic = new Dictionary<string, object>();
            string actionName;
            if (_sureBtnState == BtnState.Creat)
            {
                if (string.IsNullOrEmpty(InputGroupLable.GetComponentInParent<UIInput>().value))
                {
                    YxMessageBox.Show("您输入的名字不能为空 请确认后重试");
                    return;
                }
                if (InputGroupLable.GetComponentInParent<UIInput>().value.Length > 9)
                {
                    YxMessageBox.Show("您输入的名字过长 最多可填写9位");
                    return;
                }
                dic["tea_name"] = InputGroupLable.GetComponentInParent<UIInput>().value;
                dic["group_sign"] = InputDes.value;
                actionName = "friends.creatFriendGroup";
            }
            else
            {
                if (string.IsNullOrEmpty(InputGroupLable.GetComponentInParent<UIInput>().value))
                {
                    YxMessageBox.Show("您输入的ID不能为空 请确认后重试");
                    return;
                }
                dic["tea_id"] = InputGroupLable.GetComponentInParent<UIInput>().value;
                actionName = "friends.joinFriendGroup";
            }
            Facade.Instance<TwManager>().SendAction(actionName, dic, OnFreshFriendGroup);
            OnBackBtn();
        }

        public void OnOpenCreateRoomWindow()
        {
            var win = YxWindowManager.OpenWindow("CreateRoomWindow") as CreateRoomWindow;
            if (win == null) return;
            win.FromInfo = TeahouseController.Instance.CurrentTeaId.ToString();
            InvokeRepeating("CreatRoomSuccess", 0, 1);
        }

        private void CreatRoomSuccess()
        {
            if (PlayerPrefs.GetInt("noJoin") == 1)
            {
                var dic = new Dictionary<string, object>();
                dic["tea_id"] = TeahouseController.Instance.CurrentTeaId;
                dic["p"] = ++_curPageNum;
                Facade.Instance<TwManager>().SendAction("friends.getGroupRooms", dic, GroupRoomData);
                PlayerPrefs.SetInt("noJoin", 2);
                CancelInvoke("CreatRoomSuccess");
            }
        }

        /// <summary>
        /// 关闭此界面
        /// </summary>
        public void OnCloseBtn()
        {
            if (Bg.gameObject.activeSelf)
            {
                Left.RemoveOnFinished(new EventDelegate(() => Bg.PlayForward()));
                Bg.PlayReverse();
                Bg.AddOnFinished(() =>
                {
                    Bg.gameObject.SetActive(false);
                    Left.PlayReverse();
                    Left.AddOnFinished(Close);
                });
                TeahouseController.Instance.CurrentTeaId = -1;
            }
            else
            {
                Left.PlayReverse();
                Left.AddOnFinished(Close);
            }
        }

        /// <summary>
        /// 界面左边的加号按钮
        /// </summary>
        public void OnAddBtn()
        {
            CreatBg.SetActive(!CreatBg.activeSelf);
        }
        /// <summary>
        /// 下拉框中的创建或者加入
        /// </summary>
        /// <param name="obj"></param>
        public void OnCreatOrJoinBtn(GameObject obj)
        {
            _showState = int.Parse(obj.name);
            if (int.Parse(obj.name) == 0)
            {
                _sureBtnState = BtnState.Creat;
                GroupTitle.text = "创建俱乐部";
                InputGroupLable.text = "请输入俱乐部名";
                InputDes.gameObject.SetActive(true);
            }
            else
            {
                _sureBtnState = BtnState.Join;
                GroupTitle.text = "加入俱乐部";
                InputGroupLable.text = "请输入俱乐部ID";
                InputDes.gameObject.SetActive(false);
            }
            CreatNnGroup.PlayForward();
            GroupInfo.PlayForward();
            OnAddBtn();
        }
        /// <summary>
        /// 返回按钮
        /// </summary>
        public void OnBackBtn()
        {
            CreatNnGroup.PlayReverse();
            GroupInfo.PlayReverse();
        }
        /// <summary>
        /// 根据当前的toggle状态显示群信息
        /// </summary>
        /// <param name="toggle"></param>
        public void CurrentGroupShow(UIToggle toggle)
        {
            if (toggle.value)
            {
                PlayerPrefs.SetString("teaSelectIndex",toggle.name);
                GroupModificaOwner.gameObject.SetActive(false);
                GroupModificaOther.gameObject.SetActive(false);
                _currentToggle = toggle;
                GroupPeopleBg.SetActive(false);
                CurrentGroupName.text = toggle.GetComponent<NnFriendGroupItem>().CurrentGroupName;
                CurrentGroupSign.text = toggle.GetComponent<NnFriendGroupItem>().CurrentGroupSign;
                CurrentGroupId.text = string.Format("(ID:{0})", toggle.GetComponent<NnFriendGroupItem>().CurrentGroupId);
                var teaId= toggle.GetComponent<NnFriendGroupItem>().CurrentGroupId;
                TeahouseController.Instance.CurrentTeaId = teaId;
                _currenrGroupOwnerId = toggle.GetComponent<NnFriendGroupItem>().CurrentGroupOwnerId;
                _curPageNum = 0;
                var dict = new Dictionary<string, object>();
                dict["tea_id"] = teaId;
                dict["p"] = _curPageNum;
//                Facade.Instance<TwManager>().SendActions(new []{ "friends.findGroupFriend" , "friends.getGroupRooms"}, dict, GroupFriendCallBack);
                Facade.Instance<TwManager>().SendAction("friends.findGroupFriend", dict, GroupFriendCallBack);
                Facade.Instance<TwManager>().SendAction("friends.getGroupRooms", dict, GroupRoomData);
                if (_currenrGroupOwnerId == UserInfoModel.Instance.UserInfo.UserId)
                {
                    _isCurrentGroupOwner = true;
                    var dic = new Dictionary<string, object>();
                    dic["tea_id"] = teaId;
                    Facade.Instance<TwManager>().SendAction("friends.wiatFriendApply", dic, UserApplyCallBack);
                }
                else
                {
                    BtnWaitApply.SetActive(false);
                    GroupPeopleInfo.SetActive(false);
                    BtnRemind.SetActive(false);
                    _isCurrentGroupOwner = false;
                }
            }
        }
        /// <summary>
        /// 查找群中创建的房间信息
        /// </summary>
        /// <param name="data"></param>
        private void GroupRoomData(object data)
        {
            var roomDic=data as Dictionary<string,object>;
            if(roomDic==null)return;
            var creatRoom = roomDic["creatRoom"];
            var normalRoom = roomDic["normalRoom"];
            if (creatRoom != null)
            {
                var roomDatas = creatRoom as List<object>;
                if (roomDatas == null) return;

                _currentToggle.GetComponent<NnFriendGroupItem>().OnFreshRoomCount(roomDatas.Count);

                while (GroupRoomGrid.transform.childCount > 0)
                {
                    DestroyImmediate(GroupRoomGrid.transform.GetChild(0).gameObject);
                }
                foreach (var roomData in roomDatas)
                {
                    var roomDetial = roomData as Dictionary<string, object>;
                    if (roomDetial == null) return;
                    var roomId = roomDetial.ContainsKey("rndId") ? roomDetial["rndId"].ToString() : "";
                    var gameName = roomDetial.ContainsKey("name") ? roomDetial["name"].ToString() : "";
                    var round = roomDetial.ContainsKey("round") ? roomDetial["round"].ToString() : "";
                    var roomOwnId = roomDetial.ContainsKey("ownid") ? roomDetial["ownid"].ToString() : "";
                    var userHead = "";
                    var userName = "";
                    foreach (var userdata in _currentGroupUesrData)
                    {
                        if (userdata["user_id"].ToString().Equals(roomOwnId))
                        {
                            userHead = userdata["avatar_x"].ToString();
                            userName = userdata["nick_m"].ToString();
                        }
                    }
                    var info = roomDetial.ContainsKey("info") ? roomDetial["info"].ToString() : "";
                    var infos = info.Split(';');
                    var ante = "";
                    var rule = "";
                    foreach (var s in infos)
                    {
                        if (s.Contains("底"))
                        {
                            ante = s.Split(':')[1];
                            continue;
                        }
                        if (!s.Contains("翻")) continue;
                        rule = s.Split(':')[1].Split(' ')[0];
                    }
                    var item = YxWindowUtils.CreateItem(NnGroupRoomItem, GroupRoomGrid.transform);
                    item.InitData(userHead, userName, roomOwnId, gameName, roomId, ante, round, "房主付费", rule);
                    item.name = roomId;
                }
                GroupRoomGrid.repositionNow = true;
            }

            if (normalRoom != null)
            {
                var normalRoomData = normalRoom as List<object>;
                if(normalRoomData==null)return;
                while (GroupRecreationRoomGrid.transform.childCount > 0)
                {
                    DestroyImmediate(GroupRecreationRoomGrid.transform.GetChild(0).gameObject);
                }
                foreach (var roomData in normalRoomData)
                {
                    var roomInfo = roomData as Dictionary<string, object>;
                    if(roomInfo==null)return;
                    var minGold = roomInfo.ContainsKey("gold_min_q")
                        ? int.Parse(roomInfo["gold_min_q"].ToString())
                        : -1;
                    var maxGold = roomInfo.ContainsKey("gold_max_q")
                        ? int.Parse(roomInfo["gold_max_q"].ToString())
                        : -1;
                    var gameName = roomInfo.ContainsKey("game_name") ? roomInfo["game_name"].ToString() : "";
                    var gameKey = roomInfo.ContainsKey("game_key_c") ? roomInfo["game_key_c"].ToString() : "";
                    var gameType = roomInfo.ContainsKey("game_type") ? int.Parse(roomInfo["game_type"].ToString()) : 0;
                    var item3=YxWindowUtils.CreateItem(GroupRecreationRoomItem, GroupRecreationRoomGrid.transform);
                    item3.GetComponent<UIButton>().onClick.Add(new EventDelegate(() =>
                    {
                        GameListController.Instance.QuickGame(gameKey);
                    }));
                    item3.InitRoomData(gameName, maxGold, minGold);
                }

                GroupRecreationRoomGrid.repositionNow = true;
            }
         
         
        }
        /// <summary>
        /// 查找群成员
        /// </summary>
        public void OnGroupBtn()
        {
            GroupPeopleBg.SetActive(!GroupPeopleBg.activeSelf);
            if (GroupPeopleBg.activeSelf)
            {
                var dic = new Dictionary<string, object>();
                dic["tea_id"] = TeahouseController.Instance.CurrentTeaId;
                Facade.Instance<TwManager>().SendAction("friends.findGroupFriend", dic, GroupFriendCallBack);
            }
        }
        /// <summary>
        /// 查找好友的回调数据
        /// </summary>
        /// <param name="data"></param>
        private void GroupFriendCallBack(object data)
        {
            var info = data as Dictionary<string, object>;
            if (info == null) return;
            var userData = info.ContainsKey("data") ? info["data"] : null;
            if (userData is List<object>)
            {
                _currentGroupUesrData.Clear();
                while (GroupMemberGrid.transform.childCount > 0)
                {
                    DestroyImmediate(GroupMemberGrid.transform.GetChild(0).gameObject);
                }
                var memberInfos = userData as List<object>;
                for (int i = 0; i < memberInfos.Count; i++)
                {
                    var objects = memberInfos[i] as Dictionary<string, object>;
                    if (objects == null) return;
                    var memberData = objects;
                    _currentGroupUesrData.Add(memberData);
                    var userId = memberData.ContainsKey("user_id")
                                         ? memberData["user_id"].ToString()
                                         : "";
                    var coin = memberData.ContainsKey("coin_t") ? memberData["coin_t"].ToString() : "";
                    var nickName = memberData.ContainsKey("nick_m")
                                        ? memberData["nick_m"].ToString()
                                        : "";
                    var avatar = memberData.ContainsKey("avatar_x")
                                        ? memberData["avatar_x"].ToString()
                                        : "";
                    var item = YxWindowUtils.CreateItem(NnGroupMemberItem, GroupMemberGrid.transform);
                    item.InitData(nickName, userId, avatar, i == 0, coin);
                    if (i == 0)
                    {
                        var userdata = UserInfoModel.Instance.UserInfo;
                        if (userId.Equals(userdata.UserId))
                        {
                            BtnShowDeleteBg.SetActive(true);
                            ShowBtnShowDeleteBgState.text = "删除成员";
                        }
                        else
                        {
                            BtnShowDeleteBg.SetActive(false);
                            ShowBtnShowDeleteBgState.text = "成员列表";
                        }
                    }
                }
                GroupMemberGrid.repositionNow = true;
            }
        }

        /// <summary>
        /// 删除群好友功能按钮
        /// </summary>
        public void OnDeleteGroupFriend()
        {
            for (int i = 1; i < GroupMemberGrid.transform.childCount; i++)
            {
                var obj = GroupMemberGrid.transform.GetChild(i).GetComponent<NnGroupMemberItem>().BtnDeleteMember;
                obj.SetActive(!obj.activeSelf);
            }

        }
        /// <summary>
        /// 删除好友时候的消息提示框
        /// </summary>
        public void OnDeleteFriendRemind(GameObject obj)
        {
            _currentUserId = obj.GetComponentInParent<NnGroupMemberItem>().UserId; //被删除的玩家的ID
            _currentUserName = obj.GetComponentInParent<NnGroupMemberItem>().MemberName.text;
            var message = new YxMessageBoxData
                {
                    Msg = string.Format("确定删除群员{0}?", _currentUserName),
                    Listener = OnDeleteSingleFriend,
                    BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                    ShowBtnNames = new[] { "btn_ok|确定", "btn_cancle|取消" }
                };
            YxMessageBox.Show(message);
            _currentToggle.GetComponent<NnFriendGroupItem>().OnDeletePeopleCount();
        }

        private void OnDeleteSingleFriend(YxMessageBox boxmsg, string btnname)
        {
            if (btnname == YxMessageBox.BtnLeft)
            {
                var dic = new Dictionary<string, object>();
                dic["tea_id"] = TeahouseController.Instance.CurrentTeaId;
                dic["user_id"] = _currentUserId;
                Facade.Instance<TwManager>().SendAction("friends.deleteGroupFriend", dic, GroupFriendCallBack);
            }
        }


        /// <summary>
        /// 界面右边的设置
        /// </summary>
        public void OnSetBtn()
        {
            if (_isCurrentGroupOwner)
            {
                GroupModificaOwner.SetActive(!GroupModificaOwner.activeSelf);
            }
            else
            {
                GroupModificaOther.SetActive(!GroupModificaOther.activeSelf);
            }
        }
        /// <summary>
        /// 打开改变群的名字
        /// </summary>
        public void OpenChangeGroupName()
        {
            ChangeGroupNamePanel.SetActive(true);
            GroupNameLable.value = CurrentGroupName.text;
            GroupSignLable.value = CurrentGroupSign.text;
        }
        /// <summary>
        /// 点击确定修改群名字
        /// </summary>
        public void OnSureChangeGroupName()
        {
            var dic = new Dictionary<string, object>();
            if (!GroupNameLable.value.Equals(CurrentGroupName.text))
            {
                dic["tea_name"] = GroupNameLable.value;
            }

            if (!GroupSignLable.value.Equals(CurrentGroupSign.text))
            {
                dic["group_sign"] = GroupSignLable.value;
            }

            dic["tea_id"] = TeahouseController.Instance.CurrentTeaId;
            Facade.Instance<TwManager>().SendAction("friends.changeGroupName", dic, data =>
                {
                    ChangeGroupNamePanel.SetActive(false);
                    OnFreshFriendGroup(data);
                });
        }
        /// <summary>
        /// 关闭修改群名字界面
        /// </summary>
        public void OnCancleChangeGroupName()
        {
            ChangeGroupNamePanel.SetActive(false);
        }
        /// <summary>
        /// 打开解散群界面
        /// </summary>
        public void OpenHupGroup()
        {
            HupGroupPanel.SetActive(true);
            HupGroupShowLable.text = string.Format("[7F3204FF]确认解散群[-][BD3503FF]{0}{1}{2}[-][7F3204FF]解散后无法恢复[-]", '"', CurrentGroupName.text, '"');
        }
        /// <summary>
        /// 点击确定解散群
        /// </summary>
        public void OnSureHupGroup()
        {
            var dic = new Dictionary<string, object>();
            dic["tea_id"] = TeahouseController.Instance.CurrentTeaId;
            Facade.Instance<TwManager>().SendAction("friends.hupGroup", dic, data =>
            {
                HupGroupPanel.SetActive(false);
                OnSetBtn();
                OnFreshFriendGroup(data);
            });
        }
        /// <summary>
        /// 关闭解散群界面
        /// </summary>
        public void OnCancleHupGroup()
        {
            HupGroupPanel.SetActive(false);
        }
        /// <summary>
        /// 非群主玩家退出好友群
        /// </summary>
        public void OnQuitGroup()
        {
            var dic = new Dictionary<string, object>();
            dic["tea_id"] = TeahouseController.Instance.CurrentTeaId;
            Facade.Instance<TwManager>().SendAction("friends.quitFriendGroup", dic, OnFreshFriendGroup);
        }
        /// <summary>
        /// 好友申请的消息界面
        /// </summary>
        public void OnRemindBtn()
        {
            GroupPeopleInfo.SetActive(!GroupPeopleInfo.activeSelf);
        }
        /// <summary>
        /// 待处理的好友请求回调
        /// </summary>
        /// <param name="data"></param>
        private void UserApplyCallBack(object data)
        {
            if (data == null)
            {
                BtnWaitApply.SetActive(false);
                GroupPeopleInfo.SetActive(false);
                BtnRemind.SetActive(false);
                return;
            }
            var users = data as List<object>;
            if (users == null) return;
            BtnWaitApply.SetActive(true);
            BtnRemind.SetActive(true);
            while (GroupWaitApply.transform.childCount > 0)
            {
                DestroyImmediate(GroupWaitApply.transform.GetChild(0).gameObject);
            }
            foreach (var user in users)
            {
                var userData = user as Dictionary<string, object>;
                if (userData == null) return;
                var avatar = userData.ContainsKey("avatar_x") ? userData["avatar_x"].ToString() : "";
                var userId = userData.ContainsKey("user_id") ? userData["user_id"].ToString() : "";
                var nickName = userData.ContainsKey("nick_m") ? userData["nick_m"].ToString() : "";
                var item = YxWindowUtils.CreateItem(NnGroupWaitItem, GroupWaitApply.transform);
                item.name = userId;
                item.InitData(nickName, userId, avatar, false);
            }
            GroupWaitApply.repositionNow = true;
        }
        /// <summary>
        /// 群主处理玩家的请求
        /// </summary>
        /// <param name="obj"></param>
        public void OnDealUserApply(GameObject obj)
        {
            var dic = new Dictionary<string, object>();
            dic["deal_id"] = obj.GetComponentInParent<NnGroupMemberItem>().name;
            dic["tea_id"] = TeahouseController.Instance.CurrentTeaId;
            dic["status_f"] = obj.name;
            if (int.Parse(obj.name) == 3)
            {
                _currentToggle.GetComponent<NnFriendGroupItem>().OnAgreePeopleCount();
            }
            Destroy(obj.GetComponentInParent<NnGroupMemberItem>().gameObject);
            GroupWaitApply.repositionNow = true;
            Facade.Instance<TwManager>().SendAction("friends.dealFriendApply", dic, data =>
                {
                    OnRemindBtn();
                    UserApplyCallBack(data);
                });
        }
        /// <summary>
        /// 加入房间
        /// </summary>
        public void OnJoinRoom(GameObject obj)
        {
            var dic = new Dictionary<string, object>();
            dic["id"] = obj.GetComponentInParent<NnGroupRoomItem>().name;
            Facade.Instance<TwManager>().SendAction("room.find", dic, info =>
                {
                    var data = info as IDictionary<string, object>;
                    if (data == null)
                    {
                        YxMessageBox.Show("没有找到房间！！");
                        return;
                    }
                    var str = data.ContainsKey(RequestKey.KeyMessage) ? data[RequestKey.KeyMessage] : null;
                    if (str != null)
                    {
                        YxMessageBox.Show(str.ToString());
                        return;
                    }
                    var rid = data["roomId"];
                    Debug.Log(rid);
                    Debug.Log(rid.GetType());
                    if (rid is string)
                    {
                        rid = int.Parse(rid.ToString());
                    }
                    int roomId = int.Parse(rid.ToString());
                    YxDebug.LogError("加入房间的真实ID是" + roomId);
                    if (roomId < 1)
                    {
                        YxMessageBox.Show("查找异常！");
                        return;
                    }
                    var gameKey = (string)(data.ContainsKey("gameKey") ? data["gameKey"] : App.GameKey);
                    RoomListController.Instance.JoinFindRoom(roomId, gameKey);
                });
        }

    }
    public enum BtnState
    {
        Creat,
        Join
    }
}
