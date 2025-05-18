using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.Manager;
using YxFramwork.View;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Hall.View.AboutRoomWindows
{
    /// <summary>
    /// 查找房间窗口
    /// </summary>
    public class FindRoomWindow : YxNguiWindow
    {
        /// <summary>
        /// 房间id的Label
        /// </summary>
        [Tooltip("房间id的Label")]
        public UILabel RoomIdLabel;
        [Tooltip("由多个label组成的房间id,不包含RoomIdLabel属性")]
        public UILabel[] RoomIdMoreLabel;
        /// <summary>
        /// 房间id最大位数
        /// </summary>
        [Tooltip("房间id最大位数")]
        public int MaxIdCount = 6;
        /// <summary>
        /// 查找消息窗口
        /// </summary>
        [Tooltip("输入键")]
        public RoomInfoWindow RoominfoWindow;
        /// <summary>
        /// 房间id最大位数
        /// </summary>
        [Tooltip("输入键")]
        public UIButton[] Keyboards;
        /// <summary>
        /// 标签默认索引\
        /// </summary>
        [Tooltip("默认标签选择")]
        public int TabDefaultIndex = -1;
        /// <summary>
        /// 需要显示房间信息
        /// </summary>
        [Tooltip("需要显示房间信息")]
        public bool NeedRoomInfo = true;
        /// <summary>
        /// 自动查找
        /// </summary>
        [Tooltip("自动查找")]
        public bool AutoFind = true;

        protected override void OnAwake()
        {
            base.OnAwake();
            var count = Keyboards.Length; 
            for (var i = 0; i < count; i++)
            {
                var btn = Keyboards[i];
                if(btn==null)continue;
                UIEventListener.Get(btn.gameObject).onClick = OnClick;
            }
        }

        public void OnClick(GameObject go)
        {
            OnClickWithName(go.name);
        }

        public void OnCLick(int roomId)
        {
            FindRoomById(roomId);
        }

        public void OnClickWithName(string btnName)
        {
            switch (btnName)
            {
                case "del":
                    Delete();
                    break;
                case "cle":
                    Clear();
                    break;
                default:
                    if (Input(btnName)&&AutoFind)
                    {
                        OnFindRoom();
                    } 
                    break;
            }
        }

        private int _curInputIndex;
        private void Delete()
        {
            if (RoomIdMoreLabel.Length > 0)
            {
                if (_curInputIndex > 0)
                {
                    _curInputIndex--;
                    var label = RoomIdMoreLabel[_curInputIndex];
                    label.text = "";
                } 
                return;
            }
            var cur = RoomIdLabel.text;
            if (cur.Length < 1) return;
            RoomIdLabel.text = cur.Remove(cur.Length - 1);
        }

        protected void Clear()
        {
            if (RoomIdMoreLabel.Length > 0)
            {
                for (var i = 0; i < MaxIdCount; i++)
                {
                    var label = RoomIdMoreLabel[i];
                    label.text = "";
                    _curInputIndex = 0;
                }
                return;
            }
            RoomIdLabel.text = "";
        }

        private bool Input(string num)
        {
            if (RoomIdMoreLabel.Length > 0)
            {
                if (_curInputIndex >= MaxIdCount) return true;
                var label = RoomIdMoreLabel[_curInputIndex];
                label.text = num;
                _curInputIndex++;
                return _curInputIndex >= MaxIdCount;
            }
            if (RoomIdLabel.text.Length >= MaxIdCount) return true;
            var roomId = string.Format("{0}{1}", RoomIdLabel.text, num);
            RoomIdLabel.text = roomId;
            return RoomIdLabel.text.Length >= MaxIdCount;
        }
        /// <summary>
        /// 处理RoomId
        /// </summary>
        /// <returns></returns>
        protected string GetCurRoomId()
        {
            if (RoomIdMoreLabel.Length > 0)
            {
                var roomId = "";
                for (var i = 0; i < MaxIdCount; i++)
                {
                    roomId = string.Format("{0}{1}",roomId,RoomIdMoreLabel[i].text);
                }
                return roomId;
            }
            return RoomIdLabel.text;
        }

        public void OnClickWithInput()
        {
            var roomId = GetCurRoomId();
            if (roomId.Length < MaxIdCount) return;
            OnFindRoom();
        }

        protected virtual void OnFindRoom()
        { 
            int roomType;
            var roomCId = GetCurRoomId();
            if (!int.TryParse(roomCId, out roomType)) return;
            FindRoomById(roomType);
        }

        protected void FindRoomById(int roomType)
        {
            RoomListController.Instance.FindRoom(roomType, obj =>
            {
                Clear();
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
                var roomId = int.Parse(rid.ToString());
                YxDebug.LogError("加入房间的真实ID是" + roomId);
                if (roomId < 1)
                {
                    YxMessageBox.Show("查找异常！");
                    return;
                }
                if (NeedRoomInfo && data.ContainsKey("users"))
                {
                    var win = RoominfoWindow ?? CreateOtherWindow("RoomInfoWindow");
                    if (win != null)
                    {
                        data["_roomShowId"] = roomType;
                        win.UpdateView(data);
                        return;
                    }
                }
                var gameKey = (string)(data.ContainsKey("gameKey") ? data["gameKey"] : App.GameKey);
                RoomListController.Instance.JoinFindRoom(roomId, gameKey);
            });
        }


        public void OnOpenCreateWindow()
        {
            var win = YxWindowManager.OpenWindow("CreateRoomWindow", true);
            var createWin = (CreateRoomWindow)win;
            if (createWin == null) return;
            createWin.TabDefaultIndex = TabDefaultIndex;
        }

    }
}