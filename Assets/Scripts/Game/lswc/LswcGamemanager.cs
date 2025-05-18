using Assets.Scripts.Game.lswc.Data;
using Assets.Scripts.Game.lswc.Item;
using Assets.Scripts.Game.lswc.Manager;
using Assets.Scripts.Game.lswc.Scene;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.lswc
{
    public class LswcGamemanager : YxGameManager
    {
        public LSCameraManager CameraManager;
        public LSAnimalItemControl AnimalItemCtrl;
        public LSUIManager UIManager;
        public LSColorItemControl ColorItemControl;
        public LSBigThreeReward BigThreeReward;
        public LSShowGameTypeManager ShowGameTypeManager;
        public LSSystemControl SystemControl;
        public LSTurnTableControl TurnTableControl;
        public LSOperationManager OperationManager;
        public LSResourseManager ResourseManager;
        public LSCrystalControl CrystalControl;
        public LSFireWorksControl FireWorksControl;

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            SystemControl.InitState();
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int status, ISFSObject info)
        {
        }

        protected override string GetStateField()
        {
            return LSConstant.KeyGameStatus;
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var gdata = App.GetGameData<LswcGameData>();
            YxDebug.Log("<color=green>收到了服务器返回的消息，消息类型是：" + type + "</color>");
            switch (type)
            {
                case (int)LSRequestMessageType.ON_BEGIN_BET:
                    gdata.OnNewPage(response);
                    var gstate = gdata.GameStates;
                    gstate.EmptyState.NextState = gstate.BetState;//LSBetState.Instance;
                    gstate.EmptyState.Update();
                    break;
                case (int)LSRequestMessageType.ON_COLLECT_BET:
                    App.GetRServer<LswcGameServer>().SendBetRequest();
                    break;
                case (int)LSRequestMessageType.ON_GET_RESULT:
                    gdata.ISGetResult = true;
                    gdata.InitNewResult(response);
                    break;
                case (int)LSRequestMessageType.BET:
                    YxDebug.LogError("下注成功");
                    break;
            }
        }
    }
}
