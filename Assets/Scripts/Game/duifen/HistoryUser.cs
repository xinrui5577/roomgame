using Assets.Scripts.Common.Adapters;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.DataBundles;

#pragma warning disable 649

namespace Assets.Scripts.Game.duifen
{
    public class HistoryUser : MonoBehaviour {

        /// <summary>
        /// 名字
        /// </summary>
        [SerializeField]
        private UILabel _nameLabel;


        /// <summary>
        /// 得分
        /// </summary>
        [SerializeField]
        private UILabel _scoreLabel;


        /// <summary>
        /// 头像
        /// </summary>
        [SerializeField]
        private NguiTextureAdapter _headImage;


 

        public void InitUser(HistoryUserInfo userInfo)
        {
            var user = userInfo.UserInfo;
            PortraitDb.SetPortrait(user.AvatarX, _headImage, user.SexI);

            _nameLabel.text = userInfo.UserInfo.NickM;

            _scoreLabel.text = App.GetGameData<DuifenGlobalData>().GetShowGoldValue(userInfo.Score);
        }
    }
}
