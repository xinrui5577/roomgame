using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.components;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Tea.House;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
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
        public UITexture[] Avatars;
        public UISprite[] EmptySeat;
        public UISprite ColorSprite;
        public GameObject yxz;

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

        [Tooltip("最小值显示格式")]
        public string MinLabelForMat = "{0}~";

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
                    Users.SetActive(false);
                    OverIcon.SetActive(true);
                    Info.SetActive(true);
                    UserInfos.SetActive(true);
                    yxz.SetActive(false);
                    break;
            }
        }

        protected override void OnFreshView()
        {
            var roomData = GetData<RoomInfoData>();
            if (roomData == null) return;
            Reset();
            InfoStr = roomData.InfoStr;
            RealRoomId = roomData.RoomId;
            RoomId.text = TeaUtil.SubId(roomData.RoomId);
            GameName.text = roomData.GameName;
            GameRound.text = roomData.GameRound+(roomData.IsQuan? "圈":"局");
            CostShow(roomData);
            if (Layout)
            {
                Layout.SetLayoutByNum(roomData.UserNum);
            }
            if (roomData.UserNum > 0)
            {
                for (int i = 0; i < EmptySeat.Length; i++)
                {
                    if (roomData.UserNum > i)
                    {
                        EmptySeat[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        EmptySeat[i].gameObject.SetActive(false);
                    }

                }
            }
            for (int i = 0; i < roomData.UserInfos.Length; i++)
            {
                if (string.IsNullOrEmpty(roomData.UserInfos[i].UserName))
                {
                    continue;
                }
                UserNames[i].text = roomData.UserInfos[i].UserName;
                string url = roomData.UserInfos[i].Avatar;
                if (!string.IsNullOrEmpty(url))
                {
                    PortraitRes.SetPortrait(url, Avatars[i], 1);
                }
                Avatars[i].gameObject.SetActive(true);
            }
            if (TeaPanel.TableGameKey.Contains(roomData.GameKey))
            {
                for (int i = 0; i < TeaPanel.TableGameKey.Length; i++)
                {
                    if (roomData.GameKey==TeaPanel.TableGameKey[i])
                    {
                        ColorSprite.color=TeaPanel.TableColor[i];
                    }
                }
            }
            if (roomData.status>0)
            {
                yxz.SetActive(true);
            }

        }

        public void ClickJieSan()
        {
            if (TableState == TableState.Over)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                object obj = RealRoomId;
                dic["roomId"] = obj;
                Facade.Instance<TwManger>().SendAction("group.dissolveRoom", dic, msg =>
                {
                    TeaUtil.GetBackString(msg);
                    TeaPanel.GetTableList(false);
                });
            }
            else
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                object obj = RealRoomId;
                dic["roomId"] = obj;
                Facade.Instance<TwManger>().SendAction("group.removeRoom", dic, msg =>
                {
                    TeaUtil.GetBackString(msg);
                    TeaPanel.GetTableList(false);
                });
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
                YxWindowManager.HideWaitFor();
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
                var win = CreateOtherWindow("TeaRoomInfoWindow");
                win.UpdateView(roomData);
            });
        }

        public void Reset()
        {
            foreach (UITexture texture in Avatars)
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
            if (TableState != TableState.BeforePlay && TableState != TableState.PlayerBeforPlay)
            {
                return;
            }
            transform.localScale = Vector3.one * 0.9f;
        }

        public void OnClick()
        {
            if (TableState != TableState.BeforePlay && TableState != TableState.PlayerBeforPlay)
            {
                return;
            }
            FindRoom(int.Parse(RoomId.text));
        }

        public void Release()
        {
            if (TableState != TableState.BeforePlay && TableState != TableState.PlayerBeforPlay)
            {
                return;
            }
            transform.localScale = Vector3.one;
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
