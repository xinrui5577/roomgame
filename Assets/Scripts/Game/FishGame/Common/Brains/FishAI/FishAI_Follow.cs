using Assets.Scripts.Game.FishGame.Common.core;
using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.FishGame.Common.Brains.FishAI
{
    public class FishAI_Follow : MonoBehaviour ,IFishAI
    {
        public float DistanceToLeader = 0.2F;//Óëleader¾àÀë


        private Swimmer mLeader;
        //private Transform mTsTarget;
        private Vector3 mDirection;
        private Vector3 mPreDirectionUnNor;
        //private Transform mTs;
        private Swimmer mSwimmer;

        private bool mIsPause = false;
        public void Pause()
        {
            mIsPause = true;
        }
        public void Resume()
        {
            mIsPause = false;
        }

        void Awake()
        {
            //mTs = transform;
            mSwimmer = GetComponent<Swimmer>();
        }

        void OnDestroy()
        {
            if(mLeader != null)
                mLeader.EvtRotateStart -= Handle_TargetFishRotateStart;
        }

        //old ¸ú×ÙÂß¼­
        //void Update()
        //{
        //    if (mTsTarget == null)
        //        return;

        //    mDirection = mTsTarget.localPosition - mTs.localPosition;
        //    if (mPreDirectionUnNor == mDirection)
        //        return;

        //    mPreDirectionUnNor = mDirection;    

        //    if (mDirection.magnitude < Distance + mFish.BoundCircleRadius*2F)
        //        mFish.StopImm();
        //    else
        //        mFish.Go();
        
        //    mDirection.Normalize();
        //    mTs.right = mDirection;

        //}
        void Handle_TargetFishRotateStart(float angle)
        {
            StartCoroutine(_Coro_FollowRotate(angle));
        }

        IEnumerator _Coro_FollowRotate(float angle)
        {
            float waitTime = DistanceToLeader / mSwimmer.Speed;
            float elapse = 0F;
            while (elapse < waitTime)
            {
                if (!mIsPause) 
                    elapse += Time.deltaTime;

                yield return 0;
            }
            //yield return new WaitForSeconds(waitTime);
            mSwimmer.Rotate(angle);
        }
        public void SetTarget(Swimmer leader)
        {
            mLeader = leader;
            //mTsTarget = leader.transform;
            mLeader.EvtRotateStart += Handle_TargetFishRotateStart;
        }

    }
}
