using System;
using System.Collections.Generic;
using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.pdk.DDzGameListener.BtnCtrlPanel
{
    public class CallScoreListener : ServEvtListener
    {
        [SerializeField]
        protected GameObject NoCallBtn;
        [SerializeField]
        protected GameObject Call1Btn;
        [SerializeField]
        protected GameObject Call2Btn;
        [SerializeField]
        protected GameObject Call3Btn;

/*        protected override void OnAwake()
        {
            //跑得快暂时弃用这个脚本
            return;

            PdkGameManager.AddOnGetRejoinDataEvt(OnGetRejoionData);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeGrabSpeaker, OnTypeGrabSpeaker);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeGrab, OnTypeGrab);

            UIEventListener.Get(NoCallBtn).onClick = OnCallScoreClick;
            UIEventListener.Get(Call1Btn).onClick = OnCallScoreClick;
            UIEventListener.Get(Call2Btn).onClick = OnCallScoreClick;
            UIEventListener.Get(Call3Btn).onClick = OnCallScoreClick;
        }*/

        /// <summary>
        /// 叫分按钮点击
        /// </summary>
        /// <param name="gob"></param>
        private void OnCallScoreClick(GameObject gob)
        {
            int calScore = 0;
            if (ReferenceEquals(gob, NoCallBtn)) calScore = 0;
            else if (ReferenceEquals(gob, Call1Btn)) calScore = 1;
            else if (ReferenceEquals(gob, Call2Btn)) calScore = 2;
            else if (ReferenceEquals(gob, Call3Btn)) calScore = 3;

            //向服务器发送叫分信息
            GlobalData.ServInstance.CallGameScore(App.GetGameData<GlobalData>().GetSelfSeat, calScore);
        }

        /// <summary>
        /// 存储与界面显示相关的服务器消息缓存
        /// </summary>
        protected ISFSObject ServDataTemp;
        

        private void OnGetRejoionData(object obj,DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
           
            if ( data.ContainsKey(NewRequestKey.KeyState)
                && data.GetInt(NewRequestKey.KeyState) == GlobalConstKey.StatusChoseBanker)
            {
                if (!DDzUtil.IsServDataContainAllKey(
                new[]
                        {
                            NewRequestKey.KeyCurrp, NewRequestKey.KeyQt,
                            NewRequestKey.KeyMinScore
                        }, data))
                {
                    SetAllBtnsActive(false);
                    return;
                }


                //当前谁发言
                int curCallSeat = data.GetInt(NewRequestKey.KeyCurrp);
                if (App.GetGameData<GlobalData>().GetSelfSeat == curCallSeat)
                {
                    ServDataTemp = data;
                    RefreshUiInfo();
                }
                else
                {
                    SetAllBtnsActive(false);
                }
            }

        }

        /// <summary>
        /// 当轮到某人要准备要叫分时
        /// </summary>
        protected void OnTypeGrabSpeaker(object obj, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            if (!DDzUtil.IsServDataContainAllKey(
                new[]
                    {
                        RequestKey.KeySeat, NewRequestKey.KeyTttype
                    }, data))
            {
                return;
            }

            if (App.GetGameData<GlobalData>().GetSelfSeat != data.GetInt(RequestKey.KeySeat))
            {
                SetAllBtnsActive(false);
                return;
            }

            if (ServDataTemp == null) ServDataTemp = new SFSObject();

            ServDataTemp = data;
            //把与"qt"相同引用值的 "ttype" 的值赋值过来
            ServDataTemp.PutInt(NewRequestKey.KeyQt, data.GetInt(NewRequestKey.KeyTttype));
            RefreshUiInfo();
        }


        /// <summary>
        /// 当有人叫了分了
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        protected void OnTypeGrab(object obj, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            //如果是玩家自己叫了分了则隐藏叫分面板
            if (data.ContainsKey(RequestKey.KeySeat) && 
                data.GetInt(RequestKey.KeySeat) == App.GetGameData<GlobalData>().GetSelfSeat)
            {
                SetAllBtnsActive(false);
            }
        }





        public override void RefreshUiInfo()
        {
            if (ServDataTemp == null) return;
            var curPokerGameType = ServDataTemp.GetInt(NewRequestKey.KeyQt);

            switch (curPokerGameType)
            {
                case (int)GlobalConstKey.GameType.CallScoreWithFlow:
                    {
                        SetCallScoreWithFlowUi();
                        break;
                    }
            }

            SetAllBtnsActive(true);
        }

        /// <summary>
        /// 叫分带分流时的ui设置
        /// </summary>
        private void SetCallScoreWithFlowUi()
        {
            if(!ServDataTemp.ContainsKey(NewRequestKey.KeyMinScore)) throw new Exception("缓存信息中没有minscore无法判断callscore按钮显示");
            int score = ServDataTemp.GetInt(NewRequestKey.KeyMinScore);

            ResetBtnState();

            switch (score)
            {
                case 1:
                    {
                        DisableBtns(new[] {Call1Btn});
                        break;
                    }
                case 2:
                    {
                        DisableBtns(new[] {Call1Btn, Call2Btn});
                        break;
                    }
            }

        }
       
        
        /// <summary>
        /// 重置按钮状态
        /// </summary>
        void ResetBtnState()
        {
            var btngobs = new[] {NoCallBtn, Call1Btn, Call2Btn, Call3Btn};
            foreach (var btngob in btngobs)
            {
                btngob.SetActive(true);
                btngob.GetComponent<UISprite>().color = new Color(1, 1, 1);
                btngob.GetComponent<BoxCollider>().enabled = true;
            }
        }

        /// <summary>
        /// 让某些按钮失效
        /// </summary>
        /// <param name="btnGobs"></param>
        void DisableBtns(IEnumerable<GameObject> btnGobs)
        {
            foreach (var btngob in btnGobs)
            {
                btngob.GetComponent<BoxCollider>().enabled = false;
                btngob.GetComponent<UISprite>().color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
        
        /// <summary>
        /// 设置所有叫分按钮的显示状态
        /// </summary>
        /// <param name="isActive"></param>
        void SetAllBtnsActive(bool isActive)
        {
            NoCallBtn.SetActive(isActive);
            Call1Btn.SetActive(isActive);
            Call2Btn.SetActive(isActive);
            Call3Btn.SetActive(isActive);
        }

    }
}
