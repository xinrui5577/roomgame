using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.fillpit.skin1
{
    public class WmFriend : MonoBehaviour
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

        /// <summary>
        /// 游戏开始时,发送玩家数据
        /// </summary>
        public void OnGameBegin()
        {
            if (!App.GetGameData<FillpitGameData>().IsRoomGame) return;     //只有房卡模式才能添加牌友

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

        public void OnGetGameInfoData(ISFSObject data)
        {
            if (WmFriendBtn == null) return;
            if (data.ContainsKey("cargs2"))
            {
                var config = data.GetSFSObject("cargs2");
                if (config.ContainsKey("-friends"))
                {
                    int friends;
                    _couldSendWmInfo = int.TryParse(config.GetUtfString("-friends"), out friends) && friends > 0;
                    WmFriendBtn.SetActive(_couldSendWmInfo && !App.GetGameData<FillpitGameData>().IsPlayed);
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
    
    }
}
