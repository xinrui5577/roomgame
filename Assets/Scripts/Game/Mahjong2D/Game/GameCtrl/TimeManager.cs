using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl
{
    /// <summary>
    /// 计时器，感觉这个功能很常用。于是，它就诞生了。喜欢写N多个协程
    /// </summary>
    public class TimeManager : MonoSingleton<TimeManager>
    {

        private static List<TimeSpan> _spans;

        private static List<TimeSpan> _delSpans; 

        public bool AllStar;

        private static readonly object _safeLock = new object();

        public override void Awake()
        {
            base.Awake();
            _spans = new List<TimeSpan>();
            _delSpans=new List<TimeSpan>();
            AllStar = false;
        }


        public TimeSpan AddSpan(float frequency = 1, bool forever = false,float allTime=1,Action oneFinished = null, Action all =null)
        {
            TimeSpan span = new TimeSpan(frequency, forever,allTime, oneFinished,all);
            _spans.Add(span);
            span.allFinished += delegate()
                {
                    _delSpans.Add(span);
                };
            return span;
        }

        public void RemoveSpan(TimeSpan span)
        {
            lock (_safeLock)
            {
                _spans.Clear();
                _spans.Remove(span);
            }
        }

        public void SetState(bool startState)
        {
            AllStar = startState;
        }

        private void Update()
        {
            if (AllStar)
            {
                lock (_safeLock)
                {
                    foreach (var span in _spans)
                    {
                        span.Run();
                    }
                    if (_delSpans.Count>0)
                    {
                        for (int i = 0; i < _delSpans.Count; i++)
                        {
                            RemoveSpan(_delSpans[i]);
                        }
                        _delSpans.Clear();
                    }
                }
            }
        }

    }


    public class TimeSpan
    {
        private float _frequency;

        private float _cacheTime;

        private bool _forever;

        private float _allTime;

        private bool RunState;

        public Action oneFinished;

        public Action allFinished;
        /// <summary>
        /// 这个就是用来刷新时间的类
        /// </summary>
        /// <param name="frequency">刷新频率</param>
        /// <param name="forever">是否一直刷新</param>
        /// <param name="allTime">总刷新时间，在forever为false时生效</param>
        /// <param name="one">刷新一次的触发</param>
        public TimeSpan(float frequency = 1, bool forever = false, float allTime = 1, Action one = null, Action all =null)
        {
            _frequency = frequency;
            _forever = forever;
            _allTime = allTime;
            oneFinished = one;
            allFinished = all;
            _cacheTime = _frequency;
            RunState = true;
        }

        public void Run()
        {
            if (RunState)
            {
                _cacheTime -= Time.deltaTime;
                if (!_forever)
                {
                    _allTime -= Time.deltaTime;
                    if (_allTime <= 0)
                    {
                        if (allFinished != null)
                        {
                            allFinished();
                        }
                    }          
                }
                if (_cacheTime <= 0)
                {
                    _cacheTime = _frequency;
                    if (oneFinished != null)
                    {
                        oneFinished();
                    }
                    if (!_forever)
                    {
                        //Clear();
                    }
                }
            }

        }

        public void Clear()
        {
            _frequency = 0;
            _forever = false;
            oneFinished = null;
            allFinished = null;
            RunState = false;
            _cacheTime = 0;
        }
    }
}