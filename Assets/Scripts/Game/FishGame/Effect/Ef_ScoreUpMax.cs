using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.core;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_ScoreUpMax : MonoBehaviour {
        public tk2dSpriteAnimator Prefab_AniScoreMax;

        private static readonly float Local_OffsetYToPlayer = 112.7882496F;
        private tk2dSpriteAnimator[] mAniScoreMaxSignature;
 

        // Use this for initialization
        void Start () {
            mAniScoreMaxSignature = new tk2dSpriteAnimator[Defines.NumPlayer];
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtPlayerScoreChanged += Handle_ScoreChanged;
            gdata.EvtMainProcessStartGame += Handle_StartGame;

        }
        void Handle_StartGame()
        {
            var main = GameMain.Singleton;
            var players = main.PlayersBatterys;
            var playersScore = main.BSSetting.Dat_PlayersScore;
            var count = players.Count;
            for (var i = 0; i != Defines.NumPlayer; ++i)
            {
                if (i < count)
                {
                    Handle_ScoreChanged(players[i], playersScore[i].Val, 0);
                }
            }
        }
        void Handle_ScoreChanged(Player p, int scoreNew, int scoreChanged)
        {
            if (scoreNew >= Defines.NumScoreUpMax)
            {
                if (mAniScoreMaxSignature[p.Idx] == null)
                {
                    mAniScoreMaxSignature[p.Idx] = Instantiate(Prefab_AniScoreMax) as tk2dSpriteAnimator;
                    mAniScoreMaxSignature[p.Idx].transform.parent = p.transform;
                    mAniScoreMaxSignature[p.Idx].transform.localPosition = new Vector3(0F, Local_OffsetYToPlayer, -0.002F);
                    mAniScoreMaxSignature[p.Idx].transform.localRotation = Quaternion.identity;
                }
            }
            else
            {
                if (mAniScoreMaxSignature[p.Idx] != null)
                {
                    Destroy(mAniScoreMaxSignature[p.Idx].gameObject);
                    mAniScoreMaxSignature[p.Idx] = null;
                }
            }
        }
    }
}
