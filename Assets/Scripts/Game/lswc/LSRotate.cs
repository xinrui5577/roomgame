using DG.Tweening;
using UnityEngine;
using System.Collections;

public class LSRotate : MonoBehaviour {

public delegate void StartFunction();

        public delegate void FinishedFunction();

        public event StartFunction StartEvent;

        public event FinishedFunction FinishedEvent;
            
        public float AddAmount;

        //	[Tooltip("旋转完成所需要的总共时间")]
        public float TotalTtime;

        public float StartPosition;

        public float endPosition;

        private float AvailableTime;

        public bool IsEnable = false;

        public float RunTime = 0, percentage = 0;

        float rotate; 

        private float tempNm;

        void Update()
        {

            if (IsEnable)
            {


                RunTime += Time.deltaTime;


                percentage = RunTime / TotalTtime;

                if (percentage <= 1)
                {
                    rotate = easeOutExpo(StartPosition, endPosition, percentage);
                    SetRotate(rotate);

                    if (percentage == 1)
                    {
                        Endrotate();
                    }
                }
                else
                {

                    Endrotate();
                }

            }

        }

    private float easeOutExpo(float start, float end, float value)
    {
        end -= start;
        return end * (-Mathf.Pow(2, -10 * value) + 1) + start;
    }

    public void SetStartEvent(StartFunction se)
        {

            this.StartEvent += se;
        }

        public void SetFinishedEvent(FinishedFunction fe)
        {
            this.FinishedEvent += fe;
        }


        public void StartRotate(float availabletime, float TotalTime, float AddAmount, float delay)
        {

            this.TotalTtime = TotalTime;
            this.AddAmount = AddAmount;
            this.AvailableTime = availabletime;
            InitStart();
            InitEnd();
            InitRunTime();
            IsEnable = true;
            if (StartEvent != null)
            {
                StartEvent();
            }

          //  RotateTween();
        }



        public void Endrotate()
        {

            IsEnable = false;
            //SetRotate(endPosition);
            Vector3 ToEnd = this.transform.localEulerAngles;
            ToEnd.y = endPosition % 360;



            iTween.RotateTo(this.gameObject, iTween.Hash(
                "rotation", ToEnd,
                "time", 0.5f,
                "easetype", iTween.EaseType.easeOutCubic
                ));


            if (FinishedEvent != null)
            {
                FinishedEvent();
            }
        }



        void InitStart()
        {
            Vector3 start = this.transform.localEulerAngles;

            StartPosition = start.y;
            // YxDebug.LogError("StartPosition" + StartPosition);

        }

        void InitEnd()
        {

            endPosition = StartPosition + AddAmount;
            // YxDebug.LogError("endPosition" + endPosition);
        }

        void InitRunTime()
        {

            if (AvailableTime >= TotalTtime)
            {
                RunTime = 0;
                AvailableTime = TotalTtime;
            }
            else
            {
                RunTime = TotalTtime - AvailableTime;
            }
        }
        void SetRotate(float rotate)
        {

            Vector3 start = this.transform.localEulerAngles;
            start.y = rotate;
            this.transform.localEulerAngles = start;
        }


        private void RotateTween()
        {
            //Sequence mySequence = DOTween.Sequence();
            ////加速
            //float time1 = this.AvailableTime * 0.25f;
            //float add1 = this.AddAmount * 0.25f+StartPosition;
            //Tweener t1 = playTween(add1, time1);
            //t1.SetEase(Ease.InCubic);
            //mySequence.Append(t1);
            ////减速
            //float time2 = this.AvailableTime * 0.4f;
            //float add2 = this.AddAmount * 0.6f+StartPosition;
            //Tweener t2 = playTween(add2, time2);
            //t2.SetEase(Ease.OutCubic);
            //mySequence.Append(t2);

            ////再减速

            //float temp = endPosition - this.AddAmount * 0.6f;
            //float tempTime=this.AvailableTime * 0.35f;
            //int num = (int)temp / 15;
            //float time = tempTime / num;

            //for (int i = 0; i < num; i++)
            //{
            //    float next = add2 + (i + 1) * 15;

            //    Tweener t = playTween(next, time);
            //    mySequence.Append(t);
            //    mySequence.AppendInterval(0.1f);
            //}

            //float time3 = tempTime-(time+0.1f)*num;
            //if (time3 < 0.1f)
            //{
            //    time3 = 0.1f;
            //}
            //float add3 = endPosition;
            //Tweener t3 = playTween(add3, time3);
            //t3.SetEase(Ease.Linear);
            //mySequence.Append(t3);
            //mySequence.onComplete = Endrotate;

            Tweener t = playTween(endPosition, this.AvailableTime);
            t.SetEase(Ease.InOutCirc);

            t.OnComplete(Endrotate);
        }

        private Tweener playTween(float endy, float time)
        {

         Tweener t=   DOTween.To(()=>this.transform.localEulerAngles.y,rotate=>SetRotate(rotate),endy,time);
         return t;

        }


        //private float easeInOutQuad(float start, float end, float value)
        //{
        //    value /= .5f;
        //    end -= start;
        //    if (value < 1) return end / 2 * value * value + start;
        //    value--;
        //    return -end / 2 * (value * (value - 2) - 1) + start;
        //}
        //public float easeInOutCubic(float start, float end, float value)
        //{
        //    value /= .5f;
        //    end -= start;
        //    if (value < 1) return end / 2 * value * value * value + start;
        //    value -= 2;
        //    return end / 2 * (value * value * value + 2) + start;
        //}
        //public float easeInOutExpo(float start, float end, float value)
        //{
        //    value /= .5f;
        //    end -= start;
        //    if (value < 1) return end / 2 * Mathf.Pow(2, 10 * (value - 1)) + start;
        //    value--;
        //    return end / 2 * (-Mathf.Pow(2, -10 * value) + 2) + start;
        //}
    }