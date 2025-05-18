using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.BtnCtrlPanel
{
    public class ReadyBtnListener : ServEvtListener
    {

        /// <summary>
        /// 准备按钮
        /// </summary>
        [SerializeField]
        protected GameObject ReadyBtnSprite;

        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyGetGameInfo, OnGameInfo);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnRejoin, OnRejoin);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnUserReady, OnUserReady);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyShowReadyBtn, ShowReadyBtn);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeAllocate, HideReadyBtn);
        }

        private void HideReadyBtn(DdzbaseEventArgs args)
        {
            SetReadyBtnActive(false);
        }

        private void ShowReadyBtn(DdzbaseEventArgs args)
        {
            SetReadyBtnActive(true);
        }


        private void OnUserReady(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if (data.ContainsKey(RequestKey.KeySeat) &&
                data.GetInt(RequestKey.KeySeat) == App.GetGameData<DdzGameData>().SelfSeat)
            {
                SetReadyBtnActive(false);
            }
        }

        private void OnRejoin(DdzbaseEventArgs args)
        {
            CheckReadyBtnShow(args.IsfObjData);
        }


        public override void RefreshUiInfo()
        {
        }

        /// <summary>
        /// 发送准备游戏信息
        /// </summary>
        /// <param name="args"></param>
        private void OnGameInfo(DdzbaseEventArgs args)
        {
            CheckReadyBtnShow(args.IsfObjData);
        }



        private void CheckReadyBtnShow(ISFSObject data)
        {
            bool showReadyBtn = true;
            if (data.ContainsKey(NewRequestKey.KeyCargs2))
            {
                var cargsInfo = data.GetSFSObject(NewRequestKey.KeyCargs2);
                if (cargsInfo.ContainsKey(NewRequestKey.KeyReadyBtn))
                    showReadyBtn = cargsInfo.GetUtfString(NewRequestKey.KeyReadyBtn) == "1";
            }

            //如果不显示准备按钮，则直接自动准备
            if (!showReadyBtn)
            {
                App.GetRServer<DdzGameServer>().SendPlayerReadyServCmd();
                return;
            }

            if (!data.ContainsKey(RequestKey.KeyUser)) return;
            var userData = data.GetSFSObject(RequestKey.KeyUser);
            bool active = !App.GetGameData<DdzGameData>().IsGameStart           //游戏没有开始
                          && userData.ContainsKey(RequestKey.KeyState) &&       
                          userData.GetBool(RequestKey.KeyState) == false;       //检测玩家状态没有准备

            SetReadyBtnActive(active);
        }


        private void SetReadyBtnActive(bool value)
        {
            ReadyBtnSprite.SetActive(value);
        }

        public void OnReadyBtnClick()
        {
            App.GetRServer<DdzGameServer>().SendPlayerReadyServCmd();
        }

    }
}
