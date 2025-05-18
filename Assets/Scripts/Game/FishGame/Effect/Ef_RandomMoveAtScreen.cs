using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_RandomMoveAtScreen : MonoBehaviour {

        public float BoundCircleRadius = 1F;//绑定园半径
        public float Speed = 1F;
        private Rect mLiveDimension;//可生存区域,在初始化时计算
        private Transform mTs;
        private static readonly float CheckLiveInterval = 0.2F;//检查是否超出生存范围时间
        private float mCheckLiveRemainTime = 0F;
        private Vector3 mMoveDirection;
        public bool IsInLiveArea()
        {
            return mLiveDimension.Contains(mTs.position);
        }
     
        // Use this for initialization
        void Awake () {
            mTs = transform;
            mLiveDimension.x = GameMain.Singleton.WorldDimension.x + BoundCircleRadius;
            mLiveDimension.y = GameMain.Singleton.WorldDimension.y + BoundCircleRadius;
            mLiveDimension.width = GameMain.Singleton.WorldDimension.width - 2F * BoundCircleRadius;
            mLiveDimension.height = GameMain.Singleton.WorldDimension.height - 2F * BoundCircleRadius;
            mMoveDirection = Random.onUnitSphere;
            mMoveDirection.z = 0F;
            mMoveDirection.Normalize();
        }
     
        void Update()
        {
            mTs.position += Speed * Time.deltaTime * mMoveDirection;

            if (mCheckLiveRemainTime < 0F)
            {
                if (!IsInLiveArea())
                {
                    //向中心点移动
                    mMoveDirection = -mTs.position + (Vector3)((Vector2)Random.onUnitSphere).normalized * 0.5F;
                    mMoveDirection.z = 0F;
                    mMoveDirection.Normalize();
                }

                mCheckLiveRemainTime = CheckLiveInterval;
            }
            mCheckLiveRemainTime -= Time.deltaTime;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, BoundCircleRadius);
        }
#endif
    }
}
