using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class NamjActionTing : ActionTing
    {
        public override void SpecialTingAction(ISFSObject data)
        {
            SetData(data);

            // 7对特殊听牌：自己有5对时，别人打牌的时候，正好组成第6对时允许听牌
            // 把别人的手牌拿到自己手中，自己打一张牌    
            var groups = Game.MahjongGroups;
            var currChair = DataCenter.CurrOpChair;
            //播放特效
            MahjongUtility.PlayOperateEffect(currChair, PoolObjectType.ting);
            //打出一张手牌
            groups.MahjongHandWall[currChair].ThrowOut(mThrowoutCard);
            //打出的牌显示再桌面
            var putCard = groups.MahjongThrow[currChair].GetInMahjong(mThrowoutCard);
            Game.TableManager.ShowOutcardFlag(putCard);
            //把别人打的牌拿到手牌中               
            var otherCard = groups.MahjongThrow[DataCenter.OldOpChair].PopMahjong();
            var cardValue = otherCard.Value;
            groups.MahjongHandWall[currChair].GetInMahjongNoAni(cardValue);
            //切换听牌状态
            groups.MahjongHandWall[currChair].SetHandCardState(HandcardStateTyps.TingAndShowCard, new int[] { cardValue, cardValue });
        }
    }
}