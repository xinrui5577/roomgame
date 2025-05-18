using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.biji.EventII;
using Assets.Scripts.Game.biji.Modle;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Abstracts;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.biji.ui
{
    public class BjTableView : MonoBehaviour
    {
        public EventObject EventObj;

        public Transform CoinPos;
        public GameObject StartBtn;
        public GameObject ReadyBtn;
        public GameObject InviteBtn;
        public GameObject CopyBtn;
        public UIGrid TableGrid;
        public GameObject XiPaiBtn;
        public GameObject Arrows;
        public GameObject MenuBg;

        public UILabel RoomId;

        public UILabel Round;

        public UILabel Rule;

        private BjGameTable _gdata;
        private int _roomId;
        private bool _hasStart;

        protected void Start()
        {
            _gdata = App.GetGameData<BjGameTable>();
        }

        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "Rule":
                    OnRule(data.Data);
                    break;
                case "Start":
                    StartBtn.GetComponent<BoxCollider>().enabled = true;
                    StartBtn.GetComponent<UISprite>().color = Color.white;
                    break;
                case "Ready":
                    ReadyBtn.SetActive(true);
                    TableGrid.repositionNow = true;
                    break;
                case "RoomInfo":
                    OnFreshRoomInfo(data.Data);
                    break;
                case "FreshCurRound":
                    if (ReadyBtn.activeSelf) ReadyBtn.SetActive(false);
                    OnFreshCurRound(data.Data);
                    break;
                case "Result":
                    OnResult();
                    break;
            }
        }

        public void OnRule(object data)
        {
            var ruleData = (ISFSObject)data;
            var hasXiPai = ruleData.GetBool("xiPai");
            XiPaiBtn.SetActive(hasXiPai);
            _hasStart = ruleData.GetBool("start");
        }

        public void OnStartBtn()
        {
            StartBtn.SetActive(false);
            EventObj.SendEvent("ServerEvent", "StartGame", null);
        }

        protected void OnFreshRoomInfo(object data)
        {
            var roomData = (ISFSObject)data;
            _roomId = roomData.GetInt("roomId");
            var rule = roomData.GetUtfString("rule");
            var curRound = roomData.GetInt("curRound");
            var maxRound = roomData.GetInt("maxRound");
            var round = string.Format("{0}/{1}", curRound, maxRound);
            RoomId.text = _roomId.ToString();
            Round.text = round;
            Rule.text = rule;

            InviteBtn.SetActive(curRound <= 0);
            CopyBtn.SetActive(curRound <= 0);
            TableGrid.repositionNow = true;
        }

        protected void OnFreshCurRound(object data)
        {
            var roomData = data as YxCreateRoomInfo;
            if (roomData != null)
            {
                InviteBtn.SetActive(false);
                CopyBtn.SetActive(false);
                Round.text = string.Format("{0}/{1}", roomData.CurRound, roomData.MaxRound);
            }
        }


        public void OnReadyBtnClick()
        {
            EventObj.SendEvent("ServerEvent", "ReadyReq", null);
            if (_gdata.OwnerId == int.Parse(_gdata.GetPlayerInfo().UserId) && _hasStart && _gdata.CreateRoomInfo.CurRound == 0)
            {
                StartBtn.SetActive(true);
                StartBtn.GetComponent<BoxCollider>().enabled = false;
                TableGrid.repositionNow = true;
            }
            ReadyBtn.SetActive(false);
        }

        public void OnCreatRoomBack()
        {
            if (_gdata.IsCreatRoom)
            {
                YxMessageBox.Show(
                    "确定解散房间吗?",
                    null,
                    (window, btnname) =>
                    {
                        switch (btnname)
                        {
                            case YxMessageBox.BtnLeft:
                                EventObj.SendEvent("ServerEvent", "HupReq", 2);
                                break;
                        }
                    },
                    true,
                    YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                );
            }
            else
            {
                if (App.GameData.GStatus == YxEGameStatus.Over)
                {
                    App.QuitGameWithMsgBox();
                }
                else
                {
                    YxMessageBox.Show("游戏已经开始游戏结算后可以退出");
                }
            }

        }

        public void OnSettingClick()
        {
            EventObj.SendEvent("SettingEvent", "Open", null);
        }

        public void OnCopyRoomId()
        {
            Facade.Instance<YxGameTools>().PasteBoard = _roomId.ToString();

            YxMessageBox.Show(new YxMessageBoxData
            {
                Msg = "成功复制到剪切板",
            });
        }


        public void OnClickChatInvite()
        {
            var roomInfo = _gdata.CreateRoomInfo;
            YxTools.ShareFriend(roomInfo.RoomId.ToString(), roomInfo.RuleInfo);
        }

        public void OnResult()
        {
            EventObj.SendEvent("TableEvent", "Result", CoinPos);
        }

        public void OnShowRecord()
        {
            EventObj.SendEvent("RecordViewEvent", "Show", null);
        }

        public void Reset()
        {

        }
    }
}
