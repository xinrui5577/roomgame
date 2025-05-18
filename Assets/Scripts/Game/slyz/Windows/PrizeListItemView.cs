using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.slyz.Windows
{
    /// <summary>
    /// 中奖名单Item
    /// </summary>
    public class PrizeListItemView : YxView
    {
        /// <summary>
        /// 时间
        /// </summary>
        public UILabel TimeLabel;
        /// <summary>
        /// 昵称
        /// </summary>
        public UILabel NickLabel;
        /// <summary>
        /// 奖的类型
        /// </summary>
        public UILabel TypeLabel;
        /// <summary>
        /// 获得的金币
        /// </summary>
        public UILabel MoneyLabel;

        protected override void OnFreshView()
        {
            var data = GetData<StructPrize>();
            if (data == null) { return;}
            TimeLabel.text = data.PrizeTime;
            NickLabel.text = data.UserName;
            TypeLabel.text = data.TypeName;
            long jackpot;
            long.TryParse(data.JackpotNum, out jackpot);
            MoneyLabel.text = YxUtiles.GetShowNumberForm(jackpot);
        }
    }
}
