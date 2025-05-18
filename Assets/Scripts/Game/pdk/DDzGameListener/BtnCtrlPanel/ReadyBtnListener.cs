using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.View;

namespace Assets.Scripts.Game.pdk.DDzGameListener.BtnCtrlPanel
{
    public class ReadyBtnListener : ServEvtListener
    {

        public GameObject ReadyBtnSprite;
        public GameObject LeaveRoomBtn;

        protected override void OnAwake()
        {
            PdkGameManager.AddOnGameInfoEvt(OnGameInfo);  
            PdkGameManager.AddOnGetRejoinDataEvt(OnRejoin);
            Ddz2RemoteServer.AddOnUserReadyEvt(OnUserReady);
        }

        private void OnUserReady(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if (data.ContainsKey(RequestKey.KeySeat) &&
                data.GetInt(RequestKey.KeySeat) == App.GetGameData<GlobalData>().GetSelfSeat)
            {
                ReadyBtnSprite.SetActive(false);
                LeaveRoomBtn.SetActive(false);
            }
        }

        private void OnRejoin(object sender, DdzbaseEventArgs args)
        {
            CheckReadyBtnShow(args.IsfObjData);
        }


        public override void RefreshUiInfo()
        {
            // throw new System.NotImplementedException();
        }

        /// <summary>
        /// 发送准备游戏信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnGameInfo(object sender, DdzbaseEventArgs args)
        {
            CheckReadyBtnShow(args.IsfObjData);
        }



        private void CheckReadyBtnShow(ISFSObject data)
        {
            if (data.ContainsKey(RequestKey.KeyUser))
            {
                var userData = data.GetSFSObject(RequestKey.KeyUser);
                if (!App.GetGameData<GlobalData>().IsStartGame
                    && userData.ContainsKey(RequestKey.KeyState) && userData.GetBool(RequestKey.KeyState) == false)
                {

                    if (data.ContainsKey(NewRequestKey.KeyCurRound) && data.GetInt(NewRequestKey.KeyCurRound) > 1)
                    {
                        OnReadyBtnClick();
                    }
                    else
                    {

                        ReadyBtnSprite.SetActive(true);
                        LeaveRoomBtn.SetActive(true);
                    }

                }

                else
                {
                    ReadyBtnSprite.SetActive(false);
                    LeaveRoomBtn.SetActive(false);
                }

            }
        }
        //发送准备---按钮
        public void OnReadyBtnClick()
        {
            GlobalData.ServInstance.SendPlayerReadyServCmd();
        }

        /// <summary>
        /// 点击解散房间按钮
        /// </summary>
        public void OnClickDismisRoomBtn()
        {
            var isSelfOwner = App.GetGameData<GlobalData>().IsSelfIsOwer;

            var msgstr = isSelfOwner ? "确定要解散房间么？" : "确定离开房间吗？";

            YxMessageBox.Show(msgstr, "", (box, btnName) =>
            {
                if (btnName == YxMessageBox.BtnLeft)
                {
                    if (!App.GetGameData<GlobalData>().IsStartGame)
                    {
                        if (isSelfOwner)
                            GlobalData.ServInstance.DismissRoom();
                        else
                            GlobalData.ServInstance.LeaveRoom();
                        return;
                    }
                    GlobalData.ServInstance.StartHandsUp(2);
                }
            }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
        }

    }
}
