using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.toubao
{
    public class UserPanel : YxBaseGamePlayer
    {
        public UILabel OneRoundBet;
        public UILabel AllRoundBet;
        private long _onceBet;
        private long _allBet;
        public long OnceBet
        {
            get { return _onceBet; }
            set { _onceBet = value; }
        }
        public long AllBet
        {
            get { return _allBet; }
            set { _allBet = value; }
        }
        public void SetRoundBetShow()
        {
            OneRoundBet.text = YxUtiles.ReduceNumber(OnceBet);
            AllRoundBet.text = YxUtiles.ReduceNumber(AllBet);
        }

        public void InitOnce()
        {
            OnceBet = 0;
            OneRoundBet.text = YxUtiles.ReduceNumber(OnceBet);
        }
    }
}
