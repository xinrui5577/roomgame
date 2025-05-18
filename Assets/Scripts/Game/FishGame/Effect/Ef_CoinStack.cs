using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_CoinStack : MonoBehaviour {

        private CoinStackManager[] CoinStacks;//索引对应所属player的idx
	

        void Awake () 
        {
            CoinStacks = new CoinStackManager[Defines.MaxNumPlayer];
        }

        private void Start()
        {
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtPlayerGainScoreFromFish += Handle_PlayerGainScoreFromFish;
            gdata.EvtPlayerWonScoreChanged += Handle_PlayerWonScoreChanged;
            gdata.EvtMainProcessStartGame += Handle_StartGame;
        }

        void Handle_StartGame()
        {
            var main = GameMain.Singleton;
            var players = main.PlayersBatterys;
            var scoreWons = main.BSSetting.Dat_PlayersScoreWon;
            if (players == null) return;
            var count = players.Count;
            foreach (var p in players)
            {
                CoinStacks[p.Idx] = p.Ef_CoinStack;
                if (scoreWons[p.Idx].Val != 0)
                {
                    p.Ef_CoinStack.OneStack_SetNum(scoreWons[p.Idx].Val);
                }
            } 
        }
        void Handle_PlayerGainScoreFromFish(Player p, int score, Fish fishFirst,int bulletScore)
        {
            var outBountyTypeVal = GameMain.Singleton.BSSetting.OutBountyType_.Val;
            if (outBountyTypeVal != OutBountyType.OutCoinButtom && outBountyTypeVal != OutBountyType.OutTicketButtom) return;
            var oddNum = fishFirst.HittableType == HittableType.Normal ? fishFirst.Odds : (score / bulletScore);
            if (p != null)
            {
                var f = CoinStacks[p.Idx];
                if (f != null)
                {
                    f.RequestViewOneStack(score, oddNum, bulletScore);
                }
            }
        }

        void Handle_PlayerWonScoreChanged(Player p, int scoreNew)
        {
            CoinStacks[p.Idx].OneStack_SetNum(scoreNew);
        }
    }
}
