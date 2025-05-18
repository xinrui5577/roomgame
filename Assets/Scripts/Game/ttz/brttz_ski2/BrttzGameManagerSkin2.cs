using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Enums;

namespace Assets.Scripts.Game.ttz
{
    public class BrttzGameManagerSkin2 : BrttzGameManager
    {
        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            base.OnGetGameInfo(gameInfo);
            if (StartStatus == 30 || StartStatus == 3 || StartStatus == 20)
            {
                BrttzCardsCtrl.SendMingCardFirst(gameInfo);
            }
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            base.GameResponseStatus(type, response);
            var gdata = App.GetGameData<BrttzGameData>();
            switch (type)
            {
                case RequestType.SendMingCards:
                    YxDebug.Log("----------------发一张明牌:----" + type);
                    ResetData();
                    BrttzCardsCtrl.GetIsXiPai(response);
                    gdata.SetGameStatus(YxEGameStatus.Play);
                    BrttzCardsCtrl.BeginGiveMingCards(response);
                    break;
            }
        }
    }
}