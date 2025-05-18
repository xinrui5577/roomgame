using Assets.Scripts.Game.fillpit.Mgr;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.fillpit.skin2
{
    public class LaddyMgrSk2 : LaddyMgr
    {

        public UILabel TurnLabel;

        public UILabel TurnBetLabel;

        public string TurnFormat = "第 {0} 轮";

        public string TurnBetFormat = "当前下注 : {0}";

        private int _turnBet;
        public override int AllBetMoney
        {
            get { return base.AllBetMoney; }

            set
            {
                AllMoney = value;
                AllBetMoneyLabel.text = YxUtiles.ReduceNumber(AllMoney);//App.GetGameData<GlobalData>().GetShowGold(_allBetMoney);
            }
        }                                                       

        public override void OnDealCard(int round)
        {
            base.OnDealCard(round);
            SetTurnLabel(round);
            TurnLabel.text = round.ToString();
            _turnBet = 0;
            SetTurnBetLabel(_turnBet);
        }

        public override void OnPlayerBet(int gold)
        {
            base.OnPlayerBet(gold);
            _turnBet += gold;
            SetTurnBetLabel(_turnBet);
        }

        private void SetTurnLabel(int round)
        {
            TurnLabel.text = string.Format(TurnFormat, round);
        }


        private void SetTurnBetLabel(int gold)
        {
            string goldVal = YxUtiles.ReduceNumber(gold);
            TurnBetLabel.text = string.Format(TurnBetFormat, goldVal);
        }

        public override void Rest(bool isLandDi = false)
        {
            base.Rest(isLandDi);
            _turnBet = 0;
            TurnBetLabel.text = string.Format(TurnBetFormat, _turnBet);
        }
    }
}
