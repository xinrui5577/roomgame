using YxFramwork.Framework;

namespace Assets.Scripts.Game.fruit
{
    public class FruitPlayer : YxBaseGamePlayer
    {
        protected override void SetCoin(long coin)
        {
            if (CoinLabel == null) return;
            base.SetCoin(coin);
            var tempStr = "￥" + (coin < 9999999999 ? CoinLabel.Content.PadLeft(10, '0') : "9999999999").ToString();
            CoinLabel.Text(tempStr);
        }

        protected override void SetWinCoin(long winCoin)
        {
            if (WinCoinLabel == null) return;
            base.SetWinCoin(winCoin);
            string tempStr = "￥" + (winCoin < 9999999999 ? WinCoinLabel.Content.PadLeft(10, '0') : "9999999999").ToString();
            WinCoinLabel.Text(tempStr);

            //获取比倍彩金
            DisPlayDxResult.getInstance().BiBeiCaiJin = winCoin;
        }
    }
}
