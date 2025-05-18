using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Game.FishGame.Common.external.NemoPoolGOs
{
    public class Pool_GameObj   {
        public static Dictionary<int, Pool_GameObj> msPoolsDict;
        private static Transform msTsPoolMain;//用来放已回收的GameObject

        private GameObject mPrefab;
        private Stack<GameObject> mPoolGo;
        private int mVolume = 10;//限制数目,大于最大数目则开始删除部分
     
        public Pool_GameObj(GameObject prefab)
        {
            mPrefab = prefab;
            if (mPoolGo == null)
                mPoolGo = new Stack<GameObject>();
        }
        public void GC_Lite()
        {
            if (mPoolGo.Count != 0)
                GameObject.Destroy(mPoolGo.Pop());

        }

        public GameObject GetGO()
        { 
            GameObject outGO ;
            if (mPoolGo.Count == 0 ||(outGO = mPoolGo.Pop())==null)
            {
                outGO = Object.Instantiate(mPrefab);

                var poolObj = outGO.GetComponent("IPoolObj");
                if (poolObj != null)
                    ((IPoolObj)poolObj).Prefab = mPrefab;
            }
            else
            {
                ; 
                var poolObj = outGO.GetComponent("IPoolObj");
                if (poolObj != null)
                    ((IPoolObj)poolObj).On_Reuse(mPrefab); 
            }

        

            return outGO;
        }

    
        public void RecycleGO(GameObject go)
        {
            var poolObj = go.GetComponent("IPoolObj");
            if (poolObj != null)
                ((IPoolObj)poolObj).On_Recycle();

            if (msTsPoolMain == null)
            {
                msTsPoolMain = new GameObject("PoolObjectMain").transform;
                msTsPoolMain.gameObject.isStatic = true;
            }

            //大于限制容量则马上删除
            if (mPoolGo.Count > mVolume)
            {
                GameObject.Destroy(go);
            }
            else
            {
                go.transform.parent = msTsPoolMain;
                mPoolGo.Push(go);
            }
        }

        /// <summary>
        /// 游戏初始化时调用
        /// </summary>
        public static void Init()
        {
            Pool_GameObj_GC_Interval.StartGC();
        }

        public static GameObject GetObj(GameObject prefab)
        {
            if (msPoolsDict == null)
            {
                msPoolsDict = new Dictionary<int, Pool_GameObj>();
            }

            //找出对应的PoolGameObject
            Pool_GameObj poolGo = null;
            if (!msPoolsDict.TryGetValue(prefab.GetInstanceID(),out poolGo))
            {
                poolGo = new Pool_GameObj(prefab);
                msPoolsDict.Add(prefab.GetInstanceID(), poolGo);
            }

            return poolGo.GetGO();
        }


        public static bool RecycleGO(GameObject prefab,GameObject instGO)
        {
            if (msPoolsDict == null)
            {
                msPoolsDict = new Dictionary<int, Pool_GameObj>();
            }

            //找出对应的PoolGameObject
            if(prefab == null)
            {
                var poolObj = instGO.GetComponent(typeof(IPoolObj)) as IPoolObj;
                if (poolObj == null) return false;
                    prefab = poolObj.Prefab;
                if (prefab == null)
                {
                    //Debug.LogWarning("noPrefab ="+instGO.name);
                    return false;
                }
            }

            Pool_GameObj poolGo = null;
            if (!msPoolsDict.TryGetValue(prefab.GetInstanceID(), out poolGo))
            {
                poolGo = new Pool_GameObj(prefab);
                msPoolsDict.Add(prefab.GetInstanceID(), poolGo);
            } 
            poolGo.RecycleGO(instGO);
            return true;
        }

        //设置池容量
        public static void SetPoolVolume(GameObject prefab,int Volume)
        {
            if (msPoolsDict == null)
            {
                msPoolsDict = new Dictionary<int, Pool_GameObj>();
                return;
            }

            //找出对应的PoolGameObject
            Pool_GameObj poolGo = null;
            if (!msPoolsDict.TryGetValue(prefab.GetInstanceID(), out poolGo))
            {
                poolGo = new Pool_GameObj(prefab);
                msPoolsDict.Add(prefab.GetInstanceID(), poolGo);
            }

            poolGo.mVolume = Volume;
        }

        public static void Clear()
        {
            if (msPoolsDict == null) return;
            msPoolsDict.Clear(); 
        } 
    }
}
