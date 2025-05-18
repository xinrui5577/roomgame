using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Tea;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.TeaLq
{
    public class TeaDeskItem : YxView
    {
        public List<TeaSeatView> SeatViews;
        public NguiLabelAdapter TableNum;
        public NguiLabelAdapter GameState;
        public GameObject QuitBtn;
        public GameObject StartBtn;
        public UIButton DeskBtn;
        public string BackgroundPrefix;
        [HideInInspector]
        public int TableIndex;
        [HideInInspector]
        public RoomInfoData RoomInfoData;
        private bool _isShow;
        private float _lastSecond;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var dic = Data as Dictionary<string, object>;
            RoomInfoData = new RoomInfoData();
            RoomInfoData.ParseGameServerData(dic);
            if (int.Parse(RoomInfoData.CurRound) == 0)
            {
                GameState.gameObject.SetActive(false);
            }
            else
            {
                var info = string.Format("牌局进行中({0})", RoomInfoData.GameRound);
                GameState.gameObject.SetActive(true);
                GameState.TrySetComponentValue(info);
            }
            var index = string.Format("第{0}桌", GetIndex(RoomInfoData.Index));
            TableIndex = RoomInfoData.Index;
            TableNum.name = RoomInfoData.Index.ToString();
            TableNum.TrySetComponentValue(index);
            DeskBtn.normalSprite = string.Format("{0}{1}", BackgroundPrefix, RoomInfoData.UserNum);
            foreach (var seatView in SeatViews)
            {
                seatView.gameObject.SetActive(false);
            }
            SeatViews[RoomInfoData.UserNum - 2].gameObject.SetActive(true);
        }


        public void ShowQuitBtn(bool isShow)
        {
            _isShow = isShow;
        }

        public void FreshSeatView(TeaTableSeatData teaTableSeatData)
        {
            if (RoomInfoData == null) return;
            if (QuitBtn)
            {
                gameObject.GetComponent<BoxCollider>().enabled = !_isShow;
                QuitBtn.SetActive(_isShow);
                StartBtn.SetActive(_isShow);
            }
            SeatViews[RoomInfoData.UserNum - 2].UpdateView(teaTableSeatData);
        }

        public void OnJoinRoom()
        {
            YxMessageBox.Show("你是否确定手动进入房间等待其他玩家，进入后有可能人数不满也将开始游戏！", "", (mesBox, btnName) =>
            {
                switch (btnName)
                {
                    case YxMessageBox.BtnLeft:
                        var nowSecond = Time.time;
                        if (nowSecond - _lastSecond < 3f)
                        {
                            YxMessageBox.Show("您的操作过于频繁 请稍后点击");
                            return;
                        }
                        _lastSecond = nowSecond;
                        var dic = new Dictionary<string, object>();
                        dic["floor"] = TeaMainPanel.Floor;
                        dic["teaId"] = TeaMainPanel.CurTeaId;
                        Facade.Instance<TwManager>().SendAction("group.joinRoom", dic, (obj1) =>
                        {
                            var info = obj1 as Dictionary<string, object>;
                            if (info == null) return;
                            if (info.ContainsKey("joinRoom"))
                            {
                                var joinRoom = int.Parse(info["joinRoom"].ToString()) == 1;
                                if (joinRoom)
                                {
                                    int roomType;
                                    var roomCId = RoomInfoData.RoomId;
                                    if (!int.TryParse(roomCId, out roomType)) return;
                                    RoomListController.Instance.FindRoom(roomType, obj =>
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
                                        var roomId = int.Parse(rid.ToString());
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
                        });
                        break;
                }
            }, true, YxMessageBox.RightBtnStyle | YxMessageBox.LeftBtnStyle);

           
        }

        private string GetIndex(int num)
        {
            string index = "";
            switch (num)
            {
                case 0:
                    index = "一";
                    break;
                case 1:
                    index = "二";
                    break;
                case 2:
                    index = "三";
                    break;
                case 3:
                    index = "四";
                    break;
                case 4:
                    index = "五";
                    break;
                case 5:
                    index = "六";
                    break;
                case 6:
                    index = "七";
                    break;
                case 7:
                    index = "八";
                    break;
            }
            return index;
        }
    }
}
