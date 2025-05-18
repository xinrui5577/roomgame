using Sfs2X.Entities.Data;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class YkmjActionReconnect : ActionReconnect
    {
        public override void ReconnectAction(ISFSObject data)
        {
            base.ReconnectAction(data);
            Action<ISFSObject> handler = null;
            handler = GameCenter.GameLogic.DispatchResponseHandlers(CustomProl.CustomLogic);
            if (null != handler)
            {
                handler(data);
            }
        }
    }
}
