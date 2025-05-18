using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionTing : AbsCommandAction
    {
        public int mCurrChair;
        public int mThrowoutCard;

        public virtual void TingAction(ISFSObject data)
        {
            SetData(data);
            TingLogic(data);
            MahjongUtility.PlayOperateEffect(mCurrChair, PoolObjectType.ting);
        }

        public virtual void SpecialTingAction(ISFSObject data) { }

        protected virtual void SetData(ISFSObject data)
        {
            mThrowoutCard = data.GetInt(RequestKey.KeyOpCard);
            DataCenter.CurrOpSeat = data.GetInt(RequestKey.KeySeat);
            DataCenter.ThrowoutCard = mThrowoutCard;
            mCurrChair = DataCenter.CurrOpChair;
            GameCenter.Shortcuts.MahjongQuery.ShowQueryTip(null);
            DataCenter.Players[mCurrChair].IsAuto = true;
            GameCenter.EventHandle.Dispatch((int)EventKeys.OnTing, new OnTingArgs() { TingChair = mCurrChair });
        }

        protected void TingLogic(ISFSObject data)
        {
            Game.MahjongGroups.Do((groups) =>
            {
                groups.MahjongHandWall[mCurrChair].ThrowOut(mThrowoutCard);
                groups.MahjongHandWall[mCurrChair].SetHandCardState(HandcardStateTyps.Ting);
                var item = groups.MahjongThrow[mCurrChair].GetInMahjong(mThrowoutCard);
                Game.TableManager.ShowOutcardFlag(item);
            });
        }
    }
}
