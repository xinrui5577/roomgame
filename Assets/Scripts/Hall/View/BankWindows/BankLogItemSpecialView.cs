using System.Collections.Generic;

namespace Assets.Scripts.Hall.View.BankWindows
{
    public class BankLogItemSpecialView : BankLogItemView
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public UILabel SerialNumber;
        /// <summary>
        /// 昵称
        /// </summary>
        public UILabel NickName;
        /// <summary>
        /// 玩家的游戏ID
        /// </summary>
        public UILabel UserId;
        /// <summary>
        /// 金币的变化数量
        /// </summary>
        public UILabel CoinChange;
        /// <summary>
        /// 创建的时间
        /// </summary>
        public UILabel CreatTime;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = Data as Dictionary<string, object>;
            if (data == null) return;
            var nickName = data.ContainsKey("nick_m") ? data["nick_m"].ToString() : "";
            var id = data.ContainsKey("id") ? data["id"].ToString() : "";
            var otherId = data.ContainsKey("user_id") ? data["user_id"].ToString() : "";
            var coinNum = data.ContainsKey("coin_num_a") ? data["coin_num_a"].ToString() : "";
            var createDt=data.ContainsKey("create_dt")?data["create_dt"].ToString(): "";

            SerialNumber.text = id;
            NickName.text = nickName;
            UserId.text = otherId;
            CoinChange.text = coinNum;
            CreatTime.text = createDt;
        }
    }
}
