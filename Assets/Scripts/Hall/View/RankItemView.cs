using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;
using YxFramwork.Common.Model;
using YxFramwork.Framework;
using YxFramwork.Manager;

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
        public YxBaseTextureAdapter Protrail;
        [Tooltip("名次")]
        public UILabel RankNum;
        [Tooltip("昵称")]
        public UILabel Nick;
        [Tooltip("排行数据")]
        public NguiLabelAdapter Value;
        [Tooltip("金币显示类型")]
        public YxBaseLabelAdapter.YxELabelType CoinLabelType = YxBaseLabelAdapter.YxELabelType.NumberWithUnit;
        [Tooltip("ID")]
        public UILabel ID;
        [Tooltip("排行数据类型图标")]
        public UISprite RankIcon;
        [Tooltip("指定格式文本描述")]
        public UILabel ItemNoticeLabel;
        [Tooltip("推广人文本描述")]
        public UILabel AffiliateNoticeLabel;
        [Tooltip("茶馆ID")]
        public UILabel TeaIdLabel;
        [Tooltip("茶馆名字")]
        public UILabel TeaNameLabel;
        [Tooltip("茶馆签名")]
        public UILabel TeaSignLabel;
        [Tooltip("正常排行背景")]
        public string NormalRankBg = "";
        [Tooltip("特殊名次是否需要文本")]
        public bool SpecialRankNeedLabel;
        [Tooltip("特殊名次显示文本内容")]
        public string SpecialRankNotice = "未入榜";
        [Tooltip("未入排行背景名")]
        public string NoRankBg = "";
        [Tooltip("排行榜对应值显示格式")]
        public string FormatCoin = "n0";
        [Tooltip("排行格式")]
        public string FormatRank = "{0}";
        [Tooltip("特殊排行背景")]
        public string SpecialRankBg = "";
        [Tooltip("绑定邀请码显示格式")]
        public string AffiliateFormat = "已绑定邀请码:";
        [Tooltip("未绑定邀请码显示内容")]
        public string NoAffiliateContent = "未绑定邀请码";
        private RankItemData _data;

        public void OnUserInfoClick()
        {
            var rankData = Data as RankItemData;
            if(rankData==null)return;
           var win =YxWindowManager.OpenWindow("RankUserInfoWindow");
            var userInfo = new UserInfo
                {
                    NickM = rankData.Nick,
                    CoinA =(int)rankData.Value,
                    AvatarX = rankData.Avator,
                    UserId = rankData.ID,
                    Signature = rankData.Signature,
                };
            win.UpdateView(userInfo);
        }
        protected override void OnFreshView()
        {
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
                    if (RankNum)
                    {
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
                }
                else
                {
                    if (!string.IsNullOrEmpty(NormalRankBg)) Medal.spriteName = NormalRankBg;
                    Medal.gameObject.SetActive(false);
                    Medal.MakePixelPerfect();
                    if (RankNum)
                    {
                        RankNum.gameObject.SetActive(true);
                        RankNum.text = rankData.RankNum <= TotalCount && rankData.RankNum != 0 ? string.Format(FormatRank, rankData.RankNum) : SpecialRankNotice;
                    }
                }
            }
            else
            {
                if (RankNum)
                {
                    RankNum.gameObject.SetActive(true);
                    RankNum.text = rankData.RankNum <= TotalCount && rankData.RankNum != 0 ? string.Format(FormatRank, rankData.RankNum) : SpecialRankNotice;
                }
            }
            Nick.TrySetComponentValue(rankData.Nick);
            Value.TrySetComponentValue(rankData.Value, RankType,"{0}", CoinLabelType);
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
                PortraitDb.SetPortrait(rankData.Avator, Protrail, rankData.Sex);
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
                if (rankData.Affiliate == 0)
                {
                    AffiliateNoticeLabel.text = NoAffiliateContent;
                }
                else
                {
                    AffiliateNoticeLabel.text = string.Format("{0}{1}", AffiliateFormat, rankData.Affiliate);
                }
            }

            if (TeaIdLabel)
            {
                TeaIdLabel.text = rankData.TeaId.ToString();
            }

            if (TeaNameLabel)
            {
                TeaNameLabel.text = rankData.TeaName;
            }

            if (TeaSignLabel)
            {
                TeaSignLabel.text = rankData.TeaSign;
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
        private const string KeyAvator = "avatar_x";
        /// <summary>
        /// Key昵称
        /// </summary>
        private const string KeyUserName = "nick_m";
        /// <summary>
        /// Key玩家ID
        /// </summary>
        private const string KeyUserId = "user_id";
        /// <summary>
        /// Key排行数据
        /// </summary>
        private const string KeyValue = "value";
        /// <summary>
        /// Key性别
        /// </summary>
        private const string KeySex = "sex_i";
        /// <summary>
        /// Key推荐人ID
        /// </summary>
        private const string KeyAffiliate = "affiliate";
        /// <summary>
        /// Key自己的排行名次（RankRequest）
        /// </summary>
        private const string KeyRankNum = "rankNum";
        /// <summary>
        /// Key 自己的排行名次（TopRank）
        /// </summary>
        private const string KeyTop = "top";
        /// <summary>
        /// Key自己的个性签名（RankRequest）
        /// </summary>
        private string _keySignature = "signature_x";
        /// <summary>
        /// Key茶馆ID（RankRequest）
        /// </summary>
        private string _keyTeaId = "tea_id";
        /// <summary>
        /// Key茶馆名字（RankRequest）
        /// </summary>
        private string _keyTeaName = "tea_name";
        /// <summary>
        /// Key茶馆描述（RankRequest）
        /// </summary>
        private string _keyTeaSign = "group_sign";
        #endregion

        /// <summary>
        /// 特殊名次
        /// </summary>
        public bool IsSpecialRank;
        /// <summary>
        /// 排行名次
        /// </summary>
        public int RankNum { get; private set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; private set; }
        /// <summary>
        /// 玩家昵称
        /// </summary>
        public string Nick { get; private set; }
        /// <summary>
        /// 玩家ID
        /// </summary>
        public string ID { get; private set; }
        /// <summary>
        /// 排行值
        /// </summary>
        public long Value { get; private set; }
        /// <summary>
        /// 绑定人ID
        /// </summary>
        public int Affiliate { get; private set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string Signature { get; private set; }

        /// <summary>
        /// 头像信息
        /// </summary>
        public string Avator { get; private set; }
        /// <summary>
        /// 排行榜中显示记录数量
        /// </summary>
        public int Count { get; private set; }
        /// <summary>
        /// 茶馆id
        /// </summary>
        public int TeaId { get; private set; }
        /// <summary>
        /// 茶馆名字
        /// </summary>
        public string TeaName { get; private set; }
        /// <summary>
        /// 茶馆签名
        /// </summary>
        public string TeaSign { get; private set; }

        public RankItemData(IDictionary<string, object> param, int index, int count = 0)
        {
            if (param.ContainsKey(KeyUserName))
            {
                Nick = param[KeyUserName].ToString();
            }
            if (param.ContainsKey(KeyUserId))
            {
                ID = param[KeyUserId].ToString();
            }
            if (param.ContainsKey(KeyValue))
            {
                Value = long.Parse(param[KeyValue].ToString());
            }
            if (param.ContainsKey(KeySex))
            {
                Sex = int.Parse(param[KeySex].ToString());
            }
            if (param.ContainsKey(KeyAffiliate))
            {
                Affiliate = int.Parse(param[KeyAffiliate].ToString());
            }
            if (param.ContainsKey(KeyAvator))
            {
                Avator = param[KeyAvator].ToString();
            } 
            RankNum = param.ContainsKey(KeyRankNum) ? int.Parse(param[KeyRankNum].ToString()) : index;

            if (param.ContainsKey(KeyTop))//topRank
            {
                RankNum = int.Parse(param[KeyTop].ToString());
            }
            if (param.ContainsKey(_keySignature))
            {
                if (param[_keySignature] != null) Signature = param[_keySignature].ToString();
            }

            if (param.ContainsKey(_keyTeaId))
            {
                if (param[_keyTeaId] != null) TeaId =int.Parse(param[_keyTeaId].ToString());
            }
            if (param.ContainsKey(_keyTeaName))
            {
                if (param[_keyTeaName] != null) TeaName = param[_keyTeaName].ToString();
            }
            if (param.ContainsKey(_keyTeaSign))
            {
                if (param[_keyTeaSign] != null&& param[_keyTeaSign]!=null) TeaSign = param[_keyTeaSign].ToString();
            }

            Count = count;
        }

    }
}