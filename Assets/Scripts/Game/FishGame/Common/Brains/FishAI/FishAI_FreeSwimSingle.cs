using Assets.Scripts.Game.FishGame.Common.core;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Common.Brains.FishAI
{
    public class FishAI_FreeSwimSingle : MonoBehaviour, IFishAI
    {
        public float RotateAngleRndRange = 30F; 
        public float RotateInterval = 5F;//转向间隔
        public float RotateIntervalRndRange = 1F;//转向随机范围

        private Swimmer mSwimmer;

        private bool mIsPause = false;
        private float mElapse;
        private float mTimeRotate;
        void Start()
        {
            //StartCoroutine("_Coro_RotateInterval");
            mSwimmer = GetComponent<Swimmer>();
            mTimeRotate = RotateInterval + Random.Range(-RotateIntervalRndRange, RotateIntervalRndRange);
        }

        void Update()
        {
            if (mIsPause)
                return;

            if (mElapse > mTimeRotate)
            {
                mSwimmer.Rotate(Random.Range(-RotateAngleRndRange, RotateAngleRndRange));
                mElapse = 0F;
                mTimeRotate = RotateInterval + Random.Range(-RotateIntervalRndRange, RotateIntervalRndRange);
            }
            else
            {
                mElapse += Time.deltaTime;
            }
        }

        public void CopyDataTo(FishAI_FreeSwimSingle tar)
        {
            tar.RotateAngleRndRange = RotateAngleRndRange;
            tar.RotateInterval = RotateInterval;
            tar.RotateIntervalRndRange = RotateIntervalRndRange;
        }
        public void Pause()
        {
            mIsPause = true;
            //StopCoroutine("_Coro_RotateInterval");
        }
        public void Resume()
        {
            mIsPause = false;
            //StartCoroutine("_Coro_RotateInterval");
        }

        //IEnumerator _Coro_RotateInterval()
        //{
        //    mSwimmer = GetComponent<Swimmer>();
        //    while (true)
        //    {
        //        yield return new WaitForSeconds(RotateInterval + Random.Range(-RotateIntervalRndRange, RotateIntervalRndRange));

        //        mSwimmer.Rotate(Random.Range(-RotateAngleRndRange, RotateAngleRndRange));
        //    }
        //}


	

    }
}
