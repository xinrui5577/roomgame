using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Game.FishGame.Common.external.NemoPoolGOs
{
    /// <summary>
    /// pool GameObject的定时回收类
    /// </summary>
    /// <remarks>
    /// 从GameMain处独立出来
    /// </remarks>
    public class Pool_GameObj_GC_Interval : MonoBehaviour {
        public static float Interval_PoolGC = 1F;//池回收一个对象的时间

        private static Pool_GameObj_GC_Interval mSingleton;
    
        /// <summary>
        /// 开始收集,无论调用多少次都只开始一次
        /// </summary>
        public static void StartGC()
        {
            if(mSingleton == null)
            {
                GameObject go = new GameObject("Pool_GameObject_GC_Interval");
                mSingleton = go.AddComponent<Pool_GameObj_GC_Interval>();
                //只开启一次
                mSingleton.StartCoroutine(mSingleton._Coro_ClearPool());
            }
        }

        IEnumerator _Coro_ClearPool()
        {
 
            List<Pool_GameObj> clearQueue = new List<Pool_GameObj>();
            while (true)
            {
                if (Pool_GameObj.msPoolsDict != null){
                    foreach (KeyValuePair<int, Pool_GameObj> kvp in Pool_GameObj.msPoolsDict)
                    {
                        clearQueue.Add(kvp.Value);
                    }
                }
                foreach (Pool_GameObj obj in clearQueue)
                {
                    obj.GC_Lite();
                }
                clearQueue.Clear();
                yield return new WaitForSeconds(Interval_PoolGC);
            }
        }
    }
}
