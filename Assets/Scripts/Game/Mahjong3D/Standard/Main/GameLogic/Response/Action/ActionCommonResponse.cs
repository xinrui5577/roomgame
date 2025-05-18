using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionCommonResponse : AbsCommandAction
    {        
        public virtual void GameOverAction(ISFSObject data)
        {           
            //var db = GameCenter.DataCenter;
            //db.IsGameOver = true;
            //db.Game.SetTotalResult(data);
            //GameCenter.Instance.SetIgonreReconnectState(true);
            if (GameCenter.DataCenter.DissolvedState || !GameCenter.Controller.SingleHuState)
            {                
                var fsm = GameCenter.GameProcess;
                if (!fsm.IsCurrState<StateGameEnd>())
                {
                    fsm.ChangeState<StateGameEnd>();
                }
                else if (GameCenter.DataCenter.ConfigData.ContinueNewGame)
                {
                    //切换继续开局状态
                    fsm.ChangeState<StateGameContinue>();
                }
                GameCenter.EventHandle.Dispatch((int)EventKeys.ShowTotalResult);
            }
        }

        public virtual void RollDiceAction(ISFSObject data)
        {
            GameCenter.Hud.UIPanelController.PlayUIEffect(PoolObjectType.start);
            MahjongUtility.PlayEnvironmentSound("gamestart");

            DataCenter.BankerSeat = data.GetInt(AnalysisKeys.KeyStartP);
            DataCenter.SaiziPoint = data.GetIntArray(AnalysisKeys.KeyDiceArray);
            DataCenter.CurrOpSeat = DataCenter.BankerSeat;
            GameCenter.GameProcess.ChangeState<StateGamePlaying>();
        }

        public virtual void QueryHuCardAction(ISFSObject data)
        {
            var card = data.GetInt("card");
            var arr = data.GetIntArray("hulist");
            var ratelist = data.TryGetIntArray("ratelist");
            if (null != arr && arr.Length > 0)
            {
                GameCenter.EventHandle.Dispatch<QueryHuArgs>((int)EventKeys.QueryHuCard, (param) =>
                {
                    param.QueryCard = card;
                    param.AllowHuCards = arr;
                    param.RateArray = ratelist;
                });
            }
        }

        public virtual void CheckCardsAction(ISFSObject data)
        {
            var cards = data.GetIntArray("cards");
            string output = "";
            for (int i = 0; i < cards.Length; i++)
            {
                output += " " + cards[i];
            }
            YxDebug.LogEvent("错误后同步的手牌 :" + output);

            //重连请求
            GameCenter.Network.SendReJoinGame();
        }

        public virtual void FenzhangAction(ISFSObject data)
        {
            if (data.ContainsKey("cards"))
            {
                DataCenter.Game.FenzhangFlag = true;
                var arr = data.GetSFSArray("cards");
                DataCenter.LeaveMahjongCnt -= arr.Count;
                for (int i = 0; i < arr.Count; i++)
                {
                    var obj = arr.GetSFSObject(i);
                    if (obj != null)
                    {
                        var value = obj.GetInt("card");
                        var chair = MahjongUtility.GetChair(obj.GetInt("seat"));
                        if (value > 0)
                        {
                            Game.MahjongGroups.CurrGetMahjongWall.PopMahjong();
                            var temp = Game.MahjongCtrl.PopMahjong(value);
                            if (temp != null)
                            {
                                var mahjong = temp.GetComponent<MahjongContainer>();
                                mahjong.Laizi = DataCenter.IsLaizi(value);
                                Game.MahjongGroups.MahjongOther[chair].GetInMahjong(mahjong);
                            }
                        }
                        DataCenter.Players[chair].FenzhangCard = value;
                    }
                }
                //隐藏箭头
                Game.TableManager.HideOutcardFlag();
            }
        }

        public virtual void CustomLogicAction(ISFSObject data) { }
    }
}
