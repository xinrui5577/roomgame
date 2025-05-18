namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionCommonReplay : AbsCommandAction
    {
        public void ReplaySendCard(ReplayFrameData data)
        {
            Game.MahjongGroups.MahjongHandWall[data.OpChair].GetInMahjong(data.Cards);
        }

        public void ReplayThrowoutCard(ReplayFrameData data)
        {
            var card = data.Cards[0];
            var seat = data.OpChair;
            Game.MahjongGroups.MahjongHandWall[seat].ThrowOut(card);

            var item = Game.MahjongGroups.MahjongThrow[seat].GetInMahjong(card);
            Game.TableManager.ShowOutcardFlag(item);

            GameCenter.Controller.ReplayPlaySound(card);
        }

        public void ReplayGetCard(ReplayFrameData data)
        {
            Game.MahjongGroups.MahjongHandWall[data.OpChair].GetInMahjong(data.Cards[0]);
            MahjongUtility.PlayEnvironmentSound("zhuapai");
        }
    }
}
