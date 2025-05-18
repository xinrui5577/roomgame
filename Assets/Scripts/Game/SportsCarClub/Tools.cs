using System.Collections;
using UnityEngine;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.SportsCarClub
{
    public class Tools : MonoBehaviour {

        /// <summary>
        /// 为NGUI对象添加点击事件
        /// </summary>
        /// <param name="gob">点击对象</param>
        /// <param name="callback">监听事件</param>
        /// <param name="ID">ID</param>
        public static void NguiAddOnClick(GameObject gob, UIEventListener.VoidDelegate callback,int ID)
        {
            UIEventListener uiel = UIEventListener.Get(gob);
            uiel.onClick = callback;
            uiel.parameter = ID;
        }

        /// <summary>
        /// 根据名字只显示唯一的gob
        /// </summary>
        public static GameObject GobShowOnlyOne(GameObject[] gobs,string name)
        {
            int index = -1;

            for (int i = 0; i < gobs.Length; i++)
            {
                GameObject gob = gobs[i];
                if (gob.name.Equals(name))
                {
                    gob.SetActive(true);
                    index = i;
                }
                else
                {
                    gob.SetActive(false);
                }
            }

            return index == -1 ? null : gobs[index];
        }

        /// <summary>
        /// 根据名字返回gob对象
        /// </summary>
        /// <returns></returns>
        public static GameObject GobSelectName(GameObject[] gobs,string name)
        {
            foreach (GameObject gob in gobs)
            {
                if (gob.name.Equals(name))
                {
                    return gob;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取字典int
        /// </summary>
        public static int GetDicInt(IDictionary dic,object key)
        {

            if (!dic.Contains(key) || dic[key] == null) return 0;

            return int.Parse(dic[key].ToString());

        }
        /// <summary>
        /// 获取字典bool
        /// </summary>
        public static bool GetDicBool(IDictionary dic, object key)
        {
            return dic.Contains(key) && dic[key] != null && (bool) dic[key];
        }
        /// <summary>
        /// 获取字典string
        /// </summary>
        public static string GetDicString(IDictionary dic, object key)
        {
            if (!dic.Contains(key) || dic[key] == null) return "";

            return dic[key].ToString();
        }
    }
}
