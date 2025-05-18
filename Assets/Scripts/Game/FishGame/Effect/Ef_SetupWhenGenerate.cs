using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_SetupWhenGenerate : MonoBehaviour {

        public Transform Prefab_TsEffect;
 

        void Msg_FishGenerateWhenEnterWorld_Generated(Fish f)
        {
            Transform ts = Instantiate(Prefab_TsEffect) as Transform;
            ts.parent = f.transform;
            ts.localPosition = Vector3.zero;
            ts.localRotation = Quaternion.identity;
        }
    }
}
