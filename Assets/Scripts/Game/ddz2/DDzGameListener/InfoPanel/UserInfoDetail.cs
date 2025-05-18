using UnityEngine;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.InfoPanel
{
    public class UserInfoDetail : ServEvtListener
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

        protected override void OnAwake()
        {
            if (ShowParent)
            {
                ShowParent.SetActive(false);
            }
        }

        public void OnClickDisDetail()
        {
            ShowParent.SetActive(false);
        }

        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="username">玩家名称</param>
        /// <param name="userid">玩家id</param>
        /// <param name="ip">玩家ip</param>
        /// <param name="head"></param>
        public void ShowInfo(string username,string userid,string ip,YxBaseTextureAdapter head)
        {
            ShowParent.SetActive(true);
            UserName.text = username;
            UserID.text = userid;
            UserIP.text = ip;
            _userHead.mainTexture = head.GetTexture();
        }
        /// <summary>
        /// 
        /// </summary>
        public override void RefreshUiInfo()
        {
        }
    }
}
