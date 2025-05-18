using Sfs2X.Entities.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.ddz2.DDz2Common
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class DDzUtil{

        /// <summary>
        /// 比较两个数组结构的数据是否相等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array1">按某顺序排序好的数据</param>
        /// <param name="array2">按某顺序排序好的数据</param>
        public static bool IsTwoArrayEqual<T>(T[] array1, T[] array2)
        {
            if (array1 == null || array2 == null) return false;

            var len = array1.Length;

            if (len != array2.Length) return false;

            for (var i = 0; i < len; i++)
            {
                if (!array1[i].Equals(array2[i])) return false;
            }
            return true;
        }

        /// <summary>
        /// 检查某个数组的值，是不是另外要给数组的子集，（不包含完全和父集合完全相等的情况，那种情况用IsTwoArrayEqual方法）
        /// </summary>
        /// <param name="array">排序好的父集合</param>
        /// <param name="subsetArray">排序好的可能是完全子集（不和array完全重合）的数组</param>
        /// <returns></returns>
        public static bool IsSubsetArray(int[]array,int[]subsetArray)
        {
            if (array.Length <= subsetArray.Length) return false;

            var arrayLen = array.Length;
            var subsetArrayList = new List<int>();
            subsetArrayList.AddRange(subsetArray);
            for (int i = 0; i < arrayLen; i++)
            {
                subsetArrayList.Remove(array[i]);
            }

            return subsetArrayList.Count==0;
        }


        /// <summary>
        /// 是否包含这些key
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="isfObjData"></param>
        /// <returns></returns>
        public static bool IsServDataContainAllKey(string[]keys,ISFSObject isfObjData)
        {
            for (int i = 0; i < keys.Length;i++)
                if (!isfObjData.ContainsKey(keys[i])) return false;

            return true;
        }

      
        /// <summary>
        /// 清理playergrid
        /// </summary>
        public static void ClearPlayerGrid(GameObject girdGob)
        {
            var tran = girdGob.transform;
            tran.DestroyChildren();
        }

        //返回例子特效的播放时间
        public static float ParticleSystemLength(Transform transform)
        {
            ParticleSystem[] particleSystems = transform.GetComponentsInChildren<ParticleSystem>();
            float maxDuration = 0;
            foreach (ParticleSystem ps in particleSystems)
            {
                if (ps.emission.enabled)
                {
                    if (ps.loop)
                    {
                        return -1f;
                    }
                    float dunration;
                    if (ps.emissionRate <= 0)
                    {
                        dunration = ps.startDelay + ps.startLifetime;
                    }
                    else
                    {
                        dunration = ps.startDelay + Mathf.Max(ps.duration, ps.startLifetime);
                    }
                    if (dunration > maxDuration)
                    {
                        maxDuration = dunration;
                    }
                }
            }
            return maxDuration;
        }
    }
}
