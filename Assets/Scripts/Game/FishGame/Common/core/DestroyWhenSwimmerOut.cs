using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class DestroyWhenSwimmerOut : MonoBehaviour {

        void Awake()
        {
            Swimmer s = GetComponent<Swimmer>();
            s.EvtSwimOutLiveArea += Handle_SwimOutLiveArea;
        }
 
        void Handle_SwimOutLiveArea()
        { 
            Destroy(gameObject);
        }
    }
}
