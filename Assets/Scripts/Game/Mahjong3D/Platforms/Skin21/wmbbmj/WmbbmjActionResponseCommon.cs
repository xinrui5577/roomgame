
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class WmbbmjActionResponseCommon : ActionCommonResponse
    {
        public override void RollDiceAction(ISFSObject data)
        {
            base.RollDiceAction(data);
            GameCenter.DataCenter.Game.YuleSetLaozhuang();
        }
    }
}
