using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class BbmjActionCpg : ActionCpg
    {
        public override void ResponseSelfGangAction(ISFSObject data)
        {
            ResponseCpgAction(data);
        }

        public override void ResponseCpgAction(ISFSObject data)
        {
            CpgLogic(data);
            switch (mCpgType)
            {
                case EnGroupType.Chi:
                    MahjongUtility.PlayOperateEffect(DataCenter.CurrOpChair, PoolObjectType.chi);
                    break;
                case EnGroupType.Peng:
                    MahjongUtility.PlayOperateEffect(DataCenter.CurrOpChair, PoolObjectType.peng);
                    break;
                default:
                    string soundName = "gang";
                    if (mCpgType == EnGroupType.MingGang)
                    {
                        soundName = "minggang";
                    }
                    else if (mCpgType == EnGroupType.AnGang)
                    {
                        soundName = "angang";
                    }
                    MahjongUtility.PlayPlayerSound(mCurrOpChair, soundName);
                    MahjongUtility.PlayOperateEffect(mCurrOpChair, PoolObjectType.gang);
                    MahjongUtility.PlayEnvironmentEffect(mCurrOpChair, PoolObjectType.longjuanfeng);
                    PlayScoreEffect(data);
                    break;
            }
        }
    }
}
