using Assets.Scripts.Common.Adapters;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.DataBundles;
using YxFramwork.Tool;

#pragma warning disable 649


namespace Assets.Scripts.Game.paijiu
{

    public class HistoryUser : MonoBehaviour
    {

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
            PaiJiuUserInfo paiJiuUser = userInfo.PaiJiuUserInfo;
            PortraitDb.SetPortrait(paiJiuUser.AvatarX, _headImage, paiJiuUser.SexI);

            _nameLabel.text = userInfo.PaiJiuUserInfo.NickM;

            _scoreLabel.text =YxUtiles.ReduceNumber(userInfo.Score);
        }
    }
}