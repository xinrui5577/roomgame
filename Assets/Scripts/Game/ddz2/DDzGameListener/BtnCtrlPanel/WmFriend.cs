using System.Collections.Generic;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.BtnCtrlPanel
{
    public class WmFriend : ServEvtListener
    {

        /// <summary>
        /// 牌友邀请按钮
        /// </summary>
        [SerializeField]
        protected GameObject WmFriendBtn;

        /// <summary>
        /// 窗口名称
        /// </summary>
        [SerializeField]
        protected string WindowName = "WmFriendWindow";

        /// <summary>
        /// 是否有牌友邀请模式
        /// </summary>
        private bool _couldSendWmInfo;

        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyGetGameInfo, OnGetGameInfoData);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnRejoin, OnGetGameInfoData);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeAllocate, OnAlloCateCds);
        }

        private void OnAlloCateCds(DdzbaseEventArgs args)
        {
            if (!App.GetGameData<DdzGameData>().IsRoomGame) return;     //只有房卡模式才能添加牌友

            if (WmFriendBtn == null) return;
            WmFriendBtn.SetActive(false);

            if (!_couldSendWmInfo) return;

            var idList = new List<int>();
            var playerList = App.GameData.PlayerList;
            foreach (var player in playerList)
            {
                if (player.Info == null) continue;
                idList.Add(player.Info.Id);
            }

            var dic = new Dictionary<string, object>() {{"bundleID", Application.bundleIdentifier}, {"ID",idList} };

            Facade.Instance<TwManager>().SendAction("mahjongwm.addWmFriends", dic, _ => { });
            _couldSendWmInfo = false;
        }

        private void OnGetGameInfoData(DdzbaseEventArgs args)
        {
            if (WmFriendBtn == null) return;

            var data = args.IsfObjData;

            if (data.ContainsKey("cargs2"))
            {
                var config = data.GetSFSObject("cargs2");
                if (config.ContainsKey("-friends"))
                {
                    int friends;
                    _couldSendWmInfo = int.TryParse(config.GetUtfString("-friends"), out friends) && friends > 0;
                    WmFriendBtn.SetActive(_couldSendWmInfo && data.GetInt(DDz2Common.NewRequestKey.KeyCurRound) < 1);
                }
            }
        }

        public void OnClickWmFriendBtn()
        {
            Facade.Instance<TwManager>().SendAction("mahjongwm.WmFriends", new Dictionary<string, object>(),DealWmFriendInfo);
        }

        private void DealWmFriendInfo(object msg)
        {
            var dic = (Dictionary<string, object>) msg;
            if (dic.ContainsKey("data"))
            {
                List<object> list = (List<object>) dic["data"];
                var window = YxWindowManager.OpenWindow(WindowName);
                if (window == null)
                {
                    YxDebug.LogError(string.Format("nldld : The window '{0}' you try to open is null !!", WindowName));
                    return;
                }
                window.UpdateView(list);
            }
        }

        public override void RefreshUiInfo()
        {
        }
    }
}
