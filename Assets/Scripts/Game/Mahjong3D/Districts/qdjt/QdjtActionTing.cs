using Sfs2X.Entities.Data;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class QdjtActionTing : ActionTing
    {
        public override void SpecialTingAction(ISFSObject data)
        {
            SetData(data);
            var currChair = DataCenter.CurrOpChair;
            var cards = data.TryGetIntArray(RequestKey.KeyCards);
            var panel = GameCenter.Hud.GetPanel<PanelOtherHuTip>();
            panel.Open(cards, currChair);
            LogicActionLiangdao(cards);

            DataCenter.Players[currChair].IsAuto = true;
            MahjongUtility.PlayOperateEffect(currChair, PoolObjectType.liangdao);
            GameCenter.EventHandle.Dispatch((int)EventKeys.OnTing, new OnTingArgs() { TingChair = currChair });
        }

        /// <summary>
        /// 亮倒
        /// </summary>
        private void LogicActionLiangdao(int[] liangCards)
        {
            if (liangCards == null || liangCards.Length < 1) return;
            Game.MahjongGroups.MahjongHandWall[DataCenter.CurrOpChair]
                .SetHandCardState(HandcardStateTyps.TingAndShowCard, liangCards);
        }
    }
}
