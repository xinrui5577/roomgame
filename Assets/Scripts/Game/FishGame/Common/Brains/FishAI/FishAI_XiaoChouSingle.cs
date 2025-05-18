using System.Collections;
using Assets.Scripts.Game.FishGame.Common.core;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Common.Brains.FishAI
{
    public class FishAI_XiaoChouSingle : MonoBehaviour, IFishAI
    {
        public float RotateAngleRndRange = 30F;
        public float RotateInterval = 5F;//转向间隔
        public float RotateIntervalRndRange = 1F;//转向随机范围

        public float Accel = 1F;//加速度
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


        IEnumerator _Coro_SpeedDown()
        {
            while (mSwimmer.Speed > 0)
            {
                if (!mIsPause)
                    mSwimmer.Speed -= Accel * Time.deltaTime;


                yield return 0;
            }
            mSwimmer.Speed = 0F;
        }
        // Use this for initialization
        IEnumerator Start()
        {
            mSwimmer = GetComponent<Swimmer>();
            float elapse = 0F;
            float useTime = 0F;
            while (true)
            {
                //等待new
                elapse = 0F;
                useTime = RotateInterval + Random.Range(-RotateIntervalRndRange, RotateIntervalRndRange);
                while (elapse < useTime)
                {
                    if(!mIsPause)
                        elapse += Time.deltaTime;
                    yield return 0;
                } 


                //渐渐减速到0
                float originSpeed = mSwimmer.Speed;
                StartCoroutine("_Coro_SpeedDown");

 
                elapse = 0F;
                useTime = originSpeed / Accel * 0.9F;

                while (elapse < useTime)
                {
                    if (!mIsPause)
                        elapse += Time.deltaTime;
                    yield return 0;
                }


                //YxDebug.Log("originSpeed / Accel * 0.9F = " + (originSpeed / Accel * 0.9F));
                //转向
                //t, iTween.Hash("x", .25, "easeType", "easeInOutBack", "loopType", "pingPong", "delay", .4));
                float rotateDegree = Random.Range(-RotateAngleRndRange, RotateAngleRndRange);
                float needTime = Mathf.Abs(rotateDegree) / mSwimmer.RotateSpd;
                //iTween.RotateAdd(gameObject, iTween.Hash("z", rotateDegree, "easeType", iTween.EaseType.easeOutQuad, "loopType", iTween.LoopType.none, "time", needTime));
                StartCoroutine(_Coro_SwimmerRotate(transform, needTime, rotateDegree));
 
                elapse = 0F;
                useTime = needTime * 0.2F;//等同于 rotateDegree / 90F*0.9F//todo 跟具体角度有关.可能不用百分比
            
                while (elapse < useTime)
                {

                    if (!mIsPause)
                        elapse += Time.deltaTime;

                    yield return 0;
                }

                StopCoroutine("_Coro_SpeedDown");

                //mSwimmer.Speed =  0.2F*originSpeed;
                while (mSwimmer.Speed < originSpeed)
                {
                    if (!mIsPause)
                        mSwimmer.Speed += Accel * 2F * Time.deltaTime;

                    yield return 0;
                }
                mSwimmer.Speed = originSpeed;
            

                //mSwimmer.Rotate(Random.Range(-RotateAngleRndRange, RotateAngleRndRange));
            
            }

        }


        IEnumerator _Coro_SwimmerRotate(Transform tsSwim, float useTime, float rotateDegree)
        {
            float elapse = 0F; 
            Transform ts = transform;
 
            Quaternion tarRotation = ts.rotation * Quaternion.Euler(0F, 0F, rotateDegree);
            Quaternion oriRotation = ts.rotation;


            while (elapse < useTime)
            {

                if (!mIsPause)
                {
                    float turnPc = 1F - elapse / useTime;
                    ts.rotation = Quaternion.Slerp(oriRotation, tarRotation, (1F - turnPc * turnPc));
                    elapse += Time.deltaTime;
                }
                yield return 0;
            }
        }
    }
}
