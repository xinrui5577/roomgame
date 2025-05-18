using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Game.sss.Tool;
using UnityEngine;
using YxFramwork.Common.DataBundles;
using YxFramwork.Tool;

#pragma warning disable 649

namespace Assets.Scripts.Game.sss
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
        private NguiTextureAdapter _icon;

        /// <summary>
        /// 分数Label
        /// </summary>
        [SerializeField]
        private UILabel _scoreLabel;

        /// <summary>
        /// 每道的得分分数
        /// </summary>
        [SerializeField] private UILabel[] _normalLabels;
        /// <summary>
        /// 每道的额外加分
        /// </summary>
        [SerializeField] private UILabel[] _addLabels;

        /// <summary>
        /// 每道牌的牌型
        /// </summary>
        [SerializeField] private UISprite[] _dunTypeSprites;

        /// <summary>
        /// 每道牌数据信息父层级
        /// </summary>
        [SerializeField] private GameObject _dunInfoParent;

        [SerializeField] private UISprite _specialSprite;

        [SerializeField] private UILabel _shootTime;

        [SerializeField] private UILabel _getShootTime;

        [SerializeField] private GameObject _swatMark;

        [SerializeField] private GameObject _bankerMark;

        /// <summary>
        /// 初始化玩家信息
        /// </summary>
        /// <param name="userInfo"></param>
        public void InitUser(HistoryUserInfo userInfo)
        {
            SetUserInfo(userInfo);

            SetUserCardPokerInfo(userInfo);

            SetBankerMarkActive(userInfo);

            if (userInfo.Sprcial > (int)CardType.none)
            {
                SetSpecialSprite(userInfo);
            }
            else
            {
                SetDunInfo(userInfo);
            }
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 设置庄标记的显示
        /// </summary>
        /// <param name="userInfo"></param>
        private void SetBankerMarkActive(HistoryUserInfo userInfo)
        {
            if (_bankerMark != null)
                _bankerMark.SetActive(userInfo.IsBanker);
        }

        /// <summary>
        /// 设置玩家信息
        /// </summary>
        /// <param name="userInfo"></param>
        private void SetUserInfo(HistoryUserInfo userInfo)
        {
            SssUserInfo sssUser = userInfo.SssUserInfo;
            PortraitDb.SetPortrait(sssUser.AvatarX, _icon, sssUser.SexI);
            _nameLabel.text = sssUser.NickM;
            SetTotalScoreLabel(_scoreLabel, userInfo.Score);     //设置总分
        }

        /// <summary>
        /// 显示玩家手牌
        /// </summary>
        /// <param name="userInfo"></param>
        private void SetUserCardPokerInfo(HistoryUserInfo userInfo)
        {
            int[] pokers = userInfo.Pokers;
            for (int i = 0; i < pokers.Length; i++)
            {
                _pokers[i].SetCardId(pokers[i]);
                _pokers[i].SetCardFront();
                _pokers[i].SetCardDepth(i * 2);
            }
        }

        /// <summary>
        /// 设置每道信息
        /// </summary>
        /// <param name="userInfo"></param>
        void SetDunInfo(HistoryUserInfo userInfo)
        {
            SetDunScore(userInfo);
            SetDunType(userInfo);
            SetShootInfo(userInfo);
            _dunInfoParent.SetActive(true);
            _specialSprite.gameObject.SetActive(false);
        }

        private void SetShootInfo(HistoryUserInfo userInfo)
        {
            int shootTime = userInfo.ShootInfo.ShootCount;
            _shootTime.text = shootTime.ToString();
            _getShootTime.text = userInfo.GetShootTime.ToString();
            _swatMark.SetActive(shootTime > 2);
        }

        /// <summary>
        /// 设置每道得分
        /// </summary>
        /// <param name="userInfo"></param>
        void SetDunScore(HistoryUserInfo userInfo)
        {
            for (int i = 0; i < _normalLabels.Length; i++)
            {
                SetLineScoreLabel(_normalLabels[i], userInfo.NormalScore[i]);
                SetLineScoreLabel(_addLabels[i], userInfo.AddScore[i], true);
                _addLabels[i].transform.parent.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 特殊牌型
        /// </summary>
        /// <param name="userInfo"></param>
        void SetSpecialSprite(HistoryUserInfo userInfo)
        {
            _specialSprite.spriteName = ((CardType)userInfo.Sprcial).ToString();
            _specialSprite.MakePixelPerfect();
            _specialSprite.gameObject.SetActive(true);
            _dunInfoParent.SetActive(false);
        }

        /// <summary>
        /// 设置每道牌型
        /// </summary>
        /// <param name="userInfo"></param>
        void SetDunType(HistoryUserInfo userInfo)
        {
            int[] dunTypes = userInfo.DunType;
            if (_dunTypeSprites == null || dunTypes.Length > _dunTypeSprites.Length)
                return;
            for (int i = 0; i < dunTypes.Length; i++)
            {
                var spr = _dunTypeSprites[i];
                spr.spriteName = GetSpriteName(dunTypes[i], i);
                spr.MakePixelPerfect();
            }
        }

        /// <summary>
        /// 获得牌型的图片名称
        /// </summary>
        /// <param name="dunType">牌型</param>
        /// <param name="line">第几道</param>
        /// <returns></returns>
        private string GetSpriteName(int dunType, int line)
        {
            CardType type = (CardType)dunType;
            string spriteName;
            if (type == CardType.santiao && line == 0)
            {
                spriteName = "chongsan";
            }
            else if (type == CardType.hulu && line == 1)
            {
                spriteName = "zhongdunhulu";
            }
            else
            {
                spriteName = type.ToString();
            }

            return spriteName;
        }

        /// <summary>
        /// 设置加分字样
        /// </summary>
        /// <param name="label"></param>
        /// <param name="score">分数</param>
        void SetScoreLabel(UILabel label, int score)
        {
            if (score < 0)
            {
                label.gradientTop = Tools.ChangeToColor(0x6FFBF1);
                label.gradientBottom = Tools.ChangeToColor(0x0090ff);
                label.effectColor = Tools.ChangeToColor(0x002ea3);
            }
            else
            {
                label.gradientTop = Tools.ChangeToColor(0xffff00);
                label.gradientBottom = Tools.ChangeToColor(0xff9600);
                label.effectColor = Tools.ChangeToColor(0x831717);
            }
        }

        void SetLineScoreLabel(UILabel label, int score, bool isAdd = false)
        {
            SetScoreLabel(label, score);
            string temp = score < 0 ? score.ToString() : "+" + score;
            label.text = isAdd ? string.Format("({0})", temp) : temp;   //额外加分带括号
        }

        void SetTotalScoreLabel(UILabel label, int score)
        {
            SetScoreLabel(label, score);
            label.text = YxUtiles.ReduceNumber(score);
        }

    }
}