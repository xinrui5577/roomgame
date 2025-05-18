using UnityEngine;

namespace Assets.Scripts.Game.lswc.Core
{
    public abstract class InstanceControl : MonoBehaviour
    {

        protected void OnApplicationQuit()
        {
            Destroy(this);
        }

        public abstract void OnExit();

        protected void OnDestory()
        {
            OnExit();
        }

    }
}
