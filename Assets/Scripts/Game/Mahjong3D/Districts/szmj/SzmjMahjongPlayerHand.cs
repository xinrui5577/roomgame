namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class SzmjMahjongPlayerHand : MahjongPlayerHand
    {
        protected override void Start()
        {
            base.Start();
            //设置出牌过滤条件
            mPutOutFunc = (item) => { return item.MahjongCard.Value >= (int)MahjongValue.Zhong; };
        }
    }
}
