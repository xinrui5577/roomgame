namespace Assets.Scripts.Game.mdx
{
    public class MdxBanker : MdxPlayer
    { 
        protected override void SetNick(string nike)
        {
            base.SetNick(Info.Seat < 0 ? "系统庄" : nike);
        }

        protected override void SetWinTotalCoin(long winTotalCoin)
        {
            if (WinTotalCoinLabel == null) return;
            WinTotalCoinLabel.Text(Info.Seat < 0 ? "--": winTotalCoin.ToString());
        }

        protected override void SetTotalCount(int totalCount)
        {
            if (TotalCountLabel == null) return;
            TotalCountLabel.Text(Info.Seat < 0 ? "--" : totalCount.ToString());
        }
    }
} 