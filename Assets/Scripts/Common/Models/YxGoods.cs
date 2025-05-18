using System.Collections.Generic;
using System.Globalization;
using com.yxixia.utile.Utiles;
using YxFramwork.Tool;

namespace Assets.Scripts.Common.Models
{
    public class YxGoods
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string Type;
        /// <summary>
        /// id
        /// </summary>
        public string Id;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name = "";
        /// <summary>
        /// 消耗类型
        /// </summary>
        public string ConsumeId;
        /// <summary>
        /// 消耗个数
        /// </summary>
        public int ConsumeNum;

        private int _buyNum;
        /// <summary>
        /// 消耗个数
        /// </summary>
        public int BuyNum
        {
            get { return _buyNum < 1 ? 1 : _buyNum; }
            set { _buyNum = value; }
        }

        /// <summary>
        /// 图片url
        /// </summary>
        public string IconUrl = "";
        /// <summary>
        /// 获得的Id
        /// </summary>
        public string RewardId;
        /// <summary>
        /// 获得个数
        /// </summary>
        public int RewardNum;
        /// <summary>
        /// 描述
        /// </summary>
        public string Description = "";
        /// <summary>
        /// 购买操作
        /// </summary>
        public YxEBuyAction BuyAction;
        /// <summary>
        /// 是否是热卖
        /// </summary>
        public bool IsHot;
        /// <summary>
        /// 折扣价
        /// </summary>
        public int OldConsumeNum;
        /// <summary>
        /// 支付url
        /// </summary>
        public string PayUrl;

        public void Parse(Dictionary<string,object> data)
        {
            if (data == null) { return;}
            DictionaryHelper.Parse(data, "id", ref Id);
            DictionaryHelper.Parse(data, "item_m", ref Name);
            DictionaryHelper.Parse(data, "consume_id", ref ConsumeId);
            DictionaryHelper.Parse(data, "consume_num", ref ConsumeNum);
            DictionaryHelper.Parse(data, "icon_url_x", ref IconUrl);
            DictionaryHelper.Parse(data, "reward_id", ref RewardId);
            DictionaryHelper.Parse(data, "reward_num", ref RewardNum);
            DictionaryHelper.Parse(data, "desc_s", ref Description);
            DictionaryHelper.ParseEnum(data, "action", ref BuyAction);
            DictionaryHelper.Parse(data, "is_hot", ref IsHot);
            DictionaryHelper.Parse(data, "old_price", ref OldConsumeNum);
            DictionaryHelper.Parse(data, "open_url", ref PayUrl);
        }

        /// <summary>
        /// 物品显示价格
        /// </summary>
        /// <returns></returns>
        public string GetConsumeNumText()
        {
            return GetConsumeNumText(ConsumeId, ConsumeNum);
        }

        /// <summary>
        /// 物品显示原价格
        /// </summary>
        /// <returns></returns>
        public string GetOldConsumeNumText()
        {
            return GetConsumeNumText(ConsumeId, OldConsumeNum);
        }

        private static string GetConsumeNumText(string id,int consume)
        {
            if (consume <= 0) return "0";
            switch (id)
            {
                case "rmb"://大洋
                    return(consume / 100f).ToString("N");
                case "coin"://金币
                    return YxUtiles.GetShowNumberForm(consume);
            }
            return consume.ToString();
        }
    }

    public enum YxEBuyAction
    {
        /// <summary>
        /// 内付
        /// </summary>
        payment,
        /// <summary>
        /// 兑换
        /// </summary>
        trade,
        /// <summary>
        /// 外链
        /// </summary>
        url,
        /// <summary>
        /// 外付
        /// </summary>
        outpay
    }
}
