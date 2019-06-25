using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.components;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View
{
    public class RankItemView : YxView
    {
        /// <summary>
        /// 排行榜显示图标类型
        /// </summary>
        public static string RankType;
        /// <summary>
        /// 特殊显示数量
        /// </summary>
        public static int FirstFew;
        /// <summary>
        /// 对应种类排行提示
        /// </summary>
        public static string ItemNotice;
        /// <summary>
        /// 排行榜显示人数
        /// </summary>
        public static int TotalCount;
        [Tooltip("名次背景")]
        public UISprite Medal;
        [Tooltip("头像")]
        public UITexture Protrail;
        [Tooltip("名次")]
        public UILabel RankNum;
        [Tooltip("昵称")]
        public UILabel Nick;
        [Tooltip("排行数据")]
        public UILabel Value;
        [Tooltip("排行榜数据（倍率版）")]
        public NguiLabelAdapter ValueAdapter;
        [Tooltip("ID")]
        public UILabel ID;
        [Tooltip("排行数据类型图标")]
        public UISprite RankIcon;
        [Tooltip("指定格式文本描述")]
        public UILabel ItemNoticeLabel;
        [Tooltip("推广人文本描述")]
        public UILabel AffiliateNoticeLabel;
        [Tooltip("正常排行背景")]
        public string NormalRankBg = "";
        [Tooltip("特殊名次是否需要文本")]
        public bool SpecialRankNeedLabel;
        [Tooltip("特殊名次显示文本内容")]
        public string SpecialRankNotice= "未入榜";
        [Tooltip("未入排行背景名")]
        public string NoRankBg = "";
        [Tooltip("排行榜对应值显示格式")]
        public string FormatCoin = "n0";
        [Tooltip("排行格式")]
        public string FormatRank = "{0}";
        [Tooltip("特殊排行背景")]
        public string SpecialRankBg = "";
        [Tooltip("绑定邀请码显示格式")]
        public string AffiliateFormat= "已绑定邀请码:";
        [Tooltip("未绑定邀请码显示内容")]
        public string NoAffiliateContent = "未绑定邀请码";

        protected override void OnFreshView()
        {
            if (Data == null) return;
            var rankData = Data as RankItemData;
            if (rankData == null) return;
            if (Medal != null)
            {
                var isSpecialRank = rankData.RankNum <= FirstFew && rankData.RankNum != 0;
                if (isSpecialRank)
                {
                    if (!string.IsNullOrEmpty(SpecialRankBg))
                    {
                        Medal.spriteName = string.Format("{0}{1}", SpecialRankBg, rankData.RankNum);
                    }
                    Medal.gameObject.SetActive(true);
                    Medal.MakePixelPerfect();
                    if (SpecialRankNeedLabel)
                    {
                        RankNum.gameObject.SetActive(true);
                        RankNum.text = rankData.RankNum <= TotalCount && rankData.RankNum != 0 ? string.Format(FormatRank, rankData.RankNum) : SpecialRankNotice;
                    }
                    else
                    {
                        RankNum.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(NormalRankBg)) Medal.spriteName = NormalRankBg;
                    Medal.gameObject.SetActive(false);
                    Medal.MakePixelPerfect();
                    RankNum.gameObject.SetActive(true);
                    RankNum.text = rankData.RankNum <= TotalCount && rankData.RankNum != 0 ? string.Format(FormatRank, rankData.RankNum) : SpecialRankNotice;
                }
            }
            else
            {
                RankNum.gameObject.SetActive(true);
                RankNum.text = rankData.RankNum <= TotalCount && rankData.RankNum != 0 ? string.Format(FormatRank, rankData.RankNum) : SpecialRankNotice;
            }
            Nick.text = rankData.Nick;
            YxTools.TrySetComponentValue(Value, rankData.Value.ToString(FormatCoin));
            YxTools.TrySetComponentValue(ValueAdapter, rankData.Value,RankType);
            if (ID)
            {
                ID.text = rankData.ID;
            }
            if (RankIcon)
            {
                RankIcon.spriteName = RankType;
            }
            
            if (Protrail != null)
            {
                PortraitRes.SetPortrait(rankData.Avator, Protrail, rankData.Sex);
            }
            if (ItemNoticeLabel)
            {
                if (!string.IsNullOrEmpty(ItemNotice))
                {
                    ItemNoticeLabel.text = ItemNotice;
                }
            }
            if (AffiliateNoticeLabel)
            {
                if(rankData.Affiliate==0)
                {
                    AffiliateNoticeLabel.text = NoAffiliateContent;
                }
                else
                {
                    AffiliateNoticeLabel.text = string.Format("{0}{1}", AffiliateFormat, rankData.Affiliate);
                }
            }
        }
    }
    /// <summary>
    /// 排行榜单条数据
    /// </summary>
    public class RankItemData
    {
        #region Keys
        /// <summary>
        /// Key头像
        /// </summary>
        private string _keyAvator = "avatar_x";
        /// <summary>
        /// Key昵称
        /// </summary>
        private string _keyUserName = "nick_m";
        /// <summary>
        /// Key玩家ID
        /// </summary>
        private string _keyUserId = "user_id";
        /// <summary>
        /// Key排行数据
        /// </summary>
        private string _keyValue = "value";
        /// <summary>
        /// Key性别
        /// </summary>
        private string _keySex = "sex_i";
        /// <summary>
        /// Key推荐人ID
        /// </summary>
        private string _keyAffiliate = "affiliate";
        /// <summary>
        /// Key自己的排行名次（RankRequest）
        /// </summary>
        private string _keyRankNum = "rankNum";
        /// <summary>
        /// Key 自己的排行名次（TopRank）
        /// </summary>
        private string _keyTop = "top";
        #endregion

        /// <summary>
        /// 玩家昵称
        /// </summary>
        private string _userName;

        /// <summary>
        /// 玩家ID
        /// </summary>
        private string _userId;

        /// <summary>
        /// 头像信息
        /// </summary>
        private string _avator;

        /// <summary>
        /// 排行值
        /// </summary>
        private long _value;

        /// <summary>
        /// 性别
        /// </summary>
        private int _sex;

        /// <summary>
        /// 绑定人ID
        /// </summary>
        private int _affiliateId;

        /// <summary>
        /// 排行名次
        /// </summary>
        private int _rankNum;
        /// <summary>
        /// 排行榜中显示记录数量
        /// </summary>
        private int _count;
        /// <summary>
        /// 特殊名次
        /// </summary>
        public bool IsSpecialRank;

        public int RankNum
        {
            get { return _rankNum; }
        }

        public int Sex
        {
            get { return _sex; }
        }

        public string Nick
        {
            get { return _userName; }
        }

        public string ID
        {
            get { return _userId; }
        }

        public long Value
        {
            get { return _value; }
        }

        public int Affiliate
        {
            get { return _affiliateId; }
        }

        public string Avator
        {
            get { return _avator; }
        }

        public int Count
        {
            get { return _count; }
        }

        public RankItemData(Dictionary<string, object> param, int index, int count = 0)
        {
            if (param.ContainsKey(_keyUserName))
            {
                _userName = param[_keyUserName].ToString();
            }
            if (param.ContainsKey(_keyUserId))
            {
                _userId = param[_keyUserId].ToString();
            }
            if (param.ContainsKey(_keyValue))
            {
                _value = long.Parse(param[_keyValue].ToString());
            }
            if (param.ContainsKey(_keySex))
            {
                _sex = int.Parse(param[_keySex].ToString());
            }
            if (param.ContainsKey(_keyAffiliate))
            {
                _affiliateId = int.Parse(param[_keyAffiliate].ToString());
            }
            if (param.ContainsKey(_keyAvator))
            {
                _avator = param[_keyAvator].ToString();
            }
            else
            {
                _avator = "HS_9";
            }
            if (param.ContainsKey(_keyRankNum))
            {
                _rankNum = int.Parse(param[_keyRankNum].ToString());
            }
            else
            {
                _rankNum = index;
            }
            if (param.ContainsKey(_keyTop))//topRank
            {
                _rankNum = int.Parse(param[_keyTop].ToString());
            }
            _count = count;
        }

    }
}