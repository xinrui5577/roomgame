
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class BbmjActionTing : ActionTing
    {
        public void YoujinTingAction(ISFSObject data)
        {
            SetData(data);
            MahjongUtility.PlayOperateEffect(mCurrChair, PoolObjectType.youjin);
            Game.MahjongGroups.Do((groups) =>
            {
                int currChair = DataCenter.CurrOpChair;
                groups.MahjongHandWall[currChair].ThrowOut(mThrowoutCard);
                var putCard = groups.MahjongThrow[currChair].GetInMahjong(mThrowoutCard);
                Game.TableManager.ShowOutcardFlag(putCard);
                groups.MahjongHandWall[currChair].SetHandCardState(HandcardStateTyps.Ting);
            });
        }
    }
}
