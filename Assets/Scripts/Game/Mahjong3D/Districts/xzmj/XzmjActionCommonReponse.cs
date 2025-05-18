using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class XzmjActionCommonReponse : ActionCommonResponse
    {
        public override void RollDiceAction(ISFSObject data)
        {
            base.RollDiceAction(data);
            GameCenter.EventHandle.Dispatch((int)EventKeys.HideHuFlag);
            GameCenter.Controller.ForbbidToken = DataCenter.ConfigData.HasDingQue
                || DataCenter.ConfigData.HasHuanZhang;
        }
    }
}
