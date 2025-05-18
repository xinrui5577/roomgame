using Assets.Scripts.Common.Utils;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.mx97
{
    public class PlayerInfo : YxBaseGamePlayer
    {

        // Use this for initialization
        protected override void SetCoin(long coin)
        {
            if (CoinLabel == null) return;
            base.SetCoin(coin);
            string tempStr = "￥" + YxUtiles.GetShowNumberForm(coin);
            CoinLabel.Text(tempStr);
        }
    }
}