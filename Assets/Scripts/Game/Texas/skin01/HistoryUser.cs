using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Game.Texas.Main;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.Texas.skin01
{
    public class HistoryUser : MonoBehaviour
    {

        /// <summary>
        /// 扑克
        /// </summary>
        [SerializeField]
        private PokerCard[] _pokers = new PokerCard[2];

        /// <summary>
        /// 名字
        /// </summary>
        [SerializeField]
        private UILabel _nameLabel; 

        /// <summary>
        /// 头像
        /// </summary>
        [SerializeField]
        private YxBaseTextureAdapter _headImage;

        /// <summary>
        /// 分数Label
        /// </summary>
        [SerializeField]
        private UILabel _scoreLabel;

        /// <summary>
        /// 弃牌标记
        /// </summary>
        [SerializeField]
        private UISprite _typeSpr;

        public void InitUser(HistoryUserInfo historyUserInfo)
        {

            var user = historyUserInfo.UserInfo;
            PortraitDb.SetPortrait(user.AvatarX, _headImage, user.SexI);
            _nameLabel.text = user.NickM;
            
            SetScoreLabel(_scoreLabel, historyUserInfo.Score); //设置输赢分数

            SetUserPokers(historyUserInfo.IsFold,historyUserInfo.Pokers); //设置玩家手牌信息

            //设置玩家牌型或弃牌图片
            SetTypeSprite(historyUserInfo.IsFold ? "ct_Fold" : ((PokerType) historyUserInfo.PokerType).ToString());
            _typeSpr.MakePixelPerfect();
            
            gameObject.SetActive(true);
        }

        void SetTypeSprite(string spriteName)
        {
            if (_typeSpr == null)
                return;
            _typeSpr.spriteName = spriteName;
            _typeSpr.MakePixelPerfect();
        }

        /// <summary>
        /// 设置分数Label
        /// </summary>
        /// <param name="label"></param>
        /// <param name="score"></param>
        void SetScoreLabel(UILabel label,int score)
        {
            label.text = YxUtiles.ReduceNumber(score);
        }


        /// <summary>
        /// 设置玩家手牌信息
        /// </summary>
        /// <param name="isFold">是否弃牌</param>
        /// <param name="pokers">手牌</param>
        void SetUserPokers(bool isFold, List<int> pokers)
        {
            if (pokers == null || pokers.Count <= 0)
                return;
            for (var i = 0; i < _pokers.Length; i++)
            {
                //隐藏多出的手牌
                if (i > pokers.Count - 1)
                {
                    _pokers[i].gameObject.SetActive(false);
                    continue;
                }

                var poker = _pokers[i];
                poker.SetCardId(isFold ? 0 : pokers[i]);
                poker.SetCardFront();
                poker.SetCardDepth(i * 2 + 100);
                poker.gameObject.SetActive(true);
            }
        }
        
    }
}