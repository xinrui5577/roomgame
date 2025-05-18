using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class DlmjActionTing : ActionTing
    {
        protected int[] NiuTings;

        private void SetDaLianData(ISFSObject data)
        {
            SetData(data);
            var currChair = DataCenter.CurrOpChair;
            DataCenter.Players[currChair].IsAuto = true;
            MahjongUtility.PlayOperateEffect(currChair, PoolObjectType.ting);
            GameCenter.EventHandle.Dispatch((int)EventKeys.OnTing, new OnTingArgs() { TingChair = currChair });
            NiuTings = data.ContainsKey("niu") ? data.GetIntArray("niu") : null;
        }

        public override void TingAction(ISFSObject data)
        {
            SetDaLianData(data);
            var groups = Game.MahjongGroups;
            var currChair = DataCenter.CurrOpChair;
            //打出一张手牌
            groups.MahjongHandWall[currChair].ThrowOut(mThrowoutCard);
            //打出的牌显示再桌面
            var putCard = groups.MahjongThrow[currChair].GetInMahjong(mThrowoutCard);
            Game.TableManager.ShowOutcardFlag(putCard);
            if (NiuTings == null || NiuTings.Length < 1)
            {
                //切换听牌状态
                groups.MahjongHandWall[currChair].SetHandCardState(HandcardStateTyps.Ting);
            }
            else
            {
                //切换牛听牌状态
                groups.MahjongHandWall[currChair].SetHandCardState(HandcardStateTyps.TingAndShowCard, NiuTings);
            }
        }
    }
}