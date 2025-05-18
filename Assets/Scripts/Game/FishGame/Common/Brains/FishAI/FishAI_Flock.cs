using Assets.Scripts.Game.FishGame.Common.core;
using UnityEngine;

//群落
namespace Assets.Scripts.Game.FishGame.Common.Brains.FishAI
{
    public class FishAI_Flock : MonoBehaviour ,IFishAI
    { 
        private float DistanceToLeader = 38.4F;
     
        private Swimmer mLeader;
        private Transform mTsLeader;
        private Swimmer mSwimmer;
        private Vector3 mDirectionRndForce;
        private Transform mTs;
        private bool mIsPause = false;


        public void Pause()
        {
 
            mIsPause = true;
        }
        public void Resume()
        { 
            mIsPause = false;
        }

        float mCohesionDistance;
        Vector3 mDirectToLeader;

        float mElapseRotate;
        float mTimeRotate = 1F;
        float mSpdRotate = 180F;
        float mAngleAddtiveRotate;
        bool mIsRotating;
        Vector3 mRndForceRotate;//扰乱距离
        float mRotateSpdWithDirect;

        float mElapseIntervalRotate;
        float mTimeIntervalRotate;//0.1-2.5
 

        void Start()
        {
            mSwimmer = GetComponent<Swimmer>();
            mTs = transform;
            mCohesionDistance = DistanceToLeader + mLeader.BoundCircleRadius + mSwimmer.BoundCircleRadius;//内聚距离

            mIsRotating = true;

            mTimeRotate = 1F;
            mSpdRotate = 180F;
        
            mRndForceRotate = Random.insideUnitCircle.normalized * 96F;
            mTimeIntervalRotate = Random.Range(0.1F, 2.5F);
            mRotateSpdWithDirect = (Random.Range(0, 2) == 0 ? -1F : 1F) * mSpdRotate;
        }

        void Update()
        {
            if (mIsPause)
                return;
            if (mTsLeader == null)
                return;

            mDirectToLeader = mTsLeader.position - mTs.position;
            mDirectToLeader.z = 0F;
            //Quaternion.FromToRotation()
            mTs.right = mTsLeader.right * 384F//跟像素有关
                        + mDirectToLeader.magnitude / mCohesionDistance * mDirectToLeader//向心影响力 = 指向leader向量 * leader距离/内聚距离比
                        + mDirectionRndForce;//扰乱距离 

            if (mIsRotating)
            {
                //elapse = 0F;
             
                //while (elapse < useTime)
                //{
                //    if (mIsPause)
                //    {
                //        yield return 0;
                //        continue;
                //    }

                mAngleAddtiveRotate += mRotateSpdWithDirect * Time.deltaTime;
                mDirectionRndForce = Quaternion.Euler(0F, 0F, mAngleAddtiveRotate) * mRndForceRotate;
                mElapseRotate += Time.deltaTime;
                //yield return 0;
                //}

                if (mElapseRotate > mTimeRotate)
                {
                    mIsRotating = false;
                    mElapseRotate = 0F;
                }

                //yield return new WaitForSeconds(Random.Range(0.1F, 2.5F));
                //}
            }
            else
            {
                mElapseIntervalRotate += Time.deltaTime;
                if (mElapseIntervalRotate > mTimeIntervalRotate)
                {
                    mIsRotating = true;
                    mTimeIntervalRotate = Random.Range(0.1F, 2.5F);
                    mRotateSpdWithDirect = (Random.Range(0, 2) == 0 ? -1F : 1F) * mSpdRotate;
                    mElapseIntervalRotate = 0F;
                }
            }
        }

        //IEnumerator Start()
        //{
        //    mSwimmer = GetComponent<Swimmer>();
        //    mTs = transform;
        //    float cohesionDistance = DistanceToLeader + mLeader.BoundCircleRadius + mSwimmer.BoundCircleRadius;//内聚距离
        //    StartCoroutine(_Coro_RndRotate());
        //    Vector3 directToLeader;
        //    while (mTsLeader != null)
        //    {
        //        if (mIsPause)
        //        {
        //            yield return 0;
        //            continue;
        //        }
        //        directToLeader = mTsLeader.position - mTs.position;
        //        directToLeader.z = 0F;
        //        //Quaternion.FromToRotation()
        //        mTs.right = mTsLeader.right * 384F//跟像素有关
        //            + directToLeader.magnitude / cohesionDistance * directToLeader//向心影响力 = 指向leader向量 * leader距离/内聚距离比
        //            + mDirectionRndForce;//扰乱距离 

        //        yield return 0;
        //    }

        //}

 
        //IEnumerator _Coro_RndRotate()
        //{
        //    yield return new WaitForSeconds(Random.Range(0F, 3F));

        //    Vector3 rndForce = Random.insideUnitCircle.normalized * 96F;//扰乱距离
  
        //    float rotateSpd = 180F;
        //    float angleAdditive = 0F;

        //    float useTime = 1F;
        //    float elapse = 0F;
        //    while (true)
        //    {
        //        elapse = 0F;

        //        float rotateSpdWithDirect = (Random.Range(0, 2) == 0 ? -1F : 1F) * rotateSpd;
        //        while (elapse < useTime)
        //        {
        //            if (mIsPause)
        //            {
        //                yield return 0;
        //                continue;
        //            }

        //            angleAdditive += rotateSpdWithDirect * Time.deltaTime; 
        //            mDirectionRndForce = Quaternion.Euler(0F,0F,angleAdditive) * rndForce;
        //            elapse += Time.deltaTime;
        //            yield return 0;
        //        }

        //        yield return new WaitForSeconds(Random.Range(0.1F, 2.5F));
        //    }
        //}
        public void SetLeader(Swimmer tar)
        {
            mLeader = tar;
            mTsLeader = tar.transform;
        }
    }
}
