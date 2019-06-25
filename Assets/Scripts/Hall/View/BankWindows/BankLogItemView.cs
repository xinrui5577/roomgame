using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.BankWindows
{
    public class BankLogItemView : YxView
    {
        public UILabel LogLabel;
        public UILabel CoinLabel;
        public UILabel DateLabel;
        [Tooltip("倍率版金币")]
        public NguiLabelAdapter CoinLabelAdapter;
        /// <summary>
        /// key赠送类型
        /// </summary>
        private const string _keyType = "type_i";
        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = Data as Dictionary<string,object>;
            if (data == null) return;
            int type;
            YxTools.TryGetValueWitheKey(data, out type, _keyType);
            long coin;
            YxTools.TryGetValueWitheKey(data,out coin, "coin_num_a");
            var tempFormat = type>0? "玩家【{0}】赠送给您":"您赠送给玩家【{0}】";
            LogLabel.text = string.Format(tempFormat,data["nick_m"]);
            DateLabel.text = data["create_dt"].ToString();
            YxTools.TrySetComponentValue(CoinLabel, string.Format("x{0}金币", coin));
            YxTools.TrySetComponentValue(CoinLabelAdapter, coin, "1", "x{0}金币");
        } 
    } 
}
