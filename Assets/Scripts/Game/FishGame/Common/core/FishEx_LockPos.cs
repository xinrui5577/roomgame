using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class FishEx_LockPos : MonoBehaviour {

        public Vector3 LocalOffset;
        //public Vector3 LocalBulletOffset;
        public Vector3 LockPos
        {
            get { return   mTs.position + mTs.rotation * LocalOffset; }
        }
        public Vector3 LockBulletPos
        {
            get { return  mTs.position + mTs.rotation * LocalOffset; }
        }
        private Transform mTs;
        void Awake()
        {
            mTs = transform;
        }
    }
}
