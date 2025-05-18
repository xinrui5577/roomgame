using Assets.Scripts.Game.brnn.brnn_skin02;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace Assets.Scripts.Game.brnn.brnn_skin03
{
    public class BrnnGameManager03 : BrnnGameManager02
    {
        public TablePlayerManager TabelPlayerMgr;

        public override void OnGetGameInfo(ISFSObject requestData)
        {
            base.OnGetGameInfo(requestData);
            TabelPlayerMgr.InitTablePlayers(requestData);
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            base.GameResponseStatus(type, response);
            switch (type)
            {
                case RequestType.BeginBet:
                    TabelPlayerMgr.InitTablePlayers(response);
                    break;
            }
            
        }

    }
}
