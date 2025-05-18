using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.lyzz2d.Game.GameCtrl;
using Assets.Scripts.Game.lyzz2d.Utils;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.lyzz2d.Game.Component
{
    /// <summary>
    ///     单个玩家的显示结果
    /// </summary>
    public class PlayerResultInfo : MonoBehaviour
    {
        /// <summary>
        ///     番名称
        /// </summary>
        private string _fanName;

        /// <summary>
        ///     杠的分数
        /// </summary>
        private int _gangNum;

        /// <summary>
        ///     胡牌分数
        /// </summary>
        private int _huNum;

        /// <summary>
        ///     胡牌方式
        /// </summary>
        private string _huType;

        /// <summary>
        ///     是否是赢家
        /// </summary>
        private bool _isWiner;

        /// <summary>
        ///     是否自摸
        /// </summary>
        private bool _isZimo;

        /// <summary>
        ///     小结算的所有牌
        /// </summary>
        [SerializeField] private ResultCards _resultCards;

        /// <summary>
        ///     显示总分
        /// </summary>
        private int _totalNum;

        /// <summary>
        ///     玩家头像
        /// </summary>
        public UITexture HeadTexture;

        /// <summary>
        ///     胡牌logo
        /// </summary>
        public GameObject HuLogo;

        /// <summary>
        ///     番名称
        /// </summary>
        public UILabel LabelFanName;

        /// <summary>
        ///     杠分数
        /// </summary>
        public UILabel LabelGangNum;

        /// <summary>
        ///     胡牌分
        /// </summary>
        public UILabel LabelHuScore;

        /// <summary>
        ///     总分数
        /// </summary>
        public UILabel LabelTotalScore;

        /// <summary>
        ///     玩家名称
        /// </summary>
        public UILabel LabelUserName;

        [HideInInspector] public string ScoreInfo;

        /// <summary>
        ///     庄logo
        /// </summary>
        public GameObject ZhuangSprite;

        /// <summary>
        ///     是不是赢家
        /// </summary>
        public bool IsWiner
        {
            get { return _isWiner; }
        }

        public void SetResultInfo(ISFSObject data, MahjongPlayer player, List<int> handCards, List<int> huList)
        {
            #region data

            int type;
            long totalGold;
            ISFSArray Groups;
            GameTools.TryGetValueWitheKey(data, out type, RequestKey.KeyType);
            GameTools.TryGetValueWitheKey(data, out _huNum, RequestKey.KeyHuNum);
            GameTools.TryGetValueWitheKey(data, out _gangNum, RequestKey.KeyGangNum);
            GameTools.TryGetValueWitheKey(data, out _fanName, RequestKey.KeyHuName);
            GameTools.TryGetValueWitheKey(data, out _totalNum, RequestKey.KeyGold);
            GameTools.TryGetValueWitheKey(data, out totalGold, RequestKey.KeyTotalGold);
            GameTools.TryGetValueWitheKey(data, out Groups, RequestKey.KeyGroups);
            var groups = GameTools.GetGroupData(Groups);
            YxDebug.Log("手牌总长度是：" + handCards.Count);
            _isWiner = type > 0;
            if (type == 2) //目前自摸情况下，会把胡的那张牌从手牌中带回来，这里删掉。目前只有一个胡牌，所以这么删，待扩展
            {
                handCards.Remove(huList[0]);
            }
            YxDebug.Log("手牌实际长度是：" + handCards.Count);

            #endregion

            #region UI

            LabelUserName.text = player.UserInfo.name;
            ZhuangSprite.SetActive(player.IsZhuang);
            LabelFanName.text = _fanName;
            YxTools.TrySetComponentValue(LabelHuScore, YxUtiles.GetShowNumber(_huNum).ToString());
            YxTools.TrySetComponentValue(LabelGangNum, YxUtiles.GetShowNumber(_gangNum).ToString());
            YxTools.TrySetComponentValue(LabelTotalScore, YxUtiles.GetShowNumber(_totalNum).ToString());
            HeadTexture.mainTexture = player.CurrentInfoPanel.UserIcon.mainTexture;
            player.UserInfo.Gold = totalGold;
            player.CurrentInfoPanel.SetGold((int) totalGold);
            HuLogo.SetActive(IsWiner);

            #endregion

            _resultCards.Init(groups, handCards, huList, _isWiner);
            LabelFanName.ProcessText();
            NGUIText.Update();
        }

        public void ResetInfo()
        {
            _isWiner = false;
            _isZimo = false;
            _resultCards.Reset();
        }
    }
}