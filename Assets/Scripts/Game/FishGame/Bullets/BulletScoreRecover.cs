using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.core;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.Bullets
{
    public class BulletScoreRecover : MonoBehaviour
    {
        public delegate void Evt_BulletScoreStartRecover(int playerIdx, int score);
        public static Evt_BulletScoreStartRecover EvtBulletScoreStartRecover;
        private BackStageSetting mBss;

        private void Awake()
        {
            mBss = GameMain.Singleton.BSSetting;
        }

        private void Start()
        {
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtPlayerGunFired += Handle_PlayerGunFired;
            gdata.EvtBulletDestroy += Handle_PlayerBulletDestroy;
            gdata.EvtMainProcessFirstEnterScene += Handle_FirstEnterScene;
        }
         
        void Handle_FirstEnterScene()
        {
//            foreach (Player p in GameMain.Singleton.Players)
//            {
//                var score = mBss.Dat_PlayersBulletScore[p.Idx];
//                if (score!=null && mBss.Dat_PlayersBulletScore[p.Idx].Val > 0)
//                {
//                    if(EvtBulletScoreStartRecover != null)
//                        EvtBulletScoreStartRecover(p.Idx, mBss.Dat_PlayersBulletScore[p.Idx].Val);
//
//                    //mBss.Dat_PlayersScore[p.Idx].Val += mBss.Dat_PlayersBulletScore[p.Idx].Val;
//                    p.ChangeScore(mBss.Dat_PlayersBulletScore[p.Idx].Val);
//                    mBss.Dat_PlayersBulletScore[p.Idx].SetImmdiately(0);
//                }
//            }
        }

        void _Coro_ScoreChanged(int pidx,int s)
        {
            mBss.Dat_PlayersBulletScore[pidx].Val += s; 
        }

        void Handle_PlayerGunFired(Player p, Gun gun, int useScore, bool isLock,int bulletId)
        {
            _Coro_ScoreChanged(p.Idx, useScore);
            //mBss.Dat_PlayersBulletScore[p.Idx].Val += useScore;
        }

        void Handle_PlayerBulletDestroy(Bullet b)
        { 
            _Coro_ScoreChanged(b.Owner.Idx, -b.Score);
            //mBss.Dat_PlayersBulletScore[playerIdx].Val -= score;
        }
     
        void OnDestroy()
        {
            EvtBulletScoreStartRecover = null;
        }
    }
}
