using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    /// <summary>
    /// 粒子炮相关
    /// </summary>
    public class GunLiziGettorMain : MonoBehaviour {
        public float LiziRatio = 0.0033F;
        private GunLiziGettor[] LiziGettors;
        void Awake()
        { 
            LiziGettors = new GunLiziGettor[Defines.NumPlayer]; 
        }

        private void Start()
        {
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtPlayerGainScoreFromFish += Handle_PlayerGainScoreFromFish;
            gdata.EvtMainProcessStartGame += Handle_StartGame;
        }

        void Handle_StartGame()
        {
            var players = GameMain.Singleton.PlayersBatterys;
            if(players==null)return;
            var count = players.Count;
            for (var i=0;i<count;i++)
            {
                var p = players[i];
                LiziGettors[p.Idx] = p.GetComponent<GunLiziGettor>();
            }
        }

        void Handle_PlayerGainScoreFromFish(Player p, int score, Fish firstFish, int bulletScore)
        {

            //几率获得离子炮 
            //if (firstFish.HittableTypeS == "Normal" && p.GunInst.GetPowerType() != GunPowerType.Lizi)
            if (firstFish.HittableType == HittableType.Normal && p.PowerType != GunPowerType.Lizi)
            {
                if (Random.Range(0F, 1F) < LiziRatio)
                {
                    LiziGettors[p.Idx].GetLiziKaFrom(firstFish.transform.position);
                }
            }
        }
    }
}
