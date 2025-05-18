using System.Collections.Generic;
using YxFramwork.Framework.Core;
using YxFramwork.Controller;
using YxFramwork.Manager;
using YxFramwork.Common;
using YxFramwork.View;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class StateGameContinue : FsmState
    {
        private FindState mFindState = FindState.None;
        private bool mTipOpening = false;
        private bool mFlag = false;

        public enum FindState
        {
            None,
            Tip,
            Success,
        }

        public override void OnEnter(FsmStateArgs args)
        {           
            //注册事件           
            GameCenter.EventHandle.Subscriber((int)EventKeys.OnCreateNewGame, OnCreateNewGame);
            //开始发送消息
            Facade.Instance<TWMessageManager>().StartRollNotice();
            ContinueTaskManager.NewTask().AppendFuncTask(CheckContinueGame).Start();
            mFlag = true;
        }

        public override void OnLeave(bool isShutdown)
        {
            ResetGameData();
            //移除事件            
            GameCenter.EventHandle.Unsubscriber((int)EventKeys.OnCreateNewGame, OnCreateNewGame);
            //停止发送消息
            Facade.Instance<TWMessageManager>().StopRollNotice();
            //执行继承IContinueGameCycle接口脚本
            GameCenter.Lifecycle.ContinueGameCycle();           
        }

        /// <summary>
        /// 重置游戏数据
        /// </summary>
        private void ResetGameData()
        {
            mFindState = FindState.None;
            mTipOpening = false;
            mFlag = false;
        }

        private void OnCreateNewGame(EvtHandlerArgs args)
        {
            var apiInfo = GetPlayersInfo();
            Facade.Instance<TwManager>().SendAction("mahjongwm.findUserRoom", apiInfo, data =>
            {
                if (data != null)
                {
                    RoomListController.Instance.JoinFindRoom(int.Parse(data.ToString()), App.GameKey);
                }
                else
                {
                    YxMessageBox.Show(new YxMessageBoxData()
                    {
                        Msg = "是否使用上局游戏规则创建新房间？",
                        Listener = (box, btnName) =>
                        {
                            //返回游戏大厅
                            if (btnName == YxMessageBox.BtnRight)
                            {
                                LocalSavePlayersId();
                                //保存teaId值,用于再大厅创建茶馆Id
                                PlayerPrefs.SetString(AnalysisKeys.TeaId, GameCenter.DataCenter.Room.TeaID);
                                App.QuitGameWithEvent((obj) =>
                                {
                                    YxWindowManager.OpenWindow("CreateRoomWindow");
                                });
                            }
                            //直接创建游戏房间
                            if (btnName == YxMessageBox.BtnLeft)
                            {
                                GameCenter.Instance.SetIgonreReconnectState(false);
                                LocalSavePlayersId();
                                OnCreateNewGame();
                            }
                        },
                        IsTopShow = true,
                        BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                    });
                }
            });
        }

        private void LocalSavePlayersId()
        {
            var data = "";
            var db = GameCenter.DataCenter;
            for (int i = 1; i < db.MaxPlayerCount; i++)
            {
                data += db.Players[i].Id + "|";
            }
            MahjongUtility.RecordCountineGameData = data;
        }

        private Dictionary<string, object> GetPlayersInfo()
        {
            List<int> ids = new List<int>();
            var db = GameCenter.DataCenter;
            for (int i = 0; i < db.MaxPlayerCount; i++)
            {
                if (db.Players[i] != null)
                {
                    ids.Add(db.Players[i].Id);
                }
            }
            Dictionary<string, object> dic = new Dictionary<string, object>()
            {
                { "userIds", ids.ToArray() },
                { "roomId", db.Room.RoomID }
            };
            return dic;
        }

        private IEnumerator<float> CheckContinueGame()
        {
            var apiInfo = GetPlayersInfo();
            while (mFlag)
            {
                yield return 1f;
                Facade.Instance<TwManager>().SendAction("mahjongwm.checkContinueGame", apiInfo, data =>
                {
                    if (data == null)
                    {
                        mFindState = FindState.None;
                        return;
                    }
                    int id = 0;
                    string message = string.Empty;
                    var dic = (Dictionary<string, object>)data;
                    if (dic.ContainsKey("message"))
                    {
                        mFindState = FindState.Tip;
                        message = dic["message"].ToString();
                    }
                    if (dic.ContainsKey("roomId"))
                    {
                        mFindState = FindState.Success;
                        id = (int)dic["roomId"];
                    }
                    switch (mFindState)
                    {
                        case FindState.Tip: OnOpenTip(message); break;
                        case FindState.Success:
                            OnFindNewGame(id, message);
                            mFlag = false;
                            break;
                    }
                }, false, null, false, null);
            }
        }

        private void OnOpenTip(string message)
        {
            if (mTipOpening || mFindState == FindState.Success) return;
            mTipOpening = true;
            YxMessageBox.Show(new YxMessageBoxData()
            {
                Msg = message,
                Listener = (box, btnName) =>
                {
                    if (btnName == YxMessageBox.BtnRight)
                    {
                        App.QuitGame();
                    }
                    if (btnName == YxMessageBox.BtnLeft)
                    {
                    }
                },
                IsTopShow = true,
                BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
            });
        }

        private void OnFindNewGame(int roomid, string message)
        {
            YxMessageBox.Show(new YxMessageBoxData()
            {
                Msg = message,
                Listener = (box, btnName) =>
                {
                    if (btnName == YxMessageBox.BtnRight)
                    {
                        App.QuitGame();
                    }
                    if (btnName == YxMessageBox.BtnLeft)
                    {
                        RoomListController.Instance.JoinFindRoom(roomid, App.GameKey);
                    }
                },
                IsTopShow = true,
                BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
            });
        }

        private void OnCreateNewGame()
        {
            var db = GameCenter.DataCenter;
            if (db.Room.RoomType == MahRoomType.FanKa)
            {
                var data = new Dictionary<string, object>();
                data["cargs"] = db.ConfigData.Cargs;
                data["ruleId"] = App.GameKey + "_0";
                data["num"] = db.Room.ConsumeNum;
                data["teaId"] = db.Room.TeaID;
                RoomListController.Instance.CreateRoomFromGame(data);
            }
        }
    }
}