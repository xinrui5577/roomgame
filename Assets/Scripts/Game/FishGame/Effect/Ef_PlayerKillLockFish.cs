using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.core;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_PlayerKillLockFish : MonoBehaviour {
        public GameObject Prefab_Effect;
        // Use this for initialization
        void Start () {
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtKillLockingFish += Handle_KillLockFish;   
        }
        void Handle_KillLockFish(Player p)
        {
            var worldPos = p.transform.position;
            worldPos.z = Defines.GlobleDepth_BombParticle;
            var goEf = Instantiate(Prefab_Effect);
            goEf.transform.parent = transform;
            goEf.transform.position = worldPos;

        }
    }
}
