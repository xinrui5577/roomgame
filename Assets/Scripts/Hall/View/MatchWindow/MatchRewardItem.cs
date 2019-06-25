/** 
 *文件名称:     MatchRewardItem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-01-22 
 *描述:         比赛奖励信息
 *历史记录: 
*/

using System.Collections.Generic;
using Assets.Scripts.Hall.View.PageListWindow;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.MatchWindow
{
    public class MatchRewardItem : YxView
    {
        #region UI Param
        [Tooltip("特殊排名显示图标")]
        public UISprite RankNumSprite;
        [Tooltip("排行名次")]
        public UILabel RankNumLabel;
        [Tooltip("排行标题")]
        public UILabel NameLabel;
        [Tooltip("奖励信息")]
        public UILabel RewardInfoLabel;
        #endregion
        #region Data Param
        [Tooltip("特殊显示的排行名次（显示特殊图片），默认前三名")]
        public int SpecialNum = 3;
        [Tooltip("排行名次图片格式")]
        public string RankSpriteFormat = "n{0}";
        #endregion
        #region Local Data
        #endregion
        #region Life Cycle

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data!=null)
            {
                FreshView();
            }
        }

        #endregion
        #region Function

        private void FreshView()
        {
            if (Data is MatchRewardData)
            {
                MatchRewardData rewardData = Data as MatchRewardData;
                if (NameLabel)
                {
                    NameLabel.text = rewardData.Title;
                }
                if (RewardInfoLabel)
                {
                    RewardInfoLabel.text = rewardData.Reward;
                }
                if (rewardData.IsSingleNum)
                {
                    if (RankNumSprite)
                    {
                        var startNum = rewardData.RankNum;
                        if (startNum <= SpecialNum)
                        {
                            RankNumSprite.gameObject.SetActive(true);
                            RankNumLabel.gameObject.SetActive(false);
                            RankNumSprite.spriteName = string.Format(RankSpriteFormat, startNum);
                        }
                        else
                        {
                            RankNumSprite.gameObject.SetActive(false);
                            RankNumLabel.gameObject.SetActive(true);
                        }
                    }
                }
                else
                {
                    if (RankNumSprite)
                    {
                        RankNumSprite.gameObject.SetActive(false);
                    }
                }
                if (RankNumLabel)
                {
                    RankNumLabel.text = rewardData.RankInfo;
                }
            }
        }

        #endregion

    }

    public class MatchRewardData:YxData
    {
        /// <summary>
        /// key 显示标题
        /// </summary>
        private const string KeyTitle= "title";
        /// <summary>
        /// key 比赛奖励
        /// </summary>
        private const string KeyRewards = "reward_x";
        /// <summary>
        /// key比赛起始名次
        /// </summary>
        private const string KeyRankStartNum= "range_frome_a";
        /// <summary>
        /// key比赛结束名次
        /// </summary>
        private const string KeyRankEndNum= "range_to_a";
        /// <summary>
        /// 范围排行格式
        /// </summary>
        private string _rangeFormat = "{0}~{1}";
        /// <summary>
        /// 标题（名称）
        /// </summary>
        private string _title;
        /// <summary>
        /// 奖励信息
        /// </summary>
        private string _rewards;
        /// <summary>
        /// 排行起始名次(起始与结束不同时表示为范围，只有起始字段时，标识为单独名次)
        /// </summary>
        private int _rankStartNum;
        /// <summary>
        /// 排行结束名次（非范围时，无此字段）
        /// </summary>
        private int _rankEndNum;

        public string Title
        {
            get { return _title; }
        }

        public string Reward
        {
            get
            {
                return _rewards;
            }
        }

        public int RankNum
        {
            get { return _rankStartNum; }
        }

        public string RankInfo
        {
            get
            {
                if (_rankEndNum==0)
                {
                    return _rankStartNum.ToString();
                }
                return string.Format(_rangeFormat, _rankStartNum, _rankEndNum);
            }
        }

        public bool IsSingleNum
        {
            get { return _rankEndNum == 0; }
        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            if (dic.ContainsKey(KeyTitle))
            {
                _title = dic[KeyTitle].ToString();
            }
            if (dic.ContainsKey(KeyRewards))
            {
                _rewards = dic[KeyRewards].ToString();
            }
            if (dic.ContainsKey(KeyRankStartNum))
            {
                _rankStartNum = int.Parse(dic[KeyRankStartNum].ToString());
            }
            if (dic.ContainsKey(KeyRankEndNum))
            {
                _rankEndNum = int.Parse(dic[KeyRankEndNum].ToString());
            }
        }

        public MatchRewardData(object data) : base(data)
        { 
        }
    }
}
