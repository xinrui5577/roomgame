using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Tea.House;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Tea
{
    public class TeaTableItem : YxView
    {
        public GameObject AddBt;
        public UILabel RoomId;
        public GameObject CloseBt;
        public GameObject Users;
        public GameObject OverIcon;
        public GameObject Info;
        public GameObject UserInfos;
        public string RealRoomId;
        public UILabel GameRound;
        public UILabel GameName;
        public UILabel[] UserNames;
        public YxBaseTextureAdapter[] Avatars;
        public UISprite[] EmptySeat;
        public UISprite ColorSprite;
        public GameObject yxz;

        public TeaRoomCtrl TeaRoomCtrl;

        [HideInInspector]
        public string InfoStr;

        [HideInInspector]
        public TeaPanel TeaPanel;

        private TableState TableState;
        [Tooltip("牌桌布局处理，目前结构需要放到头像的父级，并且关联头像默认图标与存在头像后Texture")]
        public TeaTableLayout Layout;

        [Tooltip("消耗容器")]
        public GameObject CostContainer;

        [Tooltip("消耗类型")]
        public UISprite Goldtype;

        [Tooltip("消耗上限")]
        public UILabel GoldMax;

        [Tooltip("消耗下限")]
        public UILabel GoldMin;

        [Tooltip("排序ID")]
        public YxBaseLabelAdapter OrderId;

        [Tooltip("最小值显示格式")]
        public string MinLabelForMat = "{0}~";

        [Tooltip("分享状态事件")]
        public List<EventDelegate> ShareStateAction=new List<EventDelegate>();

        /// <summary>
        /// 分享按钮显示状态(1.权限整体受创建开关影响2.完结牌桌不分享)
        /// </summary>
        public bool ShareShowState
        {
            get { return TeaUtil.OnlyOwener == 0&& TableState!= TableState.Over; }
        }

        public string TableGameKey { private set; get; }

        private RoomInfoData _roomData;

        public void SetTableState(TableState state)
        {
            TableState = state;
            switch (state)
            {
                case TableState.Empty:
                    AddBt.SetActive(true);
                    RoomId.gameObject.SetActive(false);
                    CloseBt.SetActive(false);
                    Users.SetActive(false);
                    OverIcon.SetActive(false);
                    Info.SetActive(false);
                    UserInfos.SetActive(false);
                    yxz.SetActive(false);
                    break;
                case TableState.BeforePlay:
                    AddBt.SetActive(false);
                    RoomId.gameObject.SetActive(true);
                    CloseBt.SetActive(true);
                    Users.SetActive(true);
                    OverIcon.SetActive(false);
                    Info.SetActive(true);
                    UserInfos.SetActive(true);
                    yxz.SetActive(false);
                    break;
                case TableState.PlayerBeforPlay:
                    AddBt.SetActive(false);
                    RoomId.gameObject.SetActive(true);
                    CloseBt.SetActive(false);
                    Users.SetActive(true);
                    OverIcon.SetActive(false);
                    Info.SetActive(true);
                    UserInfos.SetActive(true);
                    yxz.SetActive(false);
                    break;
                case TableState.Play:
                    AddBt.SetActive(false);
                    RoomId.gameObject.SetActive(true);
                    CloseBt.SetActive(false);
                    Users.SetActive(false);
                    OverIcon.SetActive(false);
                    Info.SetActive(true);
                    UserInfos.SetActive(true);
                    yxz.SetActive(true);
                    break;
                case TableState.Over:
                    AddBt.SetActive(false);
                    RoomId.gameObject.SetActive(true);
                    CloseBt.SetActive(true);
                    OverIcon.SetActive(true);
                    Info.SetActive(true);
                    UserInfos.SetActive(true);
                    yxz.SetActive(false);
                    break;
            }
        }

        protected override void OnFreshView()
        {
            _roomData = GetData<RoomInfoData>();
            if (_roomData == null) return;
            TableGameKey = _roomData.GameKey;
            Reset();
            InfoStr = _roomData.InfoStr;
            RealRoomId = _roomData.RoomId;
            RoomId.text = TeaUtil.SubId(_roomData.RoomId);
            GameName.text = _roomData.GameName;
            GameRound.text = _roomData.GameRound+(_roomData.IsQuan? "圈":"局")+ _roomData.LimitGold;
            CostShow(_roomData);
            SetOrderId(Id);
            if (Layout)
            {
                Layout.SetLayoutByNum(_roomData.UserNum);
            }
            if (_roomData.UserNum > 0)
            {
                for (int i = 0; i < EmptySeat.Length; i++)
                {
                    if (_roomData.UserNum > i)
                    {
                        EmptySeat[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        EmptySeat[i].gameObject.SetActive(false);
                    }
                    
                }
            }

            for (int i = 0; i < _roomData.UserInfos.Length; i++)
            {
                if (string.IsNullOrEmpty(_roomData.UserInfos[i].UserName))
                {
                    continue;
                }
                UserNames[i].text = _roomData.UserInfos[i].UserName;
                string url = _roomData.UserInfos[i].Avatar;
                if (!string.IsNullOrEmpty(url))
                {
                    PortraitDb.SetPortrait(url, Avatars[i], 1);
                }
                Avatars[i].gameObject.SetActive(true);
            }
            if (TeaPanel.TableGameKey.Contains(_roomData.GameKey))
            {
                for (int i = 0; i < TeaPanel.TableGameKey.Length; i++)
                {
                    if (_roomData.GameKey==TeaPanel.TableGameKey[i])
                    {
                        ColorSprite.color=TeaPanel.TableColor[i];
                    }
                }
            }
            if (_roomData.Status>0)
            {
                yxz.SetActive(true);
            }

            if (TeaRoomCtrl)
            {
                TeaRoomCtrl.ChangeTableItemBg(_roomData, EmptySeat);
            }
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(ShareStateAction.WaitExcuteCalls());
            }
        }

        public void ClickJieSan()
        {
            if (TableState == TableState.Over)
            {
                YxMessageBox.Show(
                    "您是否确定删除房间",
                    null,
                    (window, btnname) =>
                    {
                        switch (btnname)
                        {
                            case YxMessageBox.BtnLeft:
                                Dictionary<string, object> dic = new Dictionary<string, object>();
                                object obj = RealRoomId;
                                dic["roomId"] = obj;
                                Facade.Instance<TwManager>().SendAction("group.dissolveRoom", dic, msg =>
                                {
                                    TeaUtil.GetBackString(msg);
                                    TeaPanel.GetTableList(false);
                                });
                                break;
                        }
                    },
                    true,
                    YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                );
            }
            else
            {
                YxMessageBox.Show(
                    "您是否确定解散房间",
                    null,
                    (window, btnname) =>
                    {
                        switch (btnname)
                        {
                            case YxMessageBox.BtnLeft:
                                Dictionary<string, object> dic = new Dictionary<string, object>();
                                object obj = RealRoomId;
                                dic["roomId"] = obj;
                                Facade.Instance<TwManager>().SendAction("group.removeRoom", dic, msg =>
                                {
                                    TeaUtil.GetBackString(msg);
                                    TeaPanel.GetTableList(false);
                                });
                                break;
                        }
                    },
                    true,
                    YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                );
            }
        }
        /// <summary>
        /// 消耗显示处理
        /// </summary>
        /// <param name="roomData"></param>
        private void CostShow(RoomInfoData roomData)
        {
            if (CostContainer)
            {
                bool showState = false;
                EnumCostType costtype = (EnumCostType)Enum.Parse(typeof(EnumCostType), roomData.GoldType);
                if (costtype != null)
                {
                    if (costtype != EnumCostType.TempCoin)
                    {
                        showState = true;
                        CostContainer.SetActive(true);
                        if (GoldMin)
                        {
                            GoldMin.text = string.Format(MinLabelForMat, roomData.GoldMin);
                        }
                        if (GoldMax)
                        {
                            GoldMax.text = roomData.GoldMax.ToString();
                        }
                        if (Goldtype)
                        {
                            Goldtype.spriteName = roomData.GoldType;
                        }
                    }

                }
                CostContainer.SetActive(showState);
            }
        }

        public void FindRoom(int roomId)
        {
            RoomListController.Instance.FindRoom(roomId, obj =>
            {
                var data = obj as IDictionary<string, object>;
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
                var Rid = int.Parse(rid.ToString());
                YxDebug.Log("加入房间的真实ID是" + Rid);
                if (roomId < 1)
                {
                    YxMessageBox.Show("查找异常！");
                    return;
                }
                var gameKey = (string)(data.ContainsKey("gameKey") ? data["gameKey"] : App.GameKey);
                JoinRoomData roomData = new JoinRoomData();
                roomData.roomId = Rid;
                roomData.GameKey = gameKey;
                roomData.Info = InfoStr;
                MainYxView.OpenWindowWithData("TeaRoomInfoWindow", roomData);
            });
        }

        public void Reset()
        {
            foreach (var texture in Avatars)
            {
                texture.gameObject.SetActive(false);
            }
            //CloseBt.SetActive(true);
            foreach (UISprite sprite in EmptySeat)
            {
                sprite.gameObject.SetActive(true);
            }
        }

        public void Press()
        {
//            if (TableState != TableState.BeforePlay && TableState != TableState.PlayerBeforPlay)
//            {
//                return;
//            }
            transform.localScale = Vector3.one * 0.9f;
        }

        public void OnClick()
        {
            if (TableState != TableState.BeforePlay && TableState != TableState.PlayerBeforPlay)
            {
                if (TableState == TableState.Over)
                {
                    OnCreateUserInfoWindow();
                }
                return;
            }
            FindRoom(int.Parse(RoomId.text));
        }

        public void Release()
        {
//            if (TableState != TableState.BeforePlay && TableState != TableState.PlayerBeforPlay)
//            {
//                return;
//            }
            transform.localScale = Vector3.one;
        }

        /// <summary>
        ///  设置排序ID
        /// </summary>
        public void SetOrderId(string num)
        {
            OrderId.TrySetComponentValue(num);
        }


        /// <summary>
        /// 点击分享按钮
        /// </summary>
        /// <param name="roomId">房间ID</param>
        /// <param name="roomInfo">房间信息</param>
        /// <param name="gameKey"></param>
        public void OnClickShareBtn(string roomId, string roomInfo,string gameKey)
        {
            YxTools.ShareFriend(roomId, roomInfo,gameKey,TeaUtil.CurTeaId);
        }
        /// <summary>
        /// 点击历史房间显示结算信息
        /// </summary>
        public void OnCreateUserInfoWindow()
        {
            var pWin = MainYxView as YxWindow;
            if (pWin == null)return;
            YxWindow obj = pWin.CreateChildWindow("TeaUserInfoPanel");
            TeaUserInfoPanel infoPanel = obj.GetComponent<TeaUserInfoPanel>();
            infoPanel.GameName.text = _roomData.GameName;
            infoPanel.RoomId.text = _roomData.RoomId;
            infoPanel.RoundAndUse.text = string.Format("{0}{1} {2}房卡", _roomData.GameRound, _roomData.IsQuan ? "圈" : "局", _roomData.UseNum);
            string rule = _roomData.InfoStr;
            string[] strList = rule.Split(' ');
            rule = "";
            for (int i = 0; i < strList.Length; i++)
            {
                if (strList[i] == "")
                {
                    continue;
                }
                if (i == strList.Length - 1)
                {
                    rule += strList[i];
                    continue;
                }
                rule += strList[i] + "\n";
            }
            infoPanel.RuleInfo.text = rule;

            var users = _roomData.UserInfos;
            for (int i = 0; i < users.Length; i++)
            {
                infoPanel.UserNames[i].text = users[i].UserName;
                infoPanel.UserNames[i].gameObject.SetActive(true);
                infoPanel.Scores[i].text = YxUtiles.GetShowNumberForm(long.Parse(users[i].Gold), 0, "N0");
                infoPanel.Scores[i].gameObject.SetActive(true);
                infoPanel.Heads[i].mainTexture = Avatars[i].GetTexture();
                infoPanel.Heads[i].gameObject.SetActive(true);
                if (infoPanel.Ids.Length != 0)
                {
                    infoPanel.Ids[i].text = users[i].Id;
                }
            }
        }
    }

    public enum TableState
    {
        Empty,
        BeforePlay,
        PlayerBeforPlay,
        Play,
        Over
    }

}
