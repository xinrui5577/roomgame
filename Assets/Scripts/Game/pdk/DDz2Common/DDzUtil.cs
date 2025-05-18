using Sfs2X.Entities.Data;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.pdk.DDz2Common
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class DDzUtil{
        const float BtnColorAlphaDisable = 0.7f;

        /// <summary>
        /// 设置玩家头像信息
        /// </summary>
        /// <param name="userdata"></param>
        /// <param name="headTexture"></param>
        public static void SetUserHeadTexture(ISFSObject userdata, UITexture headTexture)
        {
            short sex = 0;
            if (userdata.ContainsKey(NewRequestKey.KeySex)) sex = userdata.GetShort(NewRequestKey.KeySex);

            if (userdata.ContainsKey(NewRequestKey.KeyAvatar))
            {
                LoadRealHeadIcon(userdata.GetUtfString(NewRequestKey.KeyAvatar), sex, headTexture);
            }
            else
            {
                LoadDefaultHeadIcon(sex, headTexture);
            }
        }

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
        /// 激活按钮
        /// </summary>
        public static void ActiveBtn(GameObject btnSp,GameObject disbtnSp)
        {
            btnSp.SetActive(true);
            disbtnSp.SetActive(false);

        }
        /// <summary>
        /// 显示按钮，但是是失效状态
        /// </summary>
        public static void DisableBtn(GameObject btnSp, GameObject disbtnSp)
        {
            btnSp.SetActive(false);
            disbtnSp.SetActive(true);
        }

        /// <summary>
        /// 设置默认头像
        /// </summary>
        public static void LoadDefaultHeadIcon(short sex, UITexture headTexture)
        {
            string assetName = sex == 0 ? "headtexture0" : "headtexture1";
            var textureGob = ResourceManager.LoadAsset(assetName, assetName);
            if (headTexture != null && textureGob!=null) headTexture.mainTexture = textureGob.GetComponent<UITexture>().mainTexture;
        }

        /// <summary>
        /// 加载真实头像
        /// </summary>
        /// <param name="headImgUrl">头像地址</param>
        /// <param name="sex">按性别加载默认头像</param>
        /// <param name="headTexture"></param>
        public static void LoadRealHeadIcon(string headImgUrl, short sex,UITexture headTexture)
        {
            //加载真实头像
            Facade.Instance<AsyncImage>()
                  .GetAsyncImage(headImgUrl, (tex,code) =>
                  {
                      if (tex != null)
                      {
                          headTexture.mainTexture = tex;
                      }
                      else
                      {
                          LoadDefaultHeadIcon(sex, headTexture);
                      }
                  });
        }


        /// <summary>
        /// 清理playergrid
        /// </summary>
        public static void ClearPlayerGrid(GameObject girdGob)
        {
            var childCount = girdGob.transform.childCount;
            var gobs = new GameObject[childCount];
            for (int i = 0; i < childCount; i++)
            {
                gobs[i] = girdGob.transform.GetChild(i).gameObject;
            }
            for (int i = 0; i < childCount; i++)
            {
                Object.DestroyImmediate(gobs[i]);
            }
        }

        //返回例子特效的播放时间
        public static float ParticleSystemLength(Transform transform)
        {
            ParticleSystem[] particleSystems = transform.GetComponentsInChildren<ParticleSystem>();
            float maxDuration = 0;
            foreach (ParticleSystem ps in particleSystems)
            {
                if (ps.enableEmission)
                {
                    if (ps.loop)
                    {
                        return -1f;
                    }
                    float dunration = 0f;
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

        /// <summary>
        /// //int数组排序方法 从小到大
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int SortCdsintes(int[] x, int[] y)
        {
            if (x[0] > y[0]) return 1;
            if (x[0] == y[0]) return 0;
            return -1;
        }

        /// <summary>
        /// //int数组排序方法 根据数组长度排序
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int SortCdsintesByLen(int[] x, int[] y)
        {
            if (x.Length > y.Length) return 1;
            if (x.Length == y.Length) return 0;
            return -1;
        }
    }
}
