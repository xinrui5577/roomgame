using UnityEngine;
using System.Collections.Generic;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.GangWu
{
    public class HistoryUser : MonoBehaviour
    {

        /// <summary>
        /// 扑克
        /// </summary>
        [SerializeField]
        private PokerCard[] _pokers = new PokerCard[13];

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
        private GameObject _foldMark;

        public void InitUser(HistoryUserInfo userInfo)
        {
            var user = userInfo.UserInfo;
            PortraitDb.SetPortrait(user.AvatarX, _headImage, user.SexI);
            _nameLabel.text = user.NickM;
            SetScoreLabel(_scoreLabel, userInfo.Score);

            List<int> pokers = userInfo.Pokers;
            if (pokers == null || pokers.Count <= 0)
                return;
            for (int i = 0; i < _pokers.Length; i++)
            {
                if (i > pokers.Count - 1)
                {
                    _pokers[i].gameObject.SetActive(false);
                    continue;
                }

                PokerCard poker = _pokers[i];
                poker.SetCardId(pokers[i]);
                poker.SetCardFront();
                poker.SetCardDepth(i * 2 + 100);
                poker.gameObject.SetActive(true);
            }
            _foldMark.SetActive(userInfo.IsFold);
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 设置分数Label
        /// </summary>
        /// <param name="label"></param>
        /// <param name="score"></param>
        void SetScoreLabel(UILabel label,int score)
        {
            //*
            label.text = YxUtiles.ReduceNumber(score);//App.GetGameData<GlobalData>().GetShowGold(score);
            /*/
            //输赢显示不同分数
            if (score < 0)
            {
                label.text = score.ToString();
                label.gradientTop = Tools.ChangeToColor(0x6FFBF1);
                label.gradientBottom = Tools.ChangeToColor(0x0090ff);
                label.effectColor = Tools.ChangeToColor(0x002ea3);
            }
            else
            {
                label.text = "+" + score;
                label.gradientTop = Tools.ChangeToColor(0xffff00);
                label.gradientBottom = Tools.ChangeToColor(0xff9600);
                label.effectColor = Tools.ChangeToColor(0x831717);
            }
            //*/
        }
        
    }
}