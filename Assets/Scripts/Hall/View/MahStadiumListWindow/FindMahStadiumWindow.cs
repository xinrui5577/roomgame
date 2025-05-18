using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.MahStadiumListWindow
{
    public class FindMahStadiumWindow : YxNguiWindow
    {
        [Tooltip("MainWindow需要隐藏的内容")]
        public GameObject[] HideUIs;
        /// <summary>
        /// 房间id的Label
        /// </summary>
        [Tooltip("房间id的Label")]
        public UILabel RoomIdLabel;
        /// <summary>
        /// 房间功能的label
        /// </summary>
        [Tooltip("此界面的功能标签")]
        public UILabel RoomAction;
        /// <summary>
        /// 房间id最大位数
        /// </summary>
        [Tooltip("房间id最大位数")]
        public int MaxIdCount = 6;
        /// <summary>
        /// 房间id最大位数
        /// </summary>
        [Tooltip("输入键")]
        public UIButton[] Keyboards;

        public string MahNum;
        private int _roomState;
        private string _roomNum;
        protected override void OnAwake()
        {
            base.OnAwake();
            var count = Keyboards.Length;
            for (var i = 0; i < count; i++)
            {
                var btn = Keyboards[i];
                if (btn == null) continue;
                UIEventListener.Get(btn.gameObject).onClick = OnClick;
            }
        }

        public void OnClick(GameObject go)
        {
            OnClickWithName(go.name);
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
                    if (Input(btnName))
                    {
                        OnFindRoom();
                    }
                    break;
            }
        }

        private void Delete()
        {
            var cur = RoomIdLabel.text;
            if (cur.Length < 1) return;
            RoomIdLabel.text = cur.Remove(cur.Length - 1);
        }

        private void Clear()
        {
            RoomIdLabel.text = "";
        }

        private bool Input(string num)
        {
            if (RoomIdLabel.text.Length >= MaxIdCount) return true;
            var roomId = string.Format("{0}{1}", RoomIdLabel.text, num);
            RoomIdLabel.text = roomId;
            return RoomIdLabel.text.Length >= MaxIdCount;
        }

        private string GetCurRoomId()
        {
            return RoomIdLabel.text;
        }

        public void OnClickWithInput()
        {
            var roomId = GetCurRoomId();
            if (roomId.Length < MaxIdCount) return;
            OnFindRoom();
        }

        public void OnFindRoom()
        {
            string roomCId = GetCurRoomId();//MahNum
            if (roomCId.Equals("") && !MahNum.Equals(""))
            {
                roomCId = MahNum;
            }
            var dic = new Dictionary<string, object>();
            if (_roomState == 1)
            {
                dic["mah_id"] = _roomNum;
                dic["comm_id"] = roomCId;
            }
            else
            {
                dic["mah_id"] = roomCId;
            }
            Facade.Instance<TwManager>().SendAction("mahjongwm.findWmRoom", dic, info =>
            {
                var data = info as IDictionary<string, object>;
                if (data == null)
                {
                    YxMessageBox.Show(_roomState == 1 ? "您输入的密码不正确！！" : "没有找到麻将馆！！");
                    return;
                }
                var str = data.ContainsKey(RequestKey.KeyMessage) ? data[RequestKey.KeyMessage] : null;
                if (str != null)
                {
                    YxMessageBox.Show(str.ToString());
                    return;
                } 
                if (_roomState!=1)
                {
                    var dataInfo = data["data"] as Dictionary<string, object>;
                    if (dataInfo != null && (dataInfo.ContainsKey("is_usepwd") && int.Parse(dataInfo["is_usepwd"].ToString()) == 1))
                    {
                        _roomNum = roomCId;
                        RoomAction.text = "输入麻将馆密码";
                        RoomIdLabel.text = "";
                        _roomState = 1;
                    }
                    else
                    {
                        var win = (MahStadiumCreatGameWindow)CreateOtherWindow("MahStadiumCreatGameWindow");
                        win.UpdateView(info);
                        Close();
                    }
                }
                else
                {
                    var win = (MahStadiumCreatGameWindow)CreateOtherWindow("MahStadiumCreatGameWindow");
                    win.UpdateView(info);
                    Close();

                    RoomIdLabel.text = "";
                    RoomAction.text = "输入麻将馆号";
                    _roomState = 0;
                }
               
            });
        }
        protected override void OnEnable()
        {
            YxWindowUtils.DisplayUI(HideUIs, false);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            YxWindowUtils.DisplayUI(HideUIs);
            RoomIdLabel.text = "";
            RoomAction.text = "输入麻将馆号";
            _roomState = 0;
        }
    }
}
