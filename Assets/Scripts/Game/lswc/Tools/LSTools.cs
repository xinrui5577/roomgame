using System.Collections.Generic;
using UnityEngine;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.lswc.Tools
{
    public class LSTimeSpan 
    {

        private float _spanTime;

        private float _cacheTime;

        private bool _repeat;

        private float _totalTime;

        public delegate void TimeSpanEvent();

        public TimeSpanEvent OnTimeFrameFinished;

        public TimeSpanEvent OnTimeFinished;

        public LSTimeSpan(bool repeat=true,float time=1,float totalTime=0) 
        {
            _spanTime = time;

            _cacheTime = time;

            _repeat = repeat;

            if(totalTime==0)
            {
                _totalTime = time;
            }
            else
            {
                _totalTime = totalTime;
            }        
        }

        public void Run()
        {
            _cacheTime -= Time.deltaTime;
            if (_cacheTime <= 0)
            {
                if (OnTimeFrameFinished != null)
                {
                    OnTimeFrameFinished();
                }
                if (_repeat)
                {
                    _cacheTime = _spanTime;
                }
                else
                {
                    if (OnTimeFinished != null)
                    {
                        OnTimeFinished();
                    }
                }
                _totalTime -= _spanTime;
                if (_totalTime<=0)
                {
                    _totalTime = 0;
                    if (OnTimeFinished != null)
                    {
                        OnTimeFinished();
                    }
                }

            }
        }
    }

    public class SafeQueue<T>
    {
        Queue<T> queue = new Queue<T>();

        public void Clear()
        {
            lock (queue)
            {
                queue.Clear();
            }
        }

        public int Count
        {
            get
            {
                lock (queue)
                {
                    return queue.Count;
                }
            }
        }

        public void Enqueue(T value)
        {
            lock (queue)
            {
                queue.Enqueue(value);
            }
        }

        public T Dequeue()
        {
            lock (queue)
            {
                return queue.Dequeue();
            }
        }

        public T Peek()
        {
            lock (queue)
            {
                return queue.Peek();
            }
        }

        public void DestroySelf()
        {
            lock (queue)
            {
                Clear();
                queue = null;
            }
        }

    }

    public class LSTools
    {
        public static float GetRadian(float angle)
        {
            return Mathf.PI*angle/180;
        }

        /// <summary>
        /// 获取对面位置
        /// </summary>
        /// <returns>The opposite position.</returns>
        /// <param name="position">Position.</param>
        public static  int GetOppositePosition(int position)
        {
            if (position<0)
            {
                position = 24+position;
            }
            var opposite = 0;
            switch (position)
            { 
                case 7:
                    opposite = 19;
                    break;
                case 8:
                    opposite = 20;
                    break;
                case 9:
                    opposite = 21;
                    break;
                case 10:
                    opposite = 22;
                    break;
                case 11:
                    opposite = 23;
                    break;
                case 12:
                    opposite = 0;
                    break;
                case 13:
                    opposite = 1;
                    break;
                case 14:
                    opposite = 2;
                    break;
                case 15:
                    opposite = 3;
                    break;
                case 16:
                    opposite = 4;
                    break;
                case 17:
                    opposite = 5;
                    break;
                case 18:
                    opposite = 18;
                    break;
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                    opposite = position;
                    break;
                default:
                    YxDebug.LogError("no find position:" + position);
                    break;
            }
            return opposite;
        }
    }
}
