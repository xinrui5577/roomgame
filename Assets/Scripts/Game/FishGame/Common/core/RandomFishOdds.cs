using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class RandomFishOdds : MonoBehaviour {
        public Fish[] Prefab_RandomsFish;
        // Use this for initialization
        void Start () {
            if (Prefab_RandomsFish == null || Prefab_RandomsFish.Length == 0)
                return;

            var rndFish = Prefab_RandomsFish[Random.Range(0, Prefab_RandomsFish.Length)];
            if (rndFish == null)
            {
                YxDebug.LogError("在Prefab_RandomsFish中有鱼已经删除了~!检查下!");
                return;
            }
            var f = GetComponent<Fish>();
            f.Odds = rndFish.Odds;
            f.Prefab_GoAniDead = rndFish.Prefab_GoAniDead; 
        }
    }
}
