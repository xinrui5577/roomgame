using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Game.Component.UserInfo
{
    public class UserInfoDetail : MonoSingleton<UserInfoDetail>
    {
        [SerializeField]
        private UILabel UserIP;
        [SerializeField]
        private UILabel UserID;
        [SerializeField]
        private UILabel UserName;
        [SerializeField]
        private UITexture _userHead;

        public GameObject ShowParent;

        public void OnClickDisDetail()
        {
            ShowParent.TrySetComponentValue(!ShowParent.activeInHierarchy);
        }
        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="name">玩家名称</param>
        /// <param name="userid">玩家id</param>
        /// <param name="ip">玩家ip</param>
        public void ShowInfo(string name,string userid,string ip,Texture head)
        {
            OnClickDisDetail();
            UserName.text = name;
            UserID.text = userid;
            UserIP.text = ip;
            _userHead.mainTexture = head;
        }
    }
}
