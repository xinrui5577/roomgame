using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Enums;

namespace Assets.Scripts.Game.brnn.brnn_skin01
{
    public class BrnnGameManager01 : BrnnGameManager
    {
        public override void GameResponseStatus(int type, ISFSObject response)
        {
            var gdata = App.GetGameData<BrnnGameData>();
            switch (type)
            {
                case RequestType.Bet:
                    BetCtrl.Bet(response);
                    ProgressCtrl.RefreshNum(response);
                    ShowNumCtrl.RefreshNum(response);
                    if (response.GetInt("seat") == gdata.SelfSeat)
                    {
                        if (gdata.GetPlayerInfo().CoinA < gdata.MiniApplyBanker)
                        {
                            ApplyCtrl.Instance.SetStutus(2);
                        }
                    }
                    break;
                case RequestType.BeginBet:
                    gdata.SetGameStatus(YxEGameStatus.Play);
                    gdata.BeginBet = true;
                    CardsCtrl.ReSetPonits();
                    CardsCtrl.ReSetGiveCardsStatus();
                    ShowNumCtrl.Reset();
                    ReSetGame();
                    BetCtrl.Reset();

                    ProgressCtrl.ReSetCountdown(16);
                    ProgressCtrl.BeginCountdown();
                    ApplyCtrl.RefreshBanker();
                    break;
            
                default:
                    base.GameResponseStatus(type, response);
                    return;
            }
            OnGetPlayers(response);
        }
    }
}