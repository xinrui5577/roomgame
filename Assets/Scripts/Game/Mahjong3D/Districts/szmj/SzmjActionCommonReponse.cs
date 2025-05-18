using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class SzmjActionCommonReponse : ActionCommonResponse
    {
        public override void RollDiceAction(ISFSObject data)
        {
            if (data.ContainsKey("diling"))
            {
                DataCenter.Game.Diling = data.TryGetInt("diling");
            }
            if (data.ContainsKey("baozi"))
            {
                DataCenter.Game.Baozi = data.TryGetInt("baozi");
            }
            base.RollDiceAction(data);         
        }
    }
}