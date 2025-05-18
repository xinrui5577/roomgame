using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class DelayTimer : ShortcutsPart
    {
        protected Dictionary<int, TimerItem> mDicTimerItem = new Dictionary<int, TimerItem>();
        protected System.Random mRan;
        protected int mIndex;

        void Awake()
        {
            long tick = DateTime.Now.Ticks;
            mRan = new System.Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
        }

        protected class TimerItem
        {
            public Coroutine cor;
            public Action call;

            public TimerItem(Coroutine cor, Action call)
            {
                this.cor = cor;
                this.call = call;
            }
        }

        public int StartTimeLoop(float time, Action callBack)
        {
            mIndex = mRan.Next();
            while (mDicTimerItem.ContainsKey(mIndex))
            {
                mIndex = mRan.Next();
            }
            Coroutine cor = StartCoroutine(TimeLoop(time, callBack, mIndex));
            mDicTimerItem.Add(mIndex, new TimerItem(cor, callBack));
            return mIndex;
        }

        public IEnumerator TimeLoop(float time, Action callBack, int index)
        {
            yield return new WaitForSeconds(time);
            callBack();
            mDicTimerItem.Remove(index);
        }

        public void StopLoop(int index)
        {
            if (mDicTimerItem.ContainsKey(index))
            {
                StopCoroutine(mDicTimerItem[index].cor);
                mDicTimerItem.Remove(index);
            }
        }
    }
}