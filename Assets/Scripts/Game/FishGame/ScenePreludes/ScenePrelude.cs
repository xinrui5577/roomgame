using Assets.Scripts.Game.FishGame.Common.core;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.ScenePreludes
{
    public class ScenePrelude : MonoBehaviour
    {
    
        public Event_Generic Evt_PreludeEnd;

        void Awake()
        {
            Evt_PreludeEnd += FormationEnd;
        }
     
        public void FormationEnd()
        {
            if (Evt_PreludeEnd != null) Evt_PreludeEnd -= FormationEnd;
            UnityEngine.Resources.UnloadUnusedAssets();
        }

        public virtual void Go() { }

        public virtual void Pause() { }
        public virtual void Resume() { }

    }
}
