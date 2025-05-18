using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class WmbbmjActionCpg : ActionCpg
    {
        public override void ResponseCpgAction(ISFSObject data)
        {
            base.ResponseCpgAction(data);

            //财飘暗杠
            if (mCpgData.Type == EnGroupType.AnGang
               || mCpgData.Type == EnGroupType.MingGang
               || mCpgData.Type == EnGroupType.ZhuaGang)
            {
                var throwoutCard = GameCenter.GameLogic.GetGameResponseLogic<CommandThrowoutCard>();
                var wmbbmjLogic = throwoutCard.LogicAction as WmbbmjActionThrowoutCard;
                var flag = wmbbmjLogic.CheckCaipiaoState();
                if (flag)
                {
                    var playerHand = Game.MahjongGroups.MahjongHandWall[0].GetComponent<MahjongPlayerHand>();
                    playerHand.SwitchFreezeState();
                }
            }
        }
    }
}
