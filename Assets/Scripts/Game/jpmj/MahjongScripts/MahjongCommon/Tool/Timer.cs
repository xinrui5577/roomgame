using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool
{
    public class Timer : MonoBehaviour
    {
        public static Timer Instance;

        protected System.Random _ran;
        void Awake()
        {
            Instance = this;
        
            int iSeed = 10;
            System.Random ro = new System.Random(10);
            long tick = DateTime.Now.Ticks;
            _ran = new System.Random((int)(tick & 0xffffffffL) | (int)(tick >> 32)); 
        }

        protected int _index;
        protected class TimerItem
        {
            public Coroutine cor;
            public DVoidNoParam call;

            public TimerItem(Coroutine cor, DVoidNoParam call)
            {
                this.cor = cor;
                this.call = call;
            }
        }

        protected Dictionary<int,TimerItem> _dicTimerItem = new Dictionary<int, TimerItem>();

        public static int StartTimer(float time,DVoidNoParam callBack)
        {
            return Instance.StartTimeLoop(time,callBack);
        }

        public static void StopTimer(int index)
        {
            Instance.StopLoop(index);
        }
        protected int StartTimeLoop(float time, DVoidNoParam callBack)
        {
            _index = _ran.Next();
            while (_dicTimerItem.ContainsKey(_index))
            {
                _index = _ran.Next();
            }
            Coroutine cor = StartCoroutine(TimeLoop(time, callBack));
            _dicTimerItem.Add(_index,new TimerItem(cor,callBack));
            return _index;
        }

        protected IEnumerator TimeLoop(float time, DVoidNoParam callBack)
        {
            yield return new WaitForSeconds(time);

            callBack();
        }

        protected void StopLoop(int index)
        {
            if (_dicTimerItem.ContainsKey(index))
            {
                StopCoroutine(_dicTimerItem[index].cor);
            }
        }

        void OnDestroy()
        {
            Instance = null;
        }
    }
}
