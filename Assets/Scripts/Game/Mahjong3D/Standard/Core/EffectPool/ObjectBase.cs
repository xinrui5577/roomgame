using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public abstract class ObjectBase : MonoBehaviour
    {
        public bool AutoRecycle;
        public float DelayRecycleTime;

        private bool mIsExecute;
        private float mTimer;

        private void Update()
        {
            if (AutoRecycle && mIsExecute)
            {
                mTimer += Time.deltaTime;
                if (mTimer >= DelayRecycleTime)
                {
                    GameCenter.Pools.Push(this);
                    Reset();
                }
            }
        }

        public virtual void Reset()
        {
            mTimer = 0;
            mIsExecute = false;
        }

        public virtual void Execute()
        {
            mIsExecute = true;
        }

        public virtual void Recycle()
        {
            mTimer = 0;
            mIsExecute = false;
            GameCenter.Pools.Push(this);
        }
    }
}